﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SchedulerProject.Core
{
    class MMASData
    {
        public double evap;
        public double phe_max;
        public double phe_min;
        //public double alpha;
        public double[,] event_timeslot_pheromone;	// matrix keeping pheromone between events and timeslots
        public int[] sorted_event_list;			// vector keeping sorted lists of events

        TimeTableData _data;
        Event[] _events;
        int _eventesNumber;
        int _timeslotsNumber;

        public MMASData(TimeTableData data, int week, double evaporation, double minPher)
        {
            //member variables initialization
            _data = data;
            _events = _data.GetWeekEvents(week);
            _eventesNumber = _events.Length;
            _timeslotsNumber = data.TotalTimeSlots;
            event_timeslot_pheromone = new double[_eventesNumber, _timeslotsNumber];
            evap = evaporation;
            phe_min = minPher;

            if (evap < 1.0)
            {
                phe_max = 10.0 / (1.0 - evap);
            }
            else
            {
                phe_max = int.MaxValue;
            }

            // creating a set of pre-sorted event lists
            // sorting events based on level of correlations
            int[] event_correlation = new int[_eventesNumber];
            for (int i = 0; i < _eventesNumber; i++)
            {
                // summing up the correlations for each event
                event_correlation[i] = 0;
                for (int j = 0; j < _eventesNumber; j++)
                {
                    if (data.ConflictingEvents(_events[i].Id, _events[j].Id))
                        event_correlation[i] += 1;
                }
            }

            sorted_event_list = new int[_eventesNumber];
            for (int i = 0; i < _eventesNumber; i++)
            {
                // sorting the list
                int max_correlation = -1;
                int event_index = -1;
                for (int j = 0; j < _eventesNumber; j++)
                {
                    if (event_correlation[j] > max_correlation)
                    {
                        max_correlation = event_correlation[j];
                        event_index = j;
                    }
                }

                event_correlation[event_index] = -2;
                sorted_event_list[i] = event_index;
            }
        }

        public void SetPheromoneFromExistingAssignments(WeeklyEventAssignment[] assignments)
        {
            SetPheromone(phe_min);
            foreach (var assignment in assignments)
            {
                var eventIndex = Array.IndexOf(_events, assignment.Event);
                if (eventIndex != -1)
                {
                    var slotId = TimeSlot.ToId(assignment.TimeSlot, _data.Days, _data.SlotsPerDay);
                    event_timeslot_pheromone[eventIndex, slotId] = phe_max;
                }
            }
        }

        public void ResetPheromone()
        {
            // initialize pheromon levels between events and timeslots to the maximal values
            SetPheromone(phe_max);
        }

        void SetPheromone(double value)
        {
            for (int i = 0; i < _eventesNumber; i++)
            {
                for (int j = 0; j < _timeslotsNumber; j++)
                {
                    event_timeslot_pheromone[i, j] = value;
                }
            }
        }

        public void EvaporatePheromone()
        {
            // evaporate some pheromone
            for (int i = 0; i < _eventesNumber; i++)
            {
                for (int j = 0; j < _timeslotsNumber; j++)
                {
                    event_timeslot_pheromone[i, j] *= (1 - evap);
                }
            }
        }

        public void SetPheromoneLimits()
        {
            // limit pheromone values according to MAX-MIN
            for (int i = 0; i < _eventesNumber; i++)
            {
                for (int j = 0; j < _timeslotsNumber; j++)
                {
                    if (event_timeslot_pheromone[i, j] < phe_min)
                    {
                        event_timeslot_pheromone[i, j] = phe_min;
                    }

                    if (event_timeslot_pheromone[i, j] > phe_max)
                    {
                        event_timeslot_pheromone[i, j] = phe_max;
                    }
                }
            }
        }

        public void DepositPheromone(Solution solution)
        {
            // calculate pheromone update
            for (int i = 0; i < solution.result.Length; i++)
            {
                int timeslot = solution.result[i].TimeSlotId;
                event_timeslot_pheromone[i, timeslot] += 1.0;
            }
        }
    }
}
