using System;
using System.Linq;
using System.Collections.Generic;

namespace SchedulerProject.Core
{
    public class TimeSlot : IComparable<TimeSlot>, IEquatable<TimeSlot>
    {
        public static IEnumerable<TimeSlot> EnumerateAll(int days, int slots)
        {
            for (var day = 0; day < days; day++)
            {
                for (var slot = 0; slot < slots; slot++)
                {
                    yield return new TimeSlot(day, slot);
                }
            }
        }

        public TimeSlot(int day, int slot)
        {
            Day = day;
            Slot = slot;
        }

        // 1..
        public int Day, Slot;

        public override string ToString()
        {
            return string.Format("({0} - {1})", Day, Slot);
        }

        public static TimeSlot Parse(string str)
        {
            var tmp = str.Substring(1, str.Length - 2).Split('-').Select(s => int.Parse(s.Trim())).ToArray();
            return new TimeSlot(tmp[0], tmp[1]);
        }

        public int CompareTo(TimeSlot other)
        {
            int d = Day.CompareTo(other.Day);
            return d == 0 ? Slot.CompareTo(other.Slot) : d;
        }

        public override int GetHashCode()
        {
            return Day * 100 + Slot;
        }

        public bool Equals(TimeSlot other)
        {
            return CompareTo(other) == 0;
        }
    }

    public enum TimeConstrainsType
    {
        Desirible,  //soft constraint
        Undesirible,//soft constraint
        Necessary,  //hard constraint
        Impossible, //hard constraint
    }

    public class TimeConstraintsSet : IEnumerable<TimeSlot>
    {
        public TimeConstraintsSet(TimeConstrainsType type)
        {
            Type = type;
        }

        public TimeConstrainsType Type { get; set; }

        List<TimeSlot> constraints = new List<TimeSlot>();

        public void AddConstraint(int day, int slot)
        {
            constraints.Add(new TimeSlot(day, slot));
        }

         IEnumerator<TimeSlot> IEnumerable<TimeSlot>.GetEnumerator()
        {
            return constraints.GetEnumerator();
        }

          System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
         {
             return constraints.GetEnumerator();
         }

        public override string ToString()
        {
            return Type.ToString() + " [" + string.Join(",", constraints) + "]";
        }

        public static TimeConstraintsSet Parse(string str)
        {
            int typeLength = str.IndexOf(' ');
            string dataStr = str.Substring(typeLength).Trim();
            TimeConstrainsType type = ParseHelper.ParseEnum<TimeConstrainsType>(str.Substring(0, typeLength));
            return new TimeConstraintsSet(type)
            {
                constraints = str.Substring(1, str.Length - 2)
                                 .Split(',')
                                 .Select(s => TimeSlot.Parse(s.Trim()))
                                 .ToList()
            };
        }
    }

    public class TimeConstraints
    {
        // TODO: if there are NecessaryTimeSlots, do not take into account other constraints

        //public TimeConstraintsSet DesiribleTimeSlots { get; private set; }
        //public TimeConstraintsSet UndesiribleTimeSlots { get; private set; }
        //public TimeConstraintsSet ImpossibleTimeSlots { get; private set; }
        //public TimeConstraintsSet NecessaryTimeSlots { get; private set; }

        public TimeConstraintsSet DesiribleTimeSlots = new TimeConstraintsSet(TimeConstrainsType.Desirible);
        public TimeConstraintsSet UndesiribleTimeSlots = new TimeConstraintsSet(TimeConstrainsType.Undesirible);
        public TimeConstraintsSet ImpossibleTimeSlots = new TimeConstraintsSet(TimeConstrainsType.Impossible);
        public TimeConstraintsSet NecessaryTimeSlots = new TimeConstraintsSet(TimeConstrainsType.Necessary);

        public override string ToString()
        {
            return string.Format("[\n{0};\n{1};\n{2};\n{3};\n]", 
                                 DesiribleTimeSlots, UndesiribleTimeSlots, ImpossibleTimeSlots, NecessaryTimeSlots);
        }

        public static TimeConstraints Parse(string str)
        {
            var tmp = str.Substring(1, str.Length - 2)
                         .Split(';')
                         .Select(s => TimeConstraintsSet.Parse(s.Trim()))
                         .ToArray();

            return new TimeConstraints()
            {
                DesiribleTimeSlots = tmp[0],
                UndesiribleTimeSlots = tmp[1],
                ImpossibleTimeSlots = tmp[2],
                NecessaryTimeSlots = tmp[3]
            };
        }

        public IEnumerable<TimeConstraintsSet> EnumerateConstraintsSets()
        {
            if (DesiribleTimeSlots != null)
                yield return DesiribleTimeSlots;
            if (UndesiribleTimeSlots != null)
                yield return UndesiribleTimeSlots;
            if (ImpossibleTimeSlots != null)
                yield return ImpossibleTimeSlots;
            if (NecessaryTimeSlots != null)
                yield return NecessaryTimeSlots;
        }
    }
}