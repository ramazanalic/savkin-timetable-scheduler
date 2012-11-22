using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SchedulerProject.Core
{
    class Scheduler
    {
        const int ANTS_NUMBER = 5;
        const int DEFAULT_MAX_STEPS = 100;
        const int MAX_ITER = 50;
        const double EVAPORATION = 0.1;
        const double MIN_PHERAMONE = 0.3;

        static public EventAssignment[] Shedule(TimeTable inputTimeTable)
        {
            TimeTable timeTable = inputTimeTable;
            MMASData mmasData = new MMASData(timeTable, EVAPORATION, MIN_PHERAMONE);
            
            Solution bestSoFarSolution = new Solution(inputTimeTable);

            bestSoFarSolution.RandomInitialSolution();
            bestSoFarSolution.computeFeasibility();

            mmasData.ResetPheromone();
            for (int i = 0; i < MAX_ITER; i++)
            {
                Solution bestIterSolution = Enumerable.Range(0, ANTS_NUMBER)
                                                      //.AsParallel()
                                                      .Select(_ => new Ant(timeTable, mmasData).GetSolution())
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
            }

            bestSoFarSolution.computeHcv();
            Console.WriteLine("----------------------");
            Console.WriteLine("HCV: " + bestSoFarSolution.hcv);
            return bestSoFarSolution.result;
        }
    }
}
