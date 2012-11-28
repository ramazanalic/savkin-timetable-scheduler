using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using SchedulerProject.Core;

namespace SchedulerProject.UserInterface
{
    public class EventAssignmentControl : TimeSlotControl<EventAssignmentControl>
    {
        public EventAssignmentControl(TimeTableData data, EventAssignment assignment)
        {
            _data = data;
            SelectedEventAssignment = assignment;
            BackColor = Color.LightBlue;
        }

        public event PropertyChangedEventHandler<Event, EventAssignmentControl> EventChanged;
        public event PropertyChangedEventHandler<Room, EventAssignmentControl> RoomChanged;

        TimeTableData _data;
        Subject _subject;
        Lecturer _lecturer;
        Event _event;
        Room _room;

        public Event Event
        {
            get
            {
                return _event;
            }
            set
            {
                var oldEvent = _event;
                _event = value;
                OnEventChanged(oldEvent, value);
            }
        }

        public Room Room
        {
            get
            {
                return _room;
            }
            set
            {
                var oldRoom = _room;
                _room = value;
                OnRoomChanged(oldRoom, value);
            }
        }

        public EventAssignment SelectedEventAssignment
        {
            get
            {
                return new EventAssignment(Event, Room, TimeSlot);
            }
            set
            {
                TimeSlot = value.TimeSlot;
                Event = value.Event;
                Room = value.Room;
            }
        }
      
        protected virtual void OnRoomChanged(Room oldValue, Room newValue)
        {
            if (RoomChanged != null)
                RoomChanged(this, oldValue, newValue);
            Refresh();
        }

        protected virtual void OnEventChanged(Event oldValue, Event newValue)
        {
            _subject = _data.Subjects.First(s => s.Id == newValue.SubjectId);
            _lecturer = _data.Lecturers.First(l => l.Id == newValue.LecturerId);
            if (EventChanged != null)
                EventChanged(this, oldValue, newValue);
            Refresh();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            using (var brush = new SolidBrush(ForeColor))
            {
                e.Graphics.DrawString(_subject + "\n" + _lecturer+ "\n" + _room,
                                      Font, brush, 
                                      new Rectangle(Point.Empty, Size),
                                      new StringFormat()
                                      {
                                          Alignment = StringAlignment.Center,
                                          LineAlignment = StringAlignment.Center
                                      });
            }
            base.OnPaint(e);
        }
    }

    // for editing time table TimeTableView should be extended to EditableTimeTableView f.e.
    public class TimeTableView : TimeSlotsGrid<EventAssignmentControl> 
    {
        static int DAYS_COUNT = 6, SLOTS_COUNT = 5;

        TimeTable timeTable;
        public TimeTable TimeTable
        {
            get { return timeTable; }
            set
            {
                timeTable = value;
                FillSlots();
            }
        }

        public TimeTableView(Size slotSize)
            : base(DAYS_COUNT, SLOTS_COUNT)
        {
            TimeSlotControlSize = slotSize;
        }

        Predicate<Event> eventsFilter;
        public Predicate<Event> EventsFilter
        {
            get { return eventsFilter; }
            set
            {
                eventsFilter = value;
                FillSlots();
            }
        }

        protected void FillSlots()
        {
            SuspendLayout();
            ClearAllTimeSlots();
            if (eventsFilter != null && timeTable != null)
            {
                foreach (var assignment in timeTable.Assignments.Where(a => eventsFilter(a.Event)))
                {
                    AddControlToSlot(new EventAssignmentControl(timeTable.Data, assignment)
                    {
                        Size = TimeSlotControlSize 
                    });
                }
            }
            ResumeLayout();
        }
    }
}
