using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text;

namespace SchedulerProject.Core
{
    public class EventAssignment //: IEquatable<EventAssignment> ??
    {
        public EventAssignment(Event e, Room r, TimeSlot slot)
        {
            Event = e;
            Room = r; 
            TimeSlot = slot;
        }
        public Event Event { get; private set; }
        public Room Room { get; private set; }
        public TimeSlot TimeSlot { get; private set; }

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

        public void AddAssignment(Event e, Room r, TimeSlot slot)
        {
            assignments.Add(e, new EventAssignment(e, r, slot));
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
                            select new
                            {
                                Event = appropriateData.Events
                                            .FirstOrDefault(ev => ev.Id == int.Parse(e.Attribute("id").Value)),

                                Room = appropriateData.Rooms
                                            .FirstOrDefault(r => r.Id == int.Parse(e.Attribute("room").Value)),
                                Day = int.Parse(e.Attribute("day").Value),
                                Slot = int.Parse(e.Attribute("slot").Value)
                            };

            return new TimeTable(appropriateData)
            {
                Name = mainNode.Attribute("name").Value,
                assignments = infoQuery.ToDictionary(info => info.Event, info => new EventAssignment(info.Event,
                    info.Room,
                    new TimeSlot(info.Day, info.Slot)
                ))
            };
        }

        public void SaveToXml(string filename)
        {
            XElement root = new XElement("TimeTable",
                new XAttribute("time_table_id", Data.Id),
                new XAttribute("name", Name),
                from pair in assignments
                select new XElement("Event",
                                    new XAttribute("id", pair.Key.Id),
                                    new XAttribute("room", pair.Value.Room.Id),
                                    new XAttribute("day", pair.Value.TimeSlot.Day),
                                    new XAttribute("slot", pair.Value.TimeSlot.Slot)));
            root.Save(filename);
        }
    }
}
