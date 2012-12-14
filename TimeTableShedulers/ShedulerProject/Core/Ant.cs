﻿using System;
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
            for (int i = 0; i < _eventsCount; i++)
            {
                // chose next event from the list
                int e = _mmasData.sorted_event_list[i];

                // finding the range for normalization
                double range = 0.0;
                for (int j = 0; j < _data.TotalTimeSlots; j++)
                {
                    range += _mmasData.event_timeslot_pheromone[e, j];
                }

                // choose a random number between 0.0 and sum of the pheromone level
                // for this event and current sum of heuristic information
                double rnd = _solution.rg.NextDouble() * range;

                // choose a timeslot for the event based on the pheromone table and the random number
                double total = 0.0;
                int timeslot = -1;
                for (int j = 0; j < _totalTimeSlots; j++)
                {
                    // check the pheromone
                    total += _mmasData.event_timeslot_pheromone[e, j];
                    if (total >= rnd)
                    {
                        while (!_data.SuitableTimeSlot(_events[e].Id, j))
                            j = (j + 1) % _totalTimeSlots;
                        timeslot = j;
                        break;
                    }
                }

                // put an event i into timeslot t
                _solution.result[e].TimeSlotId = timeslot;
                _solution.timeslot_events[timeslot].Add(e);
            }

            _solution.AssignRoomForEachTimeSlot();

            return _solution;
        }
    }
}
