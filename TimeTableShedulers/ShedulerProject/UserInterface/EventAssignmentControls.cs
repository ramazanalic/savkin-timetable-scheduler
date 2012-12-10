﻿using System;
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
        public EventAssignmentControl(TimeTableData data, 
                                      WeeklyEventAssignment firstWeekAssignment,
                                      WeeklyEventAssignment secondWeekAssignment)
        {
            _data = data;
            SelectedFirstWeekAssignment = firstWeekAssignment;
            SelectedSecondWeekAssignment = secondWeekAssignment;
            BackColor = Color.LightBlue;
            TimeSlot = (firstWeekAssignment ?? secondWeekAssignment).TimeSlot;
        }

        TimeTableData _data;
        
        public WeeklyEventAssignment SelectedFirstWeekAssignment
        {
            get;
            set;
        }

        public WeeklyEventAssignment SelectedSecondWeekAssignment
        {
            get;
            set;
        }

        void DrawAssignment(WeeklyEventAssignment assignment, Graphics g, Rectangle rect)
        {
            using (var brush = new SolidBrush(ForeColor))
            {
                var subject = _data.Subjects.First(s => s.Id == assignment.Event.SubjectId);
                var lecturer = _data.Lecturers.First(l => l.Id == assignment.Event.LecturerId);
                var room = assignment.Room;
                g.DrawString(subject + "\n" + lecturer + "\n" + room,
                            Font, brush,
                            rect,
                            new StringFormat()
                            {
                                Alignment = StringAlignment.Center,
                                LineAlignment = StringAlignment.Center
                            });
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            //TODO: join areas if needed
            if (SelectedFirstWeekAssignment != null)
                DrawAssignment(SelectedFirstWeekAssignment, e.Graphics, new Rectangle(0, 0, Width, Height / 2));
            e.Graphics.DrawLine(Pens.White, 0, Height / 2, Width, Height / 2);
            if (SelectedSecondWeekAssignment != null)
                DrawAssignment(SelectedSecondWeekAssignment, e.Graphics, new Rectangle(0, Height / 2, Width, Height / 2));
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
                foreach (var assignment in timeTable.Assignments
                    .Where(a => eventsFilter(a.Event))
                    .SelectMany(a => new[] { a.FirstWeekAssignment, a.SecondWeekAssignment })
                    .Where(wa => wa != null)
                    .GroupBy(wa => wa.TimeSlot)
                    .Select(wa => new
                    {
                        First = wa.FirstOrDefault(w => w.Week == 1),
                        Second = wa.FirstOrDefault(w => w.Week == 2)
                    }))
                {
                    AddControlToSlot(new EventAssignmentControl(timeTable.Data, assignment.First, assignment.Second)
                    {
                        Size = TimeSlotControlSize
                    });
                }
            }
            ResumeLayout();
        }
    }
}
