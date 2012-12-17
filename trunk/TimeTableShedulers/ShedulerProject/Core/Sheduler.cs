using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SchedulerProject.Core
{
    static class Scheduler
    {
        const int ANTS_NUMBER = 10;
        const int DEFAULT_MAX_STEPS = 250;
        const int MAX_ITER = 30;
        const double EVAPORATION = 0.05;
        const double MIN_PHERAMONE = 0.3;

        static public TimeTable Shedule(TimeTableData problemData)
        {
            problemData.PrepareHelpers();
            var sw = System.Diagnostics.Stopwatch.StartNew();
            Solution firstWeekSolution = Shedule(problemData, 1);
            sw.Stop();
            System.Windows.Forms.MessageBox.Show(sw.ElapsedMilliseconds + " ms");
            Console.WriteLine(sw.ElapsedMilliseconds + " ms");

            var firstWeekAssignments = firstWeekSolution.ScheduledWeeklyAssignments;

            sw.Start();
            // try to partially apply the first week solution to the second one
            Solution secondWeekSolution = Shedule(problemData, 2, firstWeekAssignments);
            sw.Stop();
            System.Windows.Forms.MessageBox.Show(sw.ElapsedMilliseconds + " ms");
            Console.WriteLine(sw.ElapsedMilliseconds + " ms");

            var secondWeekAssignments = secondWeekSolution.ScheduledWeeklyAssignments;

            var result = new TimeTable(problemData);

            foreach (var assignment in firstWeekAssignments)
                result.AddAssignment(assignment);

            foreach (var assignment in secondWeekAssignments)
                result.AddAssignment(assignment);

            return result;
        }

        static Solution Shedule(TimeTableData problemData, int week, 
                                WeeklyEventAssignment[] guidingAssignments = null)
        {
            TimeTableData timeTable = problemData;
            MMASData mmasData = new MMASData(timeTable, week, EVAPORATION, MIN_PHERAMONE);
            if (guidingAssignments == null)
                mmasData.ResetPheromone();
            else 
                mmasData.SetPheromoneFromExistingAssignments(guidingAssignments);
            
            Solution bestSoFarSolution = new Solution(problemData, week);

            bestSoFarSolution.RandomInitialSolution();
            bestSoFarSolution.computeFeasibility();
            bestSoFarSolution.computeHcv();

            int i = 0;

            while (bestSoFarSolution.hcv > 0)
            {
                Solution bestIterSolution = Enumerable.Range(0, ANTS_NUMBER)
                                                      //.AsParallel()
                                                      .Select(_ => 
                                                      {
                                                          var ant = new Ant(timeTable, mmasData, week);
                                                          return guidingAssignments == null ? 
                                                              ant.GetSolution() :
                                                              ant.GetSolution(guidingAssignments);
                                                      })
                                                      .Min();

                // apply local search until local optimum is reached or a time limit reached
                //bestIterSolution.computeHcv();
                //Console.WriteLine("before LS: " + bestIterSolution.hcv);
                bestIterSolution.DoHcvCheckBeforeRoomAssignment = guidingAssignments != null;
                bestIterSolution.localSearch(DEFAULT_MAX_STEPS);
                //bestIterSolution.computeHcv();
                //Console.WriteLine("after LS: " + bestIterSolution.hcv);

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
                Console.WriteLine("iter: " + i++ + ", HCV: " + bestSoFarSolution.hcv);
            }

            return bestSoFarSolution;
        }
    }
}
