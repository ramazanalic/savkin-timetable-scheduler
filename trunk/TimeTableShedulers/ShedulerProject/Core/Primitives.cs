namespace ShedulerProject.Core
{
    public abstract class AbstractPrimitive
    {
        public int Id = -1;

        protected abstract string StringRepresentation();

        public override string ToString()
        {
            return StringRepresentation();
        }

        public virtual bool IsEmpty { get { return Id == -1; } }
    }

    public interface IConstrainedPrimitive
    {
        TimeConstraints TimeConstraints { get; }
    }

    public enum RoomType
    {
        Special,
        Laboratory,
        Lecture,
        Practice
    }

    public class Subject : AbstractPrimitive
    {
        public string Name;
        public int Course;
        public int Difficulty;
        public int? LecturerId;

        protected override string StringRepresentation()
        {
            return Name;
        }
    }

    public class Lecturer : AbstractPrimitive, IConstrainedPrimitive
    {
        public string Name;
        public TimeConstraints timeConstraints;
        public TimeConstraints TimeConstraints { get { return timeConstraints; } }

        protected override string StringRepresentation()
        {
            return Name;
        }
    }

    public class Group : AbstractPrimitive
    {
        public int Course;
        public string Name;

        protected override string StringRepresentation()
        {
            return Name;
        }
    }

    public class TimeConstraints
    {
        public int[] HardTimeSlotsConstraints;
        public int[] SoftTimeSlotsConstraints;
    }

    public class Room : AbstractPrimitive, IConstrainedPrimitive
    {
        public int Housing;
        public string RoomNumber;
        public RoomType Type;
        public TimeConstraints timeConstraints;
        public TimeConstraints TimeConstraints { get { return timeConstraints; } }

        protected override string StringRepresentation()
        {
            return string.Format("{0} - {1} ({2})", RoomNumber, Housing, Type);
        }
    }

    public class Event : AbstractPrimitive
    {
        public int LecturerId;
        public int SubjectId;
        public int[] Groups;
        public RoomType RoomType;

        protected override string StringRepresentation()
        {
            return string.Format("{0} - {1} ({2})", SubjectId, LecturerId, RoomType);
        }
        
        // should be set by the sheduler
        public int AssignedTimeSlotId;
        public int AssignedRoomId;
    }
}