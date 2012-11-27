using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text;

namespace SchedulerProject.Core
{
    public class EventAssignment
    {
        public Room Room { get; set; }
        public TimeSlot TimeSlot { get; set; }
    }

    public class IdMismatchException : Exception { };

    class Circullum
    {
        public Circullum(TimeTable timeTable)
        {
            TimeTable = timeTable;
        }

        public TimeTable TimeTable { get; private set; }
        public string Name { get; set; }

        Dictionary<Event, EventAssignment> assignments = new Dictionary<Event,EventAssignment>();

        public void AddAssignment(Event e, Room r, TimeSlot slot)
        {
            assignments.Add(e, new EventAssignment() { Room = r, TimeSlot = slot });
        }

        public static Circullum LoadFromXml(TimeTable appropriateTimeTable, string filename)
        {
            var data = XDocument.Load(filename);
            var mainNode = data.Element("Circullum");

            var id = mainNode.Attribute("time_table_id").Value;
            if (id != appropriateTimeTable.Id)
            {
                throw new IdMismatchException();
            }

            var infoQuery = from e in mainNode.Elements("Event")
                            select new
                            {
                                Event = appropriateTimeTable.Events
                                            .FirstOrDefault(ev => ev.Id == int.Parse(e.Attribute("id").Value)),

                                Room = appropriateTimeTable.Rooms
                                            .FirstOrDefault(r => r.Id == int.Parse(e.Attribute("room").Value)),
                                Day = int.Parse(e.Attribute("day").Value),
                                Slot = int.Parse(e.Attribute("slot").Value)
                            };

            return new Circullum(appropriateTimeTable)
            {
                Name = mainNode.Attribute("name").Value,
                assignments = infoQuery.ToDictionary(info => info.Event, info => new EventAssignment()
                {
                    Room = info.Room,
                    TimeSlot = new TimeSlot(info.Day, info.Slot)
                })
            };
        }

        public void SaveToXml(string filename)
        {
            XElement root = new XElement("Circullum",
                new XAttribute("time_table_id", TimeTable.Id),
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
