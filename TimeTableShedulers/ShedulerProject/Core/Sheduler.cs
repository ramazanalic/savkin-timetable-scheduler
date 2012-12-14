using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SchedulerProject.Core
{
    static class Scheduler
    {
        const int ANTS_NUMBER = 10;
        const int DEFAULT_MAX_STEPS = 100;
        const int MAX_ITER = 500;
        const double EVAPORATION = 0.1;
        const double MIN_PHERAMONE = 0.3;

        static public TimeTable Shedule(TimeTableData problemData)
        {
            problemData.PrepareHelpers();
            var sw = System.Diagnostics.Stopwatch.StartNew();
            Solution firstWeekSolution = Shedule(problemData, 1);
            sw.Stop();
            Console.WriteLine(sw.ElapsedMilliseconds + " ms");

            sw.Start();
            // try to partially apply the first week solution to the second one
            Solution secondWeekSolution = Shedule(problemData, 2);
            sw.Stop();
            Console.WriteLine(sw.ElapsedMilliseconds + " ms");

            return MakeTimeTable(problemData, firstWeekSolution.result, secondWeekSolution.result);
        }

        static Solution Shedule(TimeTableData problemData, int week)
        {
            TimeTableData timeTable = problemData;
            MMASData mmasData = new MMASData(timeTable, week, EVAPORATION, MIN_PHERAMONE);
            
            Solution bestSoFarSolution = new Solution(problemData, week);

            bestSoFarSolution.RandomInitialSolution();
            bestSoFarSolution.computeFeasibility();

            mmasData.ResetPheromone();
            for (int i = 0; i < MAX_ITER; i++)
            {
                Solution bestIterSolution = Enumerable.Range(0, ANTS_NUMBER)
                                                      //.AsParallel()
                                                      .Select(_ => new Ant(timeTable, mmasData, week).GetSolution())
                                                      .Min();

                // apply local search until local optimum is reached or a time limit reached
                bestIterSolution.localSearch(DEFAULT_MAX_STEPS);

                // and see if the solution is feasible
                bestIterSolution.computeFeasibility();

                // output the new best solution, if found
                if (bestIterSolution.feasible)
                {

                    bestIterSolution.computeScv();
                    if (bestIterSolution.scv <= bestSoFarSolution.scv)
                    {
                        bestIterSolution.CopyTo(bestSoFarSolution);
                        bestSoFarSolution.hcv = 0;
                    }
                }
                else
                {
                    bestIterSolution.computeHcv();
                    if (bestIterSolution.hcv <= bestSoFarSolution.hcv)
                    {
                        bestIterSolution.CopyTo(bestSoFarSolution);
                        bestSoFarSolution.scv = int.MaxValue;
                    }
                }

                // update pheromones
                mmasData.EvaporatePheromone();
                mmasData.SetPheromoneLimits();
                mmasData.DepositPheromone(bestSoFarSolution);
                bestSoFarSolution.computeHcv();
                Console.WriteLine("iter: " + i + ", HCV: " + bestSoFarSolution.hcv);
            }

            return bestSoFarSolution;
        }

        static TimeTable MakeTimeTable(TimeTableData data, 
                                       InternalEventAssignment[] firtsWeekAssignments, 
                                       InternalEventAssignment[] secondWeekAssignments)
        {
            var result = new TimeTable(data);
            int week = 0;
            foreach (var assignments in new[] { firtsWeekAssignments, secondWeekAssignments })
            {
                week++;
                for (int i = 0; i < assignments.Length; i++)
                {
                    Event ev = data.Events.First(e => e.Id == data.GetWeekEvents(week)[i].Id);
                    Room room = data.Rooms.First(r => r.Id == assignments[i].RoomId);
                    TimeSlot slot = TimeSlot.FromId(assignments[i].TimeSlotId,
                                                    data.Days, data.SlotsPerDay);

                    result.AddAssignment(ev, room, slot, week);
                }
            }

            return result;
        }
    }
}
