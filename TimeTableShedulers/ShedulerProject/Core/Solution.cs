using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace SchedulerProject.Core
{   
    struct InternalEventAssignment
    {
        public int TimeSlotId;
        public int RoomId;
    }

    class Solution : IComparable<Solution>
    {
        public InternalEventAssignment[] result; // vector of (timeslot, room) assigned for each event (index is an event number)
        public Dictionary<int, List<int>> timeslot_events = new Dictionary<int, List<int>>(); // for each timeslot a vector of events taking place in it
        public TimeTableData data; // a pointer to the problem data
        public Random rg = new Random();

        public bool feasible;
        public int scv;   // keeps the number of soft constraint violations (ComputeScv() has to be called)
        public int hcv;  // keeps the number of hard constraint violations (computeHcv() has to be called)

        // CHECK CORRECTNESS AND PERFORMANCE !!!
        // 
        public void CopyTo(Solution dest)
        {
            dest.data = this.data;
            dest.feasible = this.feasible;
            dest.scv = this.scv;
            dest.hcv = this.hcv;
            dest.result = this.result.AsEnumerable().ToArray();
            dest.timeslot_events = this.timeslot_events
                                    .AsEnumerable()
                                    .ToDictionary(kvp => kvp.Key,
                                                kvp => kvp.Value.AsEnumerable().ToList());
        }

        public int CompareTo(Solution other)
        {
            this.computeHcv();
            other.computeHcv();
            return this.hcv.CompareTo(other.hcv);
        }

        private Solution() { }

        /// <summary>
        /// Constructor with pointers to the problem data and to the random object.
        /// </summary>
        public Solution(TimeTableData problemData)
        {
            data = problemData;
            rg = new Random();
            result = new InternalEventAssignment[data.Events.Length];
            for (int t = 0; t < data.TotalTimeSlots; t++)
                timeslot_events.Add(t, new List<int>());
            for (int i = 0; i < result.Length; i++)
            {
                result[i].RoomId = result[i].TimeSlotId = -1;
            }
        }

        /// <summary>
        /// Produce a random initial solution.
        /// </summary>
        public void RandomInitialSolution()
        {
            // assign a random timeslot to each event
            for (int i = 0; i < data.Events.Length; i++)
            {
                int t = (int)(rg.NextDouble() * data.TotalTimeSlots);
                result[i].TimeSlotId = t;
                timeslot_events[t].Add(i);
            }

            var notEmptySlots = from p in timeslot_events
                                where p.Value.Count != 0
                                select p.Key;

            // and assign rooms to events in each non-empty timeslot
            foreach (int timeSlot in notEmptySlots)
            {
                assignRooms(timeSlot);
            }
        }

        public bool computeFeasibility()
        {
            feasible = getHcv(true) == 0;
            return feasible;
        }

        /// <summary>
        /// Check feasibility.
        /// </summary>
        int getHcv(bool stopOnFirst)
        {
            //Console.WriteLine(" === getHcv === ");
            int hcv = 0;
            for (int i = 0; i < data.Events.Length; i++)
            {
                for (int j = i + 1; j < data.Events.Length; j++)
                {
                    if (result[i].TimeSlotId == result[j].TimeSlotId &&
                        result[i].RoomId == result[j].RoomId)
                    {
                        if (stopOnFirst) return 1;
                        //Console.WriteLine("Room conflict");
                        hcv++; // only one class can be in each room at any timeslot
                    }

                    if (data.eventConflicts[i, j] &&
                        result[i].TimeSlotId == result[j].TimeSlotId)
                    {
                        if (stopOnFirst) return 1;
                        //Console.WriteLine("Event conflict");
                        hcv++; // two events sharing groups cannot be in the same timeslot
                    }
                }

                if (data.Rooms[result[i].RoomId].Type != data.Events[i].RoomType)
                {
                    if (stopOnFirst) return 1;
                    //Console.WriteLine("Room type conflict");
                    hcv++; // each event should take place in a suitable room
                }
                // TODO: check lecturer constraits
            }

            // if none of the previous hard constraint violations occurs the timetable is feasible
            return hcv;
        }

        /// <summary>
        /// Compute soft constraint violations.
        /// </summary>
        public int computeScv()
        {
            // TODO: add analysis of the required soft constraits
            return 0;
        }

        /// <summary>
        /// Compute hard constraint violations.
        /// </summary>
        public int computeHcv()
        {
            hcv = getHcv(false);
            return hcv;
        }

        /// <summary>
        /// Evaluate number of hcv caused by event e.
        /// </summary>
        int eventHcv(int e)
        {
            int eHcv = 0;			// set to zero hard constraint violations for event e
            int t = result[e].TimeSlotId;	// note the timeslot in which event e is
            for (int i = 0; i < timeslot_events[t].Count; i++)
            {
                if (timeslot_events[t][i] != e)
                {
                    if (result[e].RoomId == result[timeslot_events[t][i]].RoomId)
                    {
                        // adds up number of events sharing room and timeslot with the given one
                        eHcv = eHcv + 1;
                    }

                    if (data.eventConflicts[e, timeslot_events[t][i]])
                    {
                        // adds up number of incompatible( because of students in common) events in the same timeslot                    
                        eHcv = eHcv + 1;
                    }
                }
            }

            // the suitable room hard constraint is taken care of by the assignroom routine
            return eHcv;
        }

        /// <summary>
        /// Evaluate the hcv that might be affected when event e is moved from its timeslot.
        /// </summary>
        int eventAffectedHcv(int e)
        {
            int aHcv = 0;					// set to zero the affected hard constraint violations for event e
            int t = result[e].TimeSlotId;			// t timeslot where event e is
            for (int i = 0; i < timeslot_events[t].Count; i++)
            {
                for (int j = i + 1; j < timeslot_events[t].Count; j++)
                {
                    if (result[timeslot_events[t][i]].RoomId == result[timeslot_events[t][j]].RoomId)
                    {
                        // adds up number of room clashes in the timeslot of the given event
                        // (rooms assignement are affected by move for the whole timeslot)
                        aHcv = aHcv + 1;
                    }
                }

                if (timeslot_events[t][i] != e)
                {
                    if (data.eventConflicts[e, timeslot_events[t][i]])
                    {
                        // adds up number of incompatible (because of students in common) events in the same timeslot
                        // the only hcv of this type affected when e is moved are the ones involving e
                        aHcv = aHcv + 1;
                    }
                }
            }

            // the suitable room hard constraint is taken care of by the assignroom routine
            return aHcv;
        }

        /// <summary>
        /// Evaluate all the room hcv as above for all the events in timeslot t.
        /// </summary>
        int affectedRoomInTimeslotHcv(int t)
        {
            int roomHcv = 0;
            for (int i = 0; i < timeslot_events[t].Count; i++)
            {
                for (int j = i + 1; j < timeslot_events[t].Count; j++)
                {
                    if (result[timeslot_events[t][i]].RoomId ==
                        result[timeslot_events[t][j]].RoomId)
                    {
                        roomHcv += 1;
                    }
                }
            }

            return roomHcv;
        }

        /// <summary>
        /// Evaluate number of scv caused by event e
        /// </summary>
        int eventScv(int e)
        {
            // TODO: evaluate scv
            return 0;
        }

        /// <summary>
        /// Evaluate the number of single classes that event e actually solves in the day (or created if e leaves its timeslot).
        /// NOTE: Should be called only if the solution is feasible.
        /// </summary>
        int singleClassesScv(int e)
        {
            int t = result[e].TimeSlotId;
            int classes;
            int singleClasses = 0;
            for (int i = 0; i < data.Groups.Length; i++)
            {
                if (data.groupEvents[i, e])
                {
                    classes = 0;

                    //check 8 nearest timeslosts?
                    for (int s = t - (t % 9); s < t - (t % 9) + 9; s++)
                    {
                        if (classes > 1)
                        {
                            break;
                        }

                        if (s != t)
                        {	
                            // we are in the feasible region so there are not events sharing students in the same timeslot
                            for (int j = 0; j < timeslot_events[s].Count; j++)
                            {
                                if (data.groupEvents[i, timeslot_events[s][j]])
                                {
                                    classes += 1;
                                    break;
                                }
                            }
                        }
                    }

                    // classes = 0 means that the group under consideration has a single class in the day (for event e) but that W
                    // but we are not interested in that here (it is counted in eventScv(e))
                    if (classes == 1)
                    {
                        singleClasses += 1;
                    }
                }
            }

            return singleClasses;
        }

        /// <summary>
        /// Move event e to timeslot t (type 1 move).
        /// </summary>
        void Move1(int e, int t)
        {
	        //move event e to timeslot t
	        int prevSlot = result[e].TimeSlotId;
	        result[e].TimeSlotId = t;

            timeslot_events[prevSlot].Remove(e); // remove event e from the original timeslot 
	        timeslot_events[t].Add(e);	      // and place it in timeslot t

	        // reorder in label order events in timeslot t
            timeslot_events[t].Sort(); // TODO: try to replace insertion and sort with Binary search

	        // reassign rooms to events in timeslot t
	        assignRooms(t);

	        // do the same for the original timeslot of event e if it is not empty
	        if (timeslot_events[prevSlot].Count != 0)
	        {
		        assignRooms(prevSlot);
	        }
        }

        /// <summary>
        /// Swaps events e1 and e2 (type 2 move).
        /// </summary>
        void Move2(int e1, int e2)
        {
	        //swap timeslots between event e1 and event e2
            int t1 = result[e1].TimeSlotId,
                t2 = result[e2].TimeSlotId;

	        result[e1].TimeSlotId = t2;
	        result[e2].TimeSlotId = t1;

            ReplaceEvent(t1, e1, e2);
            ReplaceEvent(t2, e2, e1);

	        assignRooms(result[e1].TimeSlotId);
	        assignRooms(result[e2].TimeSlotId);
        }

        /// <summary>
        /// 3-cycle permutation of events e1, e2 and e3 (type 3 move).
        /// </summary>
        void Move3(int e1, int e2, int e3)
        {
	        // permute event e1, e2, and e3 in a 3-cycle
            
            int t1 = result[e1].TimeSlotId,
                t2 = result[e2].TimeSlotId,
                t3 = result[e3].TimeSlotId;

	        result[e1].TimeSlotId = t2;
	        result[e2].TimeSlotId = t3;
	        result[e3].TimeSlotId = t1;

            ReplaceEvent(t1, e1, e2);
            ReplaceEvent(t2, e2, e3);
            ReplaceEvent(t3, e3, e1);

	        assignRooms(result[e1].TimeSlotId);
	        assignRooms(result[e2].TimeSlotId);
	        assignRooms(result[e3].TimeSlotId);
        }

        void ReplaceEvent(int timeSlot, int currEvent, int newEvent)
        {
            timeslot_events[timeSlot].Remove(currEvent);
            timeslot_events[timeSlot].Add(newEvent);
            timeslot_events[timeSlot].Sort();
        }

        /// <summary>
        /// Apply local search with the given parameters.
        /// </summary>
        public void localSearch(int maxSteps, double LS_limit = 999999,
                                double prob1 = 1.0, double prob2 = 0.5, double prob3 = 0.1)
        {
            // TODO: Refactoring needed

            // perform local search with given time limit and probabilities for each type of move
            int[] eventList = new int[data.Events.Length]; // keep a list of events to go through
            for (int i = 0; i < data.Events.Length; i++)
            {
                eventList[i] = i;
            }

            for (int i = 0; i < data.Events.Length; i++)
            {	// scramble the list of events to obtain a random order
                int j = (int)(rg.NextDouble() * data.Events.Length);
                int h = eventList[i];
                eventList[i] = eventList[j];
                eventList[j] = h;
            }

            /*cout <<"event list" <<endl;
          for(int i = 0 ; i< data->n_of_events; i++)
            cout<< eventList[i] << " ";
            cout << endl;*/
            int neighbourAffectedHcv = 0;	// partial evaluation of neighbour solution hcv
            int neighbourScv = 0;			// partial evaluation of neighbour solution scv
            int evCount = 0;				// counter of events considered
            int stepCount = 0;				// set step counter to zero
            bool foundbetter = false;
            computeFeasibility();
            if (!feasible)
            {						// if the timetable is not feasible try to solve hcv
                for (int i = 0; evCount < data.Events.Length; i = (i + 1) % data.Events.Length)
                {
                    if (stepCount > maxSteps)
                    {
                        break;
                    }

                    int currentHcv = eventHcv(eventList[i]);
                    if (currentHcv == 0)
                    {				// if the event on the list does not cause any hcv
                        evCount++;	// increase the counter
                        continue;	// go to the next event
                    }

                    // otherwise if the event in consideration caused hcv
                    int currentAffectedHcv;
                    int t_start = (int)(rg.NextDouble() * data.TotalTimeSlots);	// try moves of type 1
                    int t_orig = result[eventList[i]].TimeSlotId;
                    for (int h = 0, t = t_start; h < data.TotalTimeSlots; t = (t + 1) % data.TotalTimeSlots, h++)
                    {
                        if (stepCount > maxSteps)
                        {
                            break;
                        }

                        if (rg.NextDouble() < prob1)
                        {			// with given probability
                            stepCount++;

                            Solution neighbourSolution = new Solution();
                            this.CopyTo(neighbourSolution);

                            neighbourSolution.Move1(eventList[i], t);
                            neighbourAffectedHcv = neighbourSolution.eventAffectedHcv(eventList[i]) + neighbourSolution.affectedRoomInTimeslotHcv(t_orig);
                            currentAffectedHcv = eventAffectedHcv(eventList[i]) + affectedRoomInTimeslotHcv(t);
                            if (neighbourAffectedHcv < currentAffectedHcv)
                            {
                                 neighbourSolution.CopyTo(this);
                                evCount = 0;
                                foundbetter = true;
                                break;
                            }
                        }
                    }

                    if (foundbetter)
                    {
                        foundbetter = false;
                        continue;
                    }

                    if (prob2 != 0)
                    {
                        for (int j = (i + 1) % data.Events.Length; j != i; j = (j + 1) % data.Events.Length)
                        {			// try moves of type 2
                            if (stepCount > maxSteps)
                            {
                                break;
                            }

                            if (rg.NextDouble() < prob2)
                            {		// with given probability
                                stepCount++;

                                Solution neighbourSolution = new Solution();
                                this.CopyTo(neighbourSolution);
                                neighbourSolution.Move2(eventList[i], eventList[j]);

                                neighbourAffectedHcv = neighbourSolution.eventAffectedHcv(eventList[i]) + 
                                                       neighbourSolution.eventAffectedHcv(eventList[j]);
                                currentAffectedHcv = eventAffectedHcv(eventList[i]) + eventAffectedHcv(eventList[j]);
                                if (neighbourAffectedHcv < currentAffectedHcv)
                                {
                                     neighbourSolution.CopyTo(this);
                                    evCount = 0;
                                    foundbetter = true;
                                    break;
                                }
                            }
                        }

                        if (foundbetter)
                        {
                            foundbetter = false;
                            continue;
                        }
                    }

#if FALSE // TODO: try to make Move3
                    if (prob3 != 0)
                    {
                        for (int j = (i + 1) % data->n_of_events; j != i; j = (j + 1) % data->n_of_events)
                        {			// try moves of type 3
                            if (stepCount > maxSteps)
                            {
                                break;
                            }

                            for (int k = (j + 1) % data->n_of_events; k != i; k = (k + 1) % data->n_of_events)
                            {
                                if (stepCount > maxSteps)
                                {
                                    break;
                                }

                                if (rg->next() < prob3)
                                {	// with given probability
                                    stepCount++;
                                    currentAffectedHcv = eventAffectedHcv(eventList[i]) + eventAffectedHcv(eventList[j]) + eventAffectedHcv(eventList[k]);

                                    Solution* neighbourSolution = new Solution(data, rg);
                                    neighbourSolution->copy(this);
                                    neighbourSolution->Move3(eventList[i], eventList[j], eventList[k]); //try one of the to possible 3-cycle

                                    //cout<< "event " << eventList[i] << " second event " << eventList[j] << " third event "<< eventList[k] << endl;
                                    neighbourAffectedHcv = neighbourSolution->eventAffectedHcv(eventList[i]) +
                                        neighbourSolution->eventAffectedHcv(eventList[j]) +
                                        neighbourSolution->eventAffectedHcv(eventList[k]);
                                    if (neighbourAffectedHcv < currentAffectedHcv)
                                    {
                                        copy(neighbourSolution);
                                        delete neighbourSolution;
                                        evCount = 0;
                                        foundbetter = true;
                                        break;
                                    }

                                    delete neighbourSolution;
                                }

                                if (stepCount > maxSteps)
                                {
                                    break;
                                }

                                if (rg->next() < prob3)
                                {	// with given probability
                                    stepCount++;
                                    currentAffectedHcv = eventAffectedHcv(eventList[i]) + eventAffectedHcv(eventList[k]) + eventAffectedHcv(eventList[j]);

                                    Solution* neighbourSolution = new Solution(data, rg);
                                    neighbourSolution->copy(this);
                                    neighbourSolution->Move3(eventList[i], eventList[k], eventList[j]); //try one of the to possible 3-cycle

                                    //cout<< "event " << eventList[i] << " second event " << eventList[j] << " third event "<< eventList[k] << endl;
                                    neighbourAffectedHcv = neighbourSolution->eventAffectedHcv(eventList[i]) +
                                        neighbourSolution->eventAffectedHcv(eventList[k]) +
                                        neighbourSolution->eventAffectedHcv(eventList[j]);
                                    if (neighbourAffectedHcv < currentAffectedHcv)
                                    {
                                        copy(neighbourSolution);
                                        delete neighbourSolution;
                                        evCount = 0;
                                        foundbetter = true;
                                        break;
                                    }

                                    delete neighbourSolution;
                                }
                            }

                            if (foundbetter)
                            {
                                break;
                            }
                        }

                        if (foundbetter)
                        {
                            foundbetter = false;
                            continue;
                        }
                    }
#endif
                    evCount++;
                }
            }

            computeFeasibility();

#if FALSE // TODO: try to resolve soft constraits violations
            if (feasible)
            {						// if the timetable is feasible
                evCount = 0;

                int neighbourHcv;
                for (int i = 0; evCount < data->n_of_events; i = (i + 1) % data->n_of_events)
                {					//go through the events in the list
                    if (stepCount > maxSteps)
                    {
                        break;
                    }

                    int currentScv = eventScv(eventList[i]);

                    //cout << "event " << eventList[i] << " cost " << currentScv<<endl;
                    if (currentScv == 0)
                    {				// if there are no scv
                        evCount++;	// increase counter
                        continue;	//go to the next event
                    }

                    // otherwise try all the possible moves
                    int t_start = (int)(rg->next() * 45);	// try moves of type 1
                    for (int h = 0, t = t_start; h < 45; t = (t + 1) % 45, h++)
                    {
                        if (stepCount > maxSteps)
                        {
                            break;
                        }

                        if (rg->next() < prob1)
                        {	// each with given propability
                            stepCount++;

                            Solution* neighbourSolution = new Solution(data, rg);
                            neighbourSolution->copy(this);
                            neighbourSolution->Move1(eventList[i], t);

                            //cout<< "event " << eventList[i] << " timeslot " << t << endl;
                            neighbourHcv = neighbourSolution->eventAffectedHcv(eventList[i]);	//count possible hcv introduced by move
                            if (neighbourHcv == 0)
                            {	// consider the move only if no hcv are introduced
                                //cout<< "reintroduced hcv" << neighbourSolution->computeHcv()<< endl;
                                neighbourScv = neighbourSolution->eventScv(eventList[i])	// respectively Scv involving event e
                                + singleClassesScv(eventList[i])						// + single classes introduced in day of original timeslot
                                - neighbourSolution->singleClassesScv(eventList[i]);	// - single classes "solved" in new day

                                //cout<< "neighbour cost " << neighbourScv<<" " << neighbourHcv<< endl;
                                if (neighbourScv < currentScv)
                                {
                                    //cout<<"current scv " << computeScv() << "neighbour " << neighbourSolution->computeScv()<< endl;
                                    copy(neighbourSolution);
                                    evCount = 0;
                                    foundbetter = true;
                                    break;

                                }
                            }
                        }
                    }

                    if (foundbetter)
                    {
                        foundbetter = false;
                        continue;
                    }

                    if (prob2 != 0)
                    {
                        for (int j = (i + 1) % data->n_of_events; j != i; j = (j + 1) % data->n_of_events)
                        {				//try moves of type 2
                            if (stepCount > maxSteps)
                            {
                                break;
                            }

                            if (rg->next() < prob2)
                            {			// with the given probability
                                stepCount++;

                                Solution* neighbourSolution = new Solution(data, rg);
                                neighbourSolution->copy(this);

                                //cout<< "event " << eventList[i] << " second event " << eventList[j] << endl;
                                neighbourSolution->Move2(eventList[i], eventList[j]);

                                //count possible hcv introduced with the move
                                neighbourHcv = neighbourSolution->eventAffectedHcv(eventList[i]) + neighbourSolution->eventAffectedHcv(eventList[j]);
                                if (neighbourHcv == 0)
                                {		// only if no hcv are introduced by the move
                                    //cout<< "reintroduced hcv" << neighbourSolution->computeHcv()<< endl;
                                    // compute alterations on scv for neighbour solution
                                    neighbourScv = neighbourSolution->eventScv(eventList[i]) +
                                        singleClassesScv(eventList[i]) -
                                        neighbourSolution->singleClassesScv(eventList[i]) +
                                        neighbourSolution->eventScv(eventList[j]) +
                                        singleClassesScv(eventList[j]) -
                                        neighbourSolution->singleClassesScv(eventList[j]);

                                    // cout<< "neighbour cost " << neighbourScv<<" " << neighbourHcv<< endl;
                                    if (neighbourScv < currentScv + eventScv(eventList[j]))
                                    {	// if scv are reduced
                                        //cout<<"current scv " << computeScv() << "neighbour " << neighbourSolution->computeScv()<< endl;
                                        copy(neighbourSolution);	// do the move
                                        evCount = 0;
                                        foundbetter = true;
                                        break;
                                    }
                                }
                            }
                        }

                        if (foundbetter)
                        {
                            foundbetter = false;
                            continue;
                        }
                    }

                    if (prob3 != 0)
                    {
                        for (int j = (i + 1) % data->n_of_events; j != i; j = (j + 1) % data->n_of_events)
                        {				//try moves of type 3
                            if (stepCount > maxSteps)
                            {
                                break;
                            }

                            for (int k = (j + 1) % data->n_of_events; k != i; k = (k + 1) % data->n_of_events)
                            {
                                if (stepCount > maxSteps)
                                {
                                    break;
                                }

                                if (rg->next() < prob3)
                                {		// with given probability try one of the 2 possibles 3-cycles
                                    stepCount++;

                                    Solution* neighbourSolution = new Solution(data, rg);
                                    neighbourSolution->copy(this);
                                    neighbourSolution->Move3(eventList[i], eventList[j], eventList[k]);

                                    // cout<< "event " << eventList[i] << " second event " << eventList[j] << " third event "<< eventList[k] << endl;
                                    // compute the possible hcv introduced by the move
                                    neighbourHcv = neighbourSolution->eventAffectedHcv(eventList[i]) +
                                        neighbourSolution->eventAffectedHcv(eventList[j]) +
                                        neighbourSolution->eventAffectedHcv(eventList[k]);
                                    if (neighbourHcv == 0)
                                    {	// consider the move only if hcv are not introduced
                                        // compute alterations on scv for neighbour solution
                                        neighbourScv = neighbourSolution->eventScv(eventList[i]) +
                                            singleClassesScv(eventList[i]) -
                                            neighbourSolution->singleClassesScv(eventList[i]) +
                                            neighbourSolution->eventScv(eventList[j]) +
                                            singleClassesScv(eventList[j]) -
                                            neighbourSolution->singleClassesScv(eventList[j]) +
                                            neighbourSolution->eventScv(eventList[k]) +
                                            singleClassesScv(eventList[k]) -
                                            neighbourSolution->singleClassesScv(eventList[k]);

                                        // cout<< "neighbour cost " << neighbourScv<<" " << neighbourHcv<< endl;
                                        if (neighbourScv < currentScv + eventScv(eventList[j]) + eventScv(eventList[k]))
                                        {
                                            copy(neighbourSolution);
                                            evCount = 0;
                                            foundbetter = true;
                                            break;
                                        }
                                    }
                                }

                                if (stepCount > maxSteps)
                                {
                                    break;
                                }

                                if (rg->next() < prob3)
                                {		// with the same probability try the other possible 3-cycle for the same 3 events
                                    stepCount++;

                                    Solution* neighbourSolution = new Solution(data, rg);
                                    neighbourSolution->copy(this);
                                    neighbourSolution->Move3(eventList[i], eventList[k], eventList[j]);

                                    // cout<< "event " << eventList[i] << " second event " << eventList[k] << " third event "<< eventList[j] << endl;
                                    // compute the possible hcv introduced by the move
                                    neighbourHcv = neighbourSolution->eventAffectedHcv(eventList[i]) +
                                        neighbourSolution->eventAffectedHcv(eventList[k]) +
                                        neighbourSolution->eventAffectedHcv(eventList[j]);
                                    if (neighbourHcv == 0)
                                    {	// consider the move only if hcv are not introduced
                                        // compute alterations on scv for neighbour solution
                                        neighbourScv = neighbourSolution->eventScv(eventList[i]) +
                                            singleClassesScv(eventList[i]) -
                                            neighbourSolution->singleClassesScv(eventList[i]) +
                                            neighbourSolution->eventScv(eventList[k]) +
                                            singleClassesScv(eventList[k]) -
                                            neighbourSolution->singleClassesScv(eventList[k]) +
                                            neighbourSolution->eventScv(eventList[j]) +
                                            singleClassesScv(eventList[j]) -
                                            neighbourSolution->singleClassesScv(eventList[j]);

                                        // cout<< "neighbour cost " << neighbourScv<<" " << neighbourHcv<< endl;
                                        if (neighbourScv < currentScv + eventScv(eventList[k]) + eventScv(eventList[j]))
                                        {
                                            copy(neighbourSolution);
                                            evCount = 0;
                                            foundbetter = true;
                                            break;
                                        }
                                    }
                                }
                            }

                            if (foundbetter)
                            {
                                break;
                            }
                        }

                        if (foundbetter)
                        {
                            foundbetter = false;
                            continue;
                        }
                    }

                    evCount++;
                }
            }
#endif
        }

        public void AssignRoomForEachTimeSlot()
        {
            // assign rooms to events in each non-empty timeslot
            for (int i = 0; i < data.TotalTimeSlots; i++)
                if (timeslot_events[i].Count != 0)
                    assignRooms(i);
        }

        int[] roomsAssignmentCounters;
        /// <summary>
        /// Assign rooms to events for timeslot t (first apply matching algorithm and then assign unplaced rooms).
        /// </summary>
        void assignRooms(int t)
        {
            //// stupid random
            //foreach (int e in timeslot_events[t])
            //{
            //    result[e].RoomId = (int)(rg.NextDouble() * data.Rooms.Length);
            //}
            int eventsCount = timeslot_events[t].Count;            
            var x = HopcroftKarpMatching(MakeGraph(t), eventsCount);

            roomsAssignmentCounters = new int[data.Rooms.Length];
            //roomsAssignmentCounters.Initialize();
            for (int e = 0; e < eventsCount; e++)
            {
                if (x[e] != NIL_VERT_ID)
                {
                    int roomId = x[e] - eventsCount;
                    result[timeslot_events[t][e]].RoomId = roomId;
                    roomsAssignmentCounters[roomId]++;
                }
            }

            for (int e = 0; e < eventsCount; e++)
            {
                if (x[e] == NIL_VERT_ID)
                {
                    int lessBusyRoom = FindLessBusyRoom(roomsAssignmentCounters);
                    result[timeslot_events[t][e]].RoomId = lessBusyRoom;
                    roomsAssignmentCounters[lessBusyRoom]++;
                }
            }
        }

        int FindLessBusyRoom(int[] roomsAssignments)
        {
            int id = -1;
            int min = int.MaxValue;
            for (int i = 0; i < roomsAssignments.Length; i++)
            {
                if (roomsAssignments[i] < min)
                {
                    min = roomsAssignments[i];
                    id = i;
                }
            }
            return id;
        }

        #region Mathing algorithm

        int[] Pair_G1, Pair_G2, Dist;
        int NIL_VERT_ID;
        bool[,] Graph;
        int FIRST_PART_COUNT;
        int N;

        //result: event -> room
        int[] HopcroftKarpMatching(bool[,] g, int fBipartCount)
        {
            Graph = g;
            FIRST_PART_COUNT = fBipartCount;
            N = Graph.GetLength(0);
            NIL_VERT_ID = N - 1;
            Pair_G1 = Enumerable.Repeat(NIL_VERT_ID, N).ToArray();
            Pair_G2 = Enumerable.Repeat(NIL_VERT_ID, N).ToArray();
            Dist = new int[N];
            while (BFS())
            {
                for (int v = 0; v < FIRST_PART_COUNT; v++)
                {
                    if (Pair_G1[v] == NIL_VERT_ID)
                        DFS(v);
                }
            }
            return Pair_G1.Take(FIRST_PART_COUNT).ToArray();
        }

        bool BFS()
        {
            Queue<int> q = new Queue<int>();
            for (int v = 0; v < FIRST_PART_COUNT; v++)
            {
                if (Pair_G1[v] == NIL_VERT_ID)
                {
                    Dist[v] = 0;
                    q.Enqueue(v);
                }
                else
                    Dist[v] = int.MaxValue;
            }

            Dist[NIL_VERT_ID] = int.MaxValue;

            while (q.Count != 0)
            {
                int v = q.Dequeue();
                for (int u = FIRST_PART_COUNT; u < N; u++)
                {
                    if (Graph[u, v] && Dist[Pair_G2[u]] == int.MaxValue)
                    {
                        Dist[Pair_G2[u]] = Dist[v] + 1;
                        q.Enqueue(Pair_G2[u]);
                    }
                }
            }

            return Dist[NIL_VERT_ID] != int.MaxValue; 
        }

        bool DFS(int v) 
        {
            if (v != NIL_VERT_ID)
            {
                for (int u = FIRST_PART_COUNT; u < N; u++)
                {
                    if (Graph[u, v] && Dist[Pair_G2[u]] == Dist[v] + 1)
                        if (DFS(Pair_G2[u]))
                        {
                            Pair_G2[u] = v;
                            Pair_G1[v] = u;
                            return true;
                        }
                }
                Dist[v] = int.MaxValue;
                return false;
            }
            return true;
        }

        bool[,] MakeGraph(int timeslot)
        {
            int eventsCount = timeslot_events[timeslot].Count;
            //verts 0..evenstCount - 1 - events verts ids
            //verts eventsCount..eventsCount + data.Rooms.Count - 1 - rooms verts ids
            //vert eventsCount + data.Rooms.Count - NULL vert
            var res = new bool[eventsCount + data.Rooms.Length + 1, eventsCount + data.Rooms.Length + 1];
            for (int e = 0; e < eventsCount; e++)
            {
                RoomType eventType = data.Events[timeslot_events[timeslot][e]].RoomType;
                for (int r = 0; r < data.Rooms.Length; r++)
                {
                    if (eventType == data.Rooms[r].Type)
                    {
                        res[e, eventsCount + r] = res[eventsCount + r, e] = true;
                    }
                }
            }
            return res;
        }

        #endregion
    }
}
