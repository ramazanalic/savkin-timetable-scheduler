﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace SchedulerProject.Core
{
    public class TimeTableData
    {
        private TimeTableData()
        {
        }

        public string Id;
        public int Days;
        public int SlotsPerDay;
        public int TotalTimeSlots;

        //Monday, ... Saturday
        //First = 1, Second = 2, ... Sixth = 6.
        //Tuple<Day, Period> GetDayAndPeriod(int timeSlotId, int daysCount, int slotsPerDay) 

        /// <summary>
        /// Rooms ordered by id.
        /// </summary>
        public Room[] Rooms;

        /// <summary>
        /// Events ordered by id. ???
        /// </summary>
        public Event[] Events;
        public Subject[] Subjects;
        public Group[] Groups;
        public Lecturer[] Lecturers;

        public Event[] FirstWeekEvents;
        public Event[] SecondWeekEvents;

        // helpers 
        Dictionary<int, HashSet<int>> eventConflicts;
        Dictionary<int, HashSet<int>> groupEvents;

        public static TimeTableData MakeEmpty()
        {
            byte[] bytes = new byte[10];
            new Random().NextBytes(bytes);

            return new TimeTableData()
            {
                Id = string.Join("-", bytes.Select(b => b.ToString("X"))),
                Days = 6,
                SlotsPerDay = 5,
                Rooms = new Room[0],
                Lecturers = new Lecturer[0],
                Groups = new Group[0],
                Subjects = new Subject[0],
                Events = new Event[0]
            };
        }

        public static TimeTableData LoadFromXml(string filename)
        {
            var data = XDocument.Load(filename);
            var mainNode = data.Element("SchedulerInput");

            var roomsQuery = from e in mainNode.Element("Rooms").Elements()
                             let room = new Room()
                             {
                                 Id = int.Parse(e.Attribute("id").Value),
                                 Type = ParseHelper.ParseEnum<RoomType>(e.Attribute("type").Value),
                                 Housing = e.Attribute("house_n").Value,
                                 RoomNumber = e.Attribute("class_n").Value,
                                 TimeConstraints = GetTimeConstrains(e)
                             }
                             orderby room.Id
                             select room;

            var lecturersQuery = from e in mainNode.Element("Lecturers").Elements()
                                 select new Lecturer()
                                 {
                                     Id = int.Parse(e.Attribute("id").Value),
                                     Name = e.Attribute("name").Value,
                                     TimeConstraints = GetTimeConstrains(e)
                                 };

            var groupsQuery = from courseElement in mainNode.Element("Groups").Elements()
                              let courseNumber = int.Parse(courseElement.Attribute("number").Value)
                              from groupElement in courseElement.Elements()
                              let gr = new Group()
                              {
                                  Id = int.Parse(groupElement.Attribute("id").Value),
                                  Name = groupElement.Attribute("name").Value,
                                  Course = courseNumber
                              }
                              orderby gr.Id
                              select gr;

            var subjectsQuery = from e in mainNode.Element("Subjects").Elements()
                                select new Subject()
                                {
                                    Id = int.Parse(e.Attribute("id").Value),
                                    LecturerId = GetLecturerId(e),
                                    Name = e.Attribute("name").Value
                                };

            var eventsQuery = from subjectElement in mainNode.Element("Subjects").Elements()
                              let subjectId = int.Parse(subjectElement.Attribute("id").Value)
                              let subjectLecturer = GetLecturerId(subjectElement)
                              from eventElement in subjectElement.Elements()
                              select new Event()
                              {
                                  Id = int.Parse(eventElement.Attribute("id").Value),
                                  HardAssignedRoom = GetHardAssignedRoom(eventElement),
                                  OnceInTwoWeeks = GetOnceInTwoWeeks(eventElement),
                                  SubjectId = subjectId,
                                  LecturerId = (GetLecturerId(eventElement) ?? subjectLecturer).Value,
                                  RoomType = ParseHelper.ParseEnum<RoomType>(eventElement.Attribute("type").Value),
                                  Groups = Event.ParseGroups(eventElement.Attribute("groups").Value)
                              };
            
             return new TimeTableData()
             {
                 Id = mainNode.Attribute("id").Value,
                 Days = int.Parse(mainNode.Attribute("days").Value),
                 SlotsPerDay = int.Parse(mainNode.Attribute("slots_per_day").Value),
                 Rooms = roomsQuery.ToArray(),
                 Lecturers = lecturersQuery.ToArray(),
                 Groups = groupsQuery.ToArray(),
                 Subjects = subjectsQuery.ToArray(),
                 Events = eventsQuery.ToArray()
             };

            //timeTable.CalculateHelpers();

            //return timeTable;
        }

        public void SaveToXml(string filename)
        {
            XElement root = new XElement("SchedulerInput",
                new XAttribute("id", Id),
                new XAttribute("days", Days),
                new XAttribute("slots_per_day", SlotsPerDay),
                new XElement("Rooms", 
                             from r in Rooms 
                             select new XElement("Room",
                                                 new XAttribute("id", r.Id),
                                                 new XAttribute("house_n", r.Housing),
                                                 new XAttribute("class_n", r.RoomNumber),
                                                 GetTimeConstrainsAttribute(r.TimeConstraints),
                                                 new XAttribute("type", r.Type))),
                new XElement("Groups", from g in Groups 
                                       group g by g.Course into c 
                                       select new XElement("Course",
                                                           new XAttribute("number", c.Key), 
                                                           from gc in c select 
                                                           new XElement("Group",
                                                                         new XAttribute("id", gc.Id), 
                                                                         new XAttribute("name", gc.Name)))),
                new XElement("Lecturers", 
                             from l in Lecturers 
                             select new XElement("Lecturer",
                                                 new XAttribute("id", l.Id),
                                                 new XAttribute("name", l.Name),
                                                 GetTimeConstrainsAttribute(l.TimeConstraints))),
                new XElement("Subjects", 
                             from s in Subjects 
                             select new XElement("Subject",
                                                 new XAttribute("id", s.Id),
                                                 new XAttribute("name", s.Name),
                                                 new XAttribute("lecturer_id", s.LecturerId.HasValue ? s.LecturerId.ToString() : string.Empty),
                                                 from e in Events 
                                                 where e.SubjectId == s.Id
                                                 select new XElement("Event",
                                                                     new XAttribute("id", e.Id),
                                                                     new XAttribute("type", e.RoomType), 
                                                                     new XAttribute("groups", Event.GroupsToString(e.Groups)),
                                                                     GetHardAssignedRoomAttribute(e.HardAssignedRoom),
                                                                     GetOnceInTwoWeeksAttribute(e.OnceInTwoWeeks),
                                                                     new XAttribute("lecturer_id", e.LecturerId)))));
            root.Save(filename);
        }

        void PrepareWeeklyPartition()
        {
            var r = new Random();

            var partition = from e in Events
                            where e.OnceInTwoWeeks
                            group e by e.SubjectId into gr
                            let events = gr.Shuffle(r)
                            select new
                            {
                                First = events.Take(gr.Count() / 2),
                                Second = events.Skip(gr.Count() / 2)
                            };

            var onlyFirstWeekEvents = new List<Event>();
            var onlySecondWeekEvents = new List<Event>();

            foreach (var group in partition)
            {
                var biggerGroup = group.Second;
                var smallerGroup = group.First;

                if (group.First.Count() > group.Second.Count())
                {
                    biggerGroup = group.First;
                    smallerGroup = group.Second;
                }

                if (onlyFirstWeekEvents.Count > onlySecondWeekEvents.Count)
                {
                    onlyFirstWeekEvents.AddRange(smallerGroup);
                    onlySecondWeekEvents.AddRange(biggerGroup);
                }
                else
                {
                    onlyFirstWeekEvents.AddRange(biggerGroup);
                    onlySecondWeekEvents.AddRange(smallerGroup);
                }
            }
            FirstWeekEvents = Events.Except(onlySecondWeekEvents).ToArray();
            SecondWeekEvents = Events.Except(onlyFirstWeekEvents).ToArray();
        }

        public Event[] GetWeekEvents(int week)
        {
            switch (week)
            {
                case 1: return FirstWeekEvents;
                case 2: return SecondWeekEvents;
                default: throw new ArgumentException("week");
            }
        }

        public bool GroupHasEvent(int groupId, int eventId)
        {
            return groupEvents[groupId].Contains(eventId);
        }

        public bool ConflictingEvents(int firstEventId, int secondEventId)
        {
            return eventConflicts[firstEventId].Contains(secondEventId);
        }

        public void PrepareHelpers()
        {
            TotalTimeSlots = Days * SlotsPerDay;

            PrepareWeeklyPartition();

            // calculate groups events
            groupEvents = new Dictionary<int, HashSet<int>>(Groups.Length); //new bool[Groups.Length, Events.Length];
            for (var k = 0; k < Groups.Length; k++)
            {
                var grId = Groups[k].Id;
                groupEvents[grId] = new HashSet<int>();
                for (var j = 0; j < Events.Length; j++)
                {
                    if (Events[j].Groups.Contains(grId))
                        groupEvents[grId].Add(Events[j].Id);
                }
            }

            // calculate events conflicts
            eventConflicts = new Dictionary<int, HashSet<int>>(Events.Length); //new bool[Events.Length, Events.Length];
            for (int i = 0; i < Events.Length; i++)
            {
                eventConflicts[Events[i].Id] = new HashSet<int>();
                for (int j = 0; j < Events.Length; j++)
                    for (int k = 0; k < Groups.Length; k++)
                        if (GroupHasEvent(Groups[k].Id, Events[i].Id) &&
                            GroupHasEvent(Groups[k].Id, Events[j].Id))
                        {
                            eventConflicts[Events[i].Id].Add(Events[j].Id);
                            break;
                        }
            }
        }

        static int GetHardAssignedRoom(XElement element)
        {
            int res;
            var attr = element.Attribute("hard_assigned_room");
            if (attr != null && int.TryParse(attr.Value, out res))
                return res;
            return -1;
        }

        static XAttribute GetHardAssignedRoomAttribute(int hardAssignedRoom)
        {
            return hardAssignedRoom == -1 ? null : new XAttribute("hard_assigned_room", hardAssignedRoom);
        }

        static int? GetLecturerId(XElement element)
        {
            int res;
            var attr = element.Attribute("lecturer_id");
            if (attr != null && int.TryParse(attr.Value, out res))
                return res;
            return null;
        }

        static XAttribute GetOnceInTwoWeeksAttribute(bool onceInTwoWeeks)
        {
            return onceInTwoWeeks ? new XAttribute("once_in_two_weeks", true) : null;
        }

        static bool GetOnceInTwoWeeks(XElement element)
        {
            var attr = element.Attribute("once_in_two_weeks");
            if (attr != null && !string.IsNullOrWhiteSpace(attr.Value))
            {
                try
                {
                    return bool.Parse(attr.Value);
                }
                catch
                {
                    return false;
                }
            }
            return false;     
        }

        static XAttribute GetTimeConstrainsAttribute(TimeConstraints constraints)
        {
            return constraints == null ? null : new XAttribute("time_constraints", constraints);
        }

        static TimeConstraints GetTimeConstrains(XElement element)
        {
            var attr = element.Attribute("time_constraints");
            if (attr != null && !string.IsNullOrWhiteSpace(attr.Value))
            {
                try
                {
                    return TimeConstraints.Parse(attr.Value);
                }
                catch
                {
                    return null;
                }
            }
            return null;            
        }
    }
}
