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
    public class SingleTimeSlotConstraintsController : Control
    {
        TimeConstrainsType?[] availableConstraints = new TimeConstrainsType?[]{ null,
                                                                                TimeConstrainsType.Desirible, 
                                                                                TimeConstrainsType.Undesirible, 
                                                                                TimeConstrainsType.Impossible, 
                                                                                TimeConstrainsType.Necessary};

        Color[] constrainsColors = new Color[] { Color.White, Color.LightGreen, Color.Orange, Color.Red, Color.DarkBlue };

        int currIndex = 0;

        public TimeConstrainsType? SelectedConstraint
        {
            get { return availableConstraints[currIndex]; }
            set
            {
                currIndex = Array.IndexOf(availableConstraints, value);
                if (currIndex == -1) currIndex = 0;
            }
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                currIndex = (currIndex + 1) % availableConstraints.Length;
            else if (e.Button == MouseButtons.Right)
            {
                if (currIndex == 0)
                    currIndex = availableConstraints.Length - 1;
                else
                    currIndex--;
            }
            BackColor = constrainsColors[currIndex];
            base.OnMouseClick(e);
        }
    }

    public class TimeSlotsConstraintsController : TimeSlotsControl<SingleTimeSlotConstraintsController>
    {
        int DAYS_COUNT = 6, SLOTS_COUNT = 5;
        public TimeSlotsConstraintsController()
        {
            foreach (var slot in TimeSlot.EnumerateAll(DAYS_COUNT, SLOTS_COUNT))
                AddControlToSlot(slot, new SingleTimeSlotConstraintsController());
        }

        public TimeConstraints SelectedConstraints
        {
            get { return CollectTimeConstaints(); }
            set { FillTimeConstraints(value); }
        }

        TimeConstraints CollectTimeConstaints()
        {
            return new TimeConstraints();
        }

        void FillTimeConstraints(TimeConstraints constraints)
        {
            foreach (var pair in timeSlotControls)
            {
                RemoveControlFromSlot(pair.Key);
            }

            foreach (var constraintsSet in constraints.EnumerateConstraintsSets())
                foreach(var timeSlot in constraintsSet)
                {
                    GetTimeSlotControl(timeSlot).SelectedConstraint = constraintsSet.Type;
                }
        }

    }
}
