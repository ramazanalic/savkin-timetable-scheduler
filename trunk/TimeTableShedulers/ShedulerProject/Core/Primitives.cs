namespace ShedulerProject.Core
{
    public enum RoomType
    {
        Special,
        Laboratory,
        Lecture,
        Practice
    }

    public class Subject
    {
        public int Id;
        public string Name;
        public int Course;
        public int Difficulty;
        public int? LecturerId;
    }

    public class Lecturer
    {
        public int Id;
        public string Name;
        public TimeConstraits timeConstraits;
    }

    public class Group
    {
        public int Id;
        public int Course;
        public string Name;
    }

    public class TimeConstraits
    {
        public int[] HardTimeSlotsConstraits;
        public int[] SoftTimeSlotsConstraits;
    }

    public class Room
    {
        public int Id;
        public int Housing;
        public string RoomNumber;
        public RoomType Type;
        public TimeConstraits timeConstraits;
    }

    public class Event
    {
        public int Id;
        public int LecturerId;
        public int SubjectId;
        public int[] Groups;
        public RoomType RoomType;
        
        // should be set by the sheduler
        public int AssignedTimeSlotId;
        public int AssignedRoomId;
    }
}