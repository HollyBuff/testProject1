using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;


namespace test1
{
    public class phase1
    {
        public struct item
        {
            public string name;
            public double cost;
            public double value;
        }

        public Tuple<int, List<item>> CSVin(string filename)
        {
            List<item> knapsack = new List<item>();

            string[] lines = System.IO.File.ReadAllLines(@filename);

            int cap;
            cap = int.Parse(lines[0]);

            foreach (var stuff in lines.Skip(1))
            {
                var temp = stuff.Split(',');

                item thing;

                thing.name = temp[0];
                thing.cost = int.Parse(temp[1]);
                thing.value = int.Parse(temp[2]);
                knapsack.Add(thing);
            }
            return Tuple.Create(cap, knapsack);
        }


        public Tuple<double, List<item>> dumbExhaustiveSearch(List<item> knapsack, int cap, Stopwatch time)
        {
            double bestValue = 0;
            int bestPosition = 0;
            int size = knapsack.Count();
            TimeSpan baseInterval = new TimeSpan(0, 10, 0);

            var permutations = (long)Math.Pow(2, size);

            for (int i = 0; i < permutations; i++)
            {
                double total = 0;
                double weight = 0;
                for (int j = 0; j < size; j++)
                {
                    if (((i >> j) & 1) != 1)
                        continue;
                    total += knapsack[j].value;
                    weight += knapsack[j].cost;
                    if (TimeSpan.Compare(time.Elapsed, baseInterval) == 1)
                        break;
                }
                if (weight <= cap && total > bestValue)
                {
                    bestPosition = i;
                    bestValue = total;
                }
                if (TimeSpan.Compare(time.Elapsed, baseInterval) == 1)
                    break;
            }
            var include = new List<item>();
            for (int j = 0; j < size; j++)
            {
                if (((bestPosition >> j) & 1) == 1)
                    include.Add(knapsack[j]);
            }
            return Tuple.Create(bestValue, include);
        }

        public Tuple<double, List<item>> betterExhaustiveSearch(List<item> knapsack, int cap, Stopwatch time, double min)
        {
            double bestValue = 0;
            int bestPosition = 0;
            int size = knapsack.Count();
            TimeSpan baseInterval = new TimeSpan(0, 10, 0);

            var permutations = (long)Math.Pow(2, size);

            for (int i = 0; i < permutations; i++)
            {
                double total = 0;
                double weight = 0;
                for (int j = 0; j < size; j++)
                {
                    if (((i >> j) & 1) != 1)
                        continue;
                    total += knapsack[j].value;
                    weight += knapsack[j].cost;

                    int remaining = j - size;
                    double valueLeft = 0;
                    for (int m = 0; m <= remaining; m++)
                    {
                        valueLeft += knapsack[m].value;
                    }

                    if (total > cap || valueLeft < min)
                        break;

                    if (TimeSpan.Compare(time.Elapsed, baseInterval) == 1)
                        break;
                }
                if (weight <= cap && total > bestValue)
                {
                    bestPosition = i;
                    bestValue = total;
                }
                if (TimeSpan.Compare(time.Elapsed, baseInterval) == 1)
                    break;
            }
            var include = new List<item>();
            for (int j = 0; j < size; j++)
            {
                if (((bestPosition >> j) & 1) == 1)
                    include.Add(knapsack[j]);
            }
            return Tuple.Create(bestValue, include);
        }


        public Tuple<double, List<item>> greedySearch(List<item> knapsack, int cap)
        {
            double soFar = 0, totalCost = 0;
            var include = new List<item>();

            foreach (var next in knapsack)
            {
                if ((totalCost += next.cost) <= cap)
                {
                    soFar += next.value;
                    include.Add(next);
                }
                else
                    break;
            }
            return Tuple.Create(soFar, include);
        }

        public Tuple<double, List<item>> partial(List<item> knapsack, int cap)
        {
            double soFar = 0, totalCost = 0;
            var include = new List<item>();

            foreach (var next in knapsack)
            {
                if ((totalCost + next.cost) <= cap)
                {
                    totalCost += next.cost;
                    soFar += next.value;
                    include.Add(next);
                }
                else
                {
                    soFar += ((next.value * (cap - totalCost)) / next.cost);
                    include.Add(next);
                    break;
                }
            }
            return Tuple.Create(soFar, include);
        }

