using System;
using System.Linq;
using System.Xml.Linq;

namespace SchedulerProject.Core
{
    public class TimeTable
    {
        private TimeTable()
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

        // helpers 
        public bool[,] eventConflicts;
        public bool[,] groupEvents;

        public static TimeTable MakeEmpty()
        {
            byte[] bytes = new byte[10];
            new Random().NextBytes(bytes);

            return new TimeTable()
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

        public static TimeTable LoadFromXml(string filename)
        {
            var data = XDocument.Load(filename);
            var mainNode = data.Element("SchedulerInput");

            var roomsQuery = from e in mainNode.Element("Rooms").Elements()
                             let room = new Room()
                             {
                                 Id = int.Parse(e.Attribute("id").Value) - 1,//check room id needs t obe specified in the input file
                                 Type = ParseHelper.ParseEnum<RoomType>(e.Attribute("type").Value),
                                 Housing = int.Parse(e.Attribute("house_n").Value),
                                 RoomNumber = e.Attribute("class_n").Value
                             }
                             orderby room.Id
                             select room;

            var lecturersQuery = from e in mainNode.Element("Lecturers").Elements()
                                 select new Lecturer()
                                 {
                                     Id = int.Parse(e.Attribute("id").Value),
                                     Name = e.Attribute("name").Value
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
                                  SubjectId = subjectId,
                                  LecturerId = (GetLecturerId(eventElement) ?? subjectLecturer).Value,
                                  RoomType = ParseHelper.ParseEnum<RoomType>(eventElement.Attribute("type").Value),
                                  Groups = Event.ParseGroups(eventElement.Attribute("groups").Value)
                              };
            
             return new TimeTable()
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
                                                 new XAttribute("name", l.Name))),
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
                                                                     new XAttribute("hard_assigned_room", e.HardAssignedRoom),
                                                                     new XAttribute("lecturer_id", e.LecturerId)))));
            root.Save(filename);
        }

        public void PrepareHelpers()
        {
            TotalTimeSlots = Days * SlotsPerDay;
            // calculate groups events
            groupEvents = new bool[Groups.Length, Events.Length];
            for (int k = 0; k < Groups.Length; k++)
                for (int j = 0; j < Events.Length; j++)
                {
                    groupEvents[k, j] = Events[j].Groups.Contains(Groups[k].Id);
                }


            // calculate events conflicts
            eventConflicts = new bool[Events.Length, Events.Length];
            for (int i = 0; i < Events.Length; i++)
                for (int j = 0; j < Events.Length; j++)
                    for (int k = 0; k < Groups.Length; k++)
                        if (groupEvents[k, i] && groupEvents[k, j])
                        {
                            eventConflicts[i, j] = true;
                            break;
                        }
        }

        public static int GetHardAssignedRoom(XElement element)
        {
            int res;
            var attr = element.Attribute("hard_assigned_room");
            if (attr != null && int.TryParse(attr.Value, out res))
                return res;
            return -1;
        }

        private static int? GetLecturerId(XElement element)
        {
            int res;
            var attr = element.Attribute("lecturer_id");
            if (attr != null && int.TryParse(attr.Value, out res))
                return res;
            return null;
        }
    }
}
