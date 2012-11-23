using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SchedulerProject.UserInterface
{
    public class TimeSlotsControl<TimeSlotControl> : UserControl
        where TimeSlotControl : Control
    {
        string[] dayNames = { "Понедельник", "Вторник", "Среда", "Четверг", "Пятница", "Суббота" };
        string[] slotNames = {"Первая пара", "Вторая пара", "Третья пара", "Четвертая пара", "Пятая пара"};

        public TimeSlotsControl(Func<TimeSlotControl> defaultControlConstructor = null)
        {
            TimeSlotControlSize = new Size(10, 10);
            Size = new Size(300, 300);
        }

        public Size TimeSlotControlSize
        {
            get;
            set;
        }        

        public void AddControlToSlot(TimeSlotControl control)
        {
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            SizeF[] slotRectangles = slotNames.Select(s => e.Graphics.MeasureString(s, Font)).ToArray();
            SizeF[] dayRectangles = dayNames.Select(s => e.Graphics.MeasureString(s, Font)).ToArray();
            int slotOffset = (int)slotRectangles.Max(r => r.Width);
            int dayOffset = (int)dayRectangles.Max(r => r.Height);
            int minTimeSlotControlWidth = (int)dayRectangles.Max(r => r.Width);
            int minTimeSlotControlHeight = (int)slotRectangles.Max(r => r.Height);
            TimeSlotControlSize = new Size(Math.Max(minTimeSlotControlWidth, TimeSlotControlSize.Width),
                                           Math.Max(minTimeSlotControlHeight, TimeSlotControlSize.Height));

            for (int day = 0; day < 6; day++)
            {
                for (int slot = 0; slot < 5; slot++)
                {
                    e.Graphics.DrawRectangle(Pens.Black, new Rectangle(new Point(slotOffset + day * TimeSlotControlSize.Width,
                                                                                 dayOffset + slot * TimeSlotControlSize.Height),
                                                                       TimeSlotControlSize));                                                         
                }
            }

            base.OnPaint(e);
        }
    }
}
