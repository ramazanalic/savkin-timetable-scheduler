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
    public class TimeSlotsControl<TimeSlotControl> : UserControl
        where TimeSlotControl : Control
    {
        string[] dayNames = { "Понедельник", "Вторник", "Среда", "Четверг", "Пятница", "Суббота" };
        string[] slotNames = {"Первая пара", "Вторая пара", "Третья пара", "Четвертая пара", "Пятая пара"};

        public TimeSlotsControl(Func<TimeSlotControl> defaultControlConstructor = null)
        {
            TimeSlotControlSize = new Size(50, 50);
        }

        Size timeSlotControlSize;

        public Size TimeSlotControlSize
        {
            get { return timeSlotControlSize; }
            set
            {
                timeSlotControlSize = value;
                CalculateControlParams();
                //OnTimeSlotSizeChanged();
            }
        }

        protected Dictionary<TimeSlot, TimeSlotControl> timeSlotControls = new Dictionary<TimeSlot, TimeSlotControl>();

        public void AddControlToSlot(TimeSlot timeSlot, TimeSlotControl control)
        {
            if (!Controls.Contains(control))
            {
                RemoveControlFromSlot(timeSlot);
                timeSlotControls[timeSlot] = control;
                Controls.Add(control);
                SetControlLocation(timeSlot, control);
            }
            
        }

        public void RemoveControlFromSlot(TimeSlot slot)
        {
            TimeSlotControl control = GetTimeSlotControl(slot);
            if (control != null)
            {
                Controls.Remove(control);
                timeSlotControls.Remove(slot);
            }
        }

        public TimeSlotControl GetTimeSlotControl(TimeSlot slot)
        {
            TimeSlotControl control;
            if (timeSlotControls.TryGetValue(slot, out control))
            {
                return control;
            }
            return null;
        }

        void SetControlLocation(TimeSlot timeSlot, TimeSlotControl control)
        {
            var x = slotOffset + (timeSlot.Day - 1) * TimeSlotControlSize.Width;
            var y = dayOffset + (timeSlot.Slot - 1) * TimeSlotControlSize.Height;
            control.Location = new Point(x, y);
        }

        protected override void OnFontChanged(EventArgs e)
        {
            CalculateControlParams();
            base.OnFontChanged(e);
        }

        int slotOffset;
        int dayOffset;
        int minTimeSlotControlWidth; 
        int minTimeSlotControlHeight;

        const int SLOTS_COUNT = 5, DAYS_COUNT = 6;

        protected void CalculateControlParams()
        {
            using (Graphics g = CreateGraphics())
            {
                SizeF[] slotRectangles = slotNames.Select(s => g.MeasureString(s, Font)).ToArray();
                SizeF[] dayRectangles = dayNames.Select(s => g.MeasureString(s, Font)).ToArray();
                slotOffset = (int)slotRectangles.Max(r => r.Width);
                dayOffset = (int)dayRectangles.Max(r => r.Height);
                minTimeSlotControlWidth = (int)dayRectangles.Max(r => r.Width) + 4;
                minTimeSlotControlHeight = (int)slotRectangles.Max(r => r.Height) + 4;
                timeSlotControlSize = new Size(Math.Max(minTimeSlotControlWidth, timeSlotControlSize.Width),
                                               Math.Max(minTimeSlotControlHeight, timeSlotControlSize.Height));

                 Size = MaximumSize = new Size(slotOffset + DAYS_COUNT * timeSlotControlSize.Width + 1,
                                               dayOffset + SLOTS_COUNT * timeSlotControlSize.Height + 1);

                foreach (var pair in timeSlotControls)
                    SetControlLocation(pair.Key, pair.Value);
            }
        }

        //public event EventHandler TimeSlotSizeChanged;

        //public virtual void OnTimeSlotSizeChanged()
        //{
        //    if (TimeSlotSizeChanged != null)
        //        TimeSlotSizeChanged(this, new EventArgs());
        //}

        protected override void OnPaint(PaintEventArgs e)
        {
            for (var day = 0; day < DAYS_COUNT; day++)
            {                
                var x = slotOffset + day * TimeSlotControlSize.Width;
                e.Graphics.DrawString(dayNames[day], Font, new SolidBrush(ForeColor),
                    new RectangleF(x, 0, TimeSlotControlSize.Width, minTimeSlotControlHeight),
                    new StringFormat() { Alignment = StringAlignment.Center });
                for (var slot = 0; slot < SLOTS_COUNT; slot++)
                {                    
                    e.Graphics.DrawRectangle(Pens.Black, new Rectangle(new Point(x, dayOffset + slot * TimeSlotControlSize.Height),
                                                                       TimeSlotControlSize));                                                         
                }
            }

            for (var slot = 0; slot < SLOTS_COUNT; slot++)
            {
                var y = dayOffset + slot * TimeSlotControlSize.Height;
                e.Graphics.DrawString(slotNames[slot], Font, new SolidBrush(ForeColor),
                    new RectangleF(0, y, minTimeSlotControlWidth, TimeSlotControlSize.Height),
                    new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
            }

            base.OnPaint(e);
        }
    }
}