        public static void Main()
        {
            phase1 phase = new phase1();
            List<item> knapsack = new List<item>();
            List<item> highvalList = new List<item>();
            List<item> lowCostList = new List<item>();
            List<item> ratioList = new List<item>();
            List<item> partList = new List<item>();

            string filename;
            Console.Write("enter the filename: ");
            filename = Console.ReadLine();

            Stopwatch time = new Stopwatch();

            Tuple<int, List<item>> tuple = phase.CSVin(filename);

            int capacity = tuple.Item1;
            knapsack = tuple.Item2;


            //tree = makeTree(knapsack);

            Tuple<double, List<item>> tuple1 = phase.greedySearch(knapsack.OrderByDescending(x => x.value).ToList(), capacity);
            double highVal = tuple1.Item1;
            highvalList = tuple1.Item2.OrderBy(x => x.name).ToList();
            string highList = "";
            foreach (var thing in highvalList)
                highList += thing.name + "," + Convert.ToString(thing.cost) + "," + Convert.ToString(thing.value) + "\n";

            Tuple<double, List<item>> tuple2 = phase.greedySearch(knapsack.OrderBy(x => x.cost).ToList(), capacity);
            double lowCost = tuple2.Item1;
            lowCostList = tuple2.Item2.OrderBy(x => x.name).ToList();
            string lowList = "";
            foreach (var thing in lowCostList)
                lowList += thing.name + "," + Convert.ToString(thing.cost) + "," + Convert.ToString(thing.value) + "\n";

            Tuple<double, List<item>> tuple3 = phase.greedySearch(knapsack.OrderByDescending(x => x.value / x.cost).ToList(), capacity);
            double ratio = tuple3.Item1;
            ratioList = tuple3.Item2.OrderBy(x => x.name).ToList();
            string ratList = "";
            foreach (var thing in ratioList)
                ratList += thing.name + "," + Convert.ToString(thing.cost) + "," + Convert.ToString(thing.value) + "\n";

            Tuple<double, List<item>> tuple4 = phase.partial(knapsack.OrderByDescending(x => x.value / x.cost).ToList(), capacity);
            double part = tuple4.Item1;
            partList = tuple4.Item2.OrderBy(x => x.name).ToList();
            string parList = "";
            foreach (var thing in partList)
                parList += thing.name + "," + Convert.ToString(thing.cost) + "," + Convert.ToString(thing.value) + "\n";

            time.Start();
            Tuple<double, List<item>> tuple5 = phase.dumbExhaustiveSearch(knapsack, capacity, time);
            time.Stop();
            TimeSpan buildTime = time.Elapsed;
            string timeSpent = Convert.ToString(buildTime);

            List<item> exList = tuple5.Item2.OrderBy(x => x.name).ToList();
            double exhaustive = tuple5.Item1;
            string exhList = "";
            foreach (var thing in exList)
                exhList += thing.name + "," + Convert.ToString(thing.cost) + "," + Convert.ToString(thing.value) + "\n";


            double minBound = Math.Min(Math.Min(highVal, lowCost), Math.Min(ratio, part));
            double maxBound = Math.Max(Math.Max(highVal, lowCost), Math.Max(ratio, part));

            time.Reset();
            time.Start();
            Tuple<double, List<item>> tuple6 = phase.betterExhaustiveSearch(knapsack.OrderByDescending(x => x.value / x.cost).ToList(), capacity, time, minBound);
            time.Stop();
            TimeSpan buildTime2 = time.Elapsed;
            string timeSpent2 = Convert.ToString(buildTime2);

            List<item> prunedList = tuple6.Item2.OrderBy(x => x.name).ToList();
            double pruned = tuple5.Item1;
            string pruneList = "";
            foreach (var thing in exList)
                pruneList += thing.name + "," + Convert.ToString(thing.cost) + "," + Convert.ToString(thing.value) + "\n";

            string greedyMin = "";
            if (minBound == highVal)
                greedyMin = highList;
            else if (minBound == lowCost)
                greedyMin = lowList;
            else if (minBound == ratio)
                greedyMin = ratList;
            else
                greedyMin = "Final value is partially included: \n" + parList;

            string greedyMax = "";
            if (maxBound == highVal)
                greedyMax = highList;
            else if (maxBound == lowCost)
                greedyMax = lowList;
            else if (maxBound == ratio)
                greedyMax = ratList;
            else
                greedyMax = "Final value is partially included: \n" + parList;


            string text = "Filename: " + filename + "\n\nCapacity: " + capacity + "\n\nGreedy minimum boundry: \n" + greedyMin + "\nGreedy maximum boundry: \n" + greedyMax + "\nOptimal Solution: \n" + exhList + "\nDumb search completed after " + timeSpent + "\nSmarter search completed after " + timeSpent2;


            string file = filename;
            file = System.IO.Path.GetFileNameWithoutExtension(@file);
            file += ".txt";

            System.IO.File.WriteAllText(@file, text);



            //	Console.WriteLine("highVal: " + highVal);
            //	Console.WriteLine("lowCost: " + lowCost);
            //	Console.WriteLine("ratio: " + ratio);
            //	Console.WriteLine("partial: " + part);

            //	Console.Write("exhaustive: " + exhaustive);
            //	Console.WriteLine();
            //	foreach (var thing in exList)
            //		Console.Write(thing.name + ",");
            //	Console.WriteLine();
            //	Console.WriteLine("Exhaustive search completed after " + buildTime);
            //	Console.ReadKey();
            //
        }
    }
}
