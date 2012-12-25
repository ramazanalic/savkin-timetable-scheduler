﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text;

namespace SchedulerProject.Core
{
    public class WeeklyEventAssignment //: IEquatable<EventAssignment> ??
    {
        public WeeklyEventAssignment(Event e, Room r, TimeSlot slot, int week)
        {
            Event = e;
            Room = r; 
            TimeSlot = slot;
            Week = week;
        }

        public Event Event { get; private set; }
        public Room Room { get; private set; }
        public int RoomId
        {
            get { return Room == null ? -1 : Room.Id; }
        }
        public TimeSlot TimeSlot { get; private set; }
        public int Week { get; private set; }

        public string ToString(TimeTableData data)
        {            
            var subject = data.Subjects.First(s => s.Id == Event.SubjectId).ToString();
            // TODO: add an event property indicating such way of displaying
            if (subject == "Військова підготовка" || subject == "Фізична підготовка" || subject == "Фізичне виховання")
                return subject;
            var lecturer = data.Lecturers.First(l => l.Id == Event.LecturerId);
            return subject + "\n" + lecturer + "\n" + (Room == null ? "Комната не назначена" : Room.ToString());
        }
    }

    public class EventAssignment
    {
        public EventAssignment(Event e)
        {
            Event = e;
        }
        public Event Event { get; private set; }
        public WeeklyEventAssignment FirstWeekAssignment {get; set;}
        public WeeklyEventAssignment SecondWeekAssignment { get; set; }
    }

    public class IdMismatchException : Exception { };

    public class TimeTable
    {
        public TimeTable(TimeTableData data)
        {
            Data = data;
        }

        public TimeTableData Data { get; private set; }
        public string Name { get; set; }

        Dictionary<Event, EventAssignment> assignments = new Dictionary<Event, EventAssignment>();

        public void AddAssignment(WeeklyEventAssignment assignment)
        {
            Event e = assignment.Event;
            EventAssignment current;
            if (!assignments.TryGetValue(e, out current))
            {
                current = new EventAssignment(e);
                assignments.Add(e, current);
            }

            if (assignment.Week == 1)
                current.FirstWeekAssignment = assignment;
            else
                current.SecondWeekAssignment = assignment;
        }

        public bool RemoveAssignment(Event e)
        {
            return assignments.Remove(e);
        }

        public EventAssignment GetAssignment(Event e)
        {
            EventAssignment result;
            return assignments.TryGetValue(e, out result) ? result : null;
        }

        public IEnumerable<EventAssignment> Assignments
        {
            get { return assignments.Values; }
        }

        public static TimeTable LoadFromXml(TimeTableData appropriateData, string filename)
        {
            var data = XDocument.Load(filename);
            var mainNode = data.Element("TimeTable");

            var id = mainNode.Attribute("time_table_id").Value;
            if (id != appropriateData.Id)
            {
                throw new IdMismatchException();
            }

            var infoQuery = from e in mainNode.Elements("Event")
                            let ev = appropriateData.Events
                                            .FirstOrDefault(ev => ev.Id == int.Parse(e.Attribute("id").Value))
                            select new EventAssignment(ev)
                            {
                                FirstWeekAssignment = ParseWeeklyAssignment(appropriateData, e.Elements("FirstWeek").FirstOrDefault(), ev, 1),
                                SecondWeekAssignment = ParseWeeklyAssignment(appropriateData, e.Elements("SecondWeek").FirstOrDefault(), ev, 2),
                            };

            return new TimeTable(appropriateData)
            {
                Name = mainNode.Attribute("name").Value,
                assignments = infoQuery.ToDictionary(a => a.Event, a => a)
            };
        }

        private static WeeklyEventAssignment ParseWeeklyAssignment(TimeTableData appropriateData, XElement elem, Event ev, int week)
        {
            if (elem != null)
            {
                var room = appropriateData.Rooms.FirstOrDefault(r => r.Id == int.Parse(elem.Attribute("room").Value));
                var timeSlot = new TimeSlot(int.Parse(elem.Attribute("day").Value),
                                            int.Parse(elem.Attribute("slot").Value));

                return new WeeklyEventAssignment(ev, room, timeSlot, week);
            }
            return null;
        }

        public void SaveToXml(string filename)
        {
            XElement root = new XElement("TimeTable",
                new XAttribute("time_table_id", Data.Id),
                new XAttribute("name", Name),
                from pair in assignments
                let fwa = pair.Value.FirstWeekAssignment ?? pair.Value.SecondWeekAssignment 
                let swa = pair.Value.SecondWeekAssignment ?? pair.Value.FirstWeekAssignment
                select new XElement("Event",
                                    new XAttribute("id", pair.Key.Id),
                                    new XElement("FirstWeek",
                                        new XAttribute("room", fwa.RoomId),
                                        new XAttribute("day", fwa.TimeSlot.Day),
                                        new XAttribute("slot", fwa.TimeSlot.Slot)),
                                    new XElement("SecondWeek", 
                                        new XAttribute("room", swa.RoomId),
                                        new XAttribute("day", swa.TimeSlot.Day),
                                        new XAttribute("slot", swa.TimeSlot.Slot))));
            root.Save(filename);
        }
    }
}
