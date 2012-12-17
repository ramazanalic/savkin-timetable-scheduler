using System;
using System.Collections.Generic;
using System.Text;

namespace SchedulerProject.Core
{
    class Ant
    {
        public Ant(TimeTableData problemData, MMASData mmasData, int week)
        {
            // memeber variables initialization
            _data = problemData;
            _solution = new Solution(problemData, week);
            _events = _data.GetWeekEvents(week);
            _eventsCount = _events.Length;
            _mmasData = mmasData;
            _totalTimeSlots = _data.TotalTimeSlots;
        }

        Solution _solution;
        TimeTableData _data;
        MMASData _mmasData;
        Event[] _events;
        int _totalTimeSlots;
        int _eventsCount;

        public Solution GetSolution()
        {
            // itarate through all the events to complete the path
            for (var i = 0; i < _eventsCount; i++)
            {
                // chose next event from the list
                var e = _mmasData.sorted_event_list[i];
                var timeslot = GetTimeSlotId(e);
                // put an event i into timeslot t
                _solution.result[e].TimeSlotId = timeslot;
                _solution.timeslot_events[timeslot].Add(e);
            }

            _solution.AssignRoomForEachTimeSlot();

            return _solution;
        }

        public Solution GetSolution(WeeklyEventAssignment[] existingAssignments)
        {
            foreach (var assignment in existingAssignments)
            {
                var eventIndex = Array.IndexOf(_events, assignment.Event);
                if (eventIndex != -1)
                {
                    var slotId = TimeSlot.ToId(assignment.TimeSlot, _data.Days, _data.SlotsPerDay);
                    _solution.result[eventIndex].TimeSlotId = slotId;
                    _solution.result[eventIndex].RoomId = assignment.Room.Id;
                    _solution.timeslot_events[slotId].Add(eventIndex);
                }
            }

            // itarate through all the events to complete the path
            for (var i = 0; i < _eventsCount; i++)
            {
                // chose next event from the list
                var e = _mmasData.sorted_event_list[i];
                if (_solution.result[e].TimeSlotId == -1)
                {
                    var timeslot = GetTimeSlotId(e);
                    _solution.result[e].TimeSlotId = timeslot;
                    _solution.timeslot_events[timeslot].Add(e);
                }
            }

            return _solution;
        }

        // finding the range for normalization
        double GetNormalizationRange(int eventIndex)
        {
            double range = 0.0;
            for (int j = 0; j < _data.TotalTimeSlots; j++)
                range += _mmasData.event_timeslot_pheromone[eventIndex, j];
            return range;
        }

        // choose a timeslot for the event based on the pheromone table and the random number
        int GetTimeSlotId(int eventIndex)
        {
            // choose a random number between 0.0 and sum of the pheromone level
            // for this event and current sum of heuristic information
            var limit = _solution.rg.NextDouble() * GetNormalizationRange(eventIndex);

            double total = 0.0;
            int timeslot = -1;
            for (int j = 0; j < _totalTimeSlots; j++)
            {
                // check the pheromone
                total += _mmasData.event_timeslot_pheromone[eventIndex, j];
                if (total >= limit)
                {
                    while (!_data.SuitableTimeSlot(_events[eventIndex].Id, j))
                        j = (j + 1) % _totalTimeSlots;
                    timeslot = j;
                    break;
                }
            }
            return timeslot;
        }
    }
}
