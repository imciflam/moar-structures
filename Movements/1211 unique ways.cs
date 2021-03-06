using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movements
{

    public class Graph<T>
    {
        public Dictionary<T, HashSet<T>> AdjacencyList { get; } = new Dictionary<T, HashSet<T>>();

        public Graph(IEnumerable<T> vertices, IEnumerable<Tuple<T, T>> edges)
        {
            foreach (var vertex in vertices)
            {
                AddVertex(vertex);
                //Console.WriteLine(vertex);
            }

            foreach (var edge in edges)
            {
                AddEdge(edge);
                //Console.WriteLine(edge);
            }
        }

        public void AddVertex(T vertex)
        {
            AdjacencyList[vertex] = new HashSet<T>();
        }

        public void AddEdge(Tuple<T, T> edge)
        {
            if (AdjacencyList.ContainsKey(edge.Item1) && AdjacencyList.ContainsKey(edge.Item2))
            {
                AdjacencyList[edge.Item1].Add(edge.Item2);//откуда и куда
            }
        }


    }

    class Program
    {
        /// <summary>
        /// Движение между отделениями
        /// </summary>
        class Movement
        {
            /// <summary>
            /// Отделение откуда
            /// </summary>
            public int From;
            /// <summary>
            /// Отделение куда
            /// </summary>
            public int To;
            /// <summary>
            /// Индекс перехода
            /// </summary>
            public int index;

            public bool visited = false;
            // Можно добавлять в класс вспомогательные члены
        }


        static void Main(string[] args)
        {
            try
            {
                string[] ff = System.IO.File.ReadAllLines(@"..\..\movements2.txt");
                List<int> x = new List<int>();
                List<int> y = new List<int>();
                for (int i = 0; i < ff.Length; i++)
                {
                    string[] ss = ff[i].Split(';');
                    x.Add(Convert.ToInt32(ss[0]));
                    y.Add(Convert.ToInt32(ss[1]));
                }
                List<int> z = x.Concat(y).Distinct().ToList();
                //Console.WriteLine(z.Count);
                int[] vertices = z.ToArray();

                var edges = new Tuple<int, int>[ff.Length];
                for (int i = 0; i < ff.Length; i++)
                {
                    string[] ss = ff[i].Split(';');
                    edges[i] = Tuple.Create(Convert.ToInt32(ss[0]), Convert.ToInt32(ss[1]));
                }

                var graph = new Graph<int>(vertices, edges);


                //Movement[] ms = new Movement[]
                //{
                //    new Movement() { From = "334", To = "331", index = 0 },
                //    new Movement() { From = "331", To = "334", index = 1 },
                //    new Movement() { From = "332", To = "331", index = 2 },
                //    //new Movement() { From = "125", To = "112", index = 4 }
                //};
                //string begin = "332";

                Movement[] ms = new Movement[ff.Length];
                for (int i = 0; i < ff.Length; i++)
                {
                    string[] ss = ff[i].Split(';');
                    ms[i] = new Movement() { From = Convert.ToInt32(ss[0]), To = Convert.ToInt32(ss[1]), index = i };
                }
                int begin = Convert.ToInt32(ms[0].From);


                // поиск вариантов переходов
                //Stopwatch start = Stopwatch.StartNew();
                //List<int[]> result = FindMovements(begin, ms);
                // start.Stop();
                // Console.WriteLine(start);

                // печать результатов
                //PrintResults(ms, begin, result);
                //FindMovements(begin, ms);

                List<List<int>> ways = new List<List<int>>();
                List<List<int>> uniqueWays = new List<List<int>>();
                FindMovements(begin, ms, ways, uniqueWays);
                Console.Write(graph); 
                /*foreach (var element in path)
                {

                    Console.WriteLine("+");
                    Console.WriteLine(element.To);
                    Console.WriteLine(element.From);
                    Console.WriteLine("+");
                }*/
                //allDFS(graph, 6);
                Console.ReadKey();
                //Console.WriteLine("\nГотово!");
                Console.ReadKey();
            }
            catch (Exception exp)
            {
                Console.WriteLine(exp.ToString());
            }
        }

        /// <summary>
        /// Печать результатов
        /// </summary>
        /// <param name="ms">Движения между отделениями</param>
        /// <param name="begin">Первое отделение (откуда старт)</param>
        /// <param name="result">Результат выполнения функции FindMovements()</param>
        static void PrintResults(Movement[] ms, string begin, List<int[]> result)
        {
            Console.WriteLine(result.Count);
            //return;


            if (result == null || result.Count == 0)
            {
                Console.WriteLine("Переходов не найдено.");
            }
            else
            {
                Console.WriteLine("Найденные переходы:");

                foreach (int[] list in result)
                {
                    Movement prev = null;
                    for (int i = 0; i < ms.Length; i++)
                    {
                        if (i >= list.Length)
                        {
                            Console.Write("Ошибка: отсутствует переход.");
                        }
                        else
                        {
                            int idx = list[i];
                            if (idx < 0 || idx >= ms.Length)
                            {
                                Console.Write("Ошибка: индекс за пределами диапазона.");
                            }
                            else
                            {
                                Movement movement = ms[idx];
                                //Console.Write(movement.From);
                                //Console.Write(" -> ");
                                //Console.Write(movement.To);

                                /*string b = prev == null ? begin : prev.To;

                                // проверка с предыдущим
                                if (movement.From != b)
                                {
                                    Console.Write(" ОШИБКА!");
                                }
                                prev = movement;*/
                            }
                        }
                        //Console.WriteLine();
                    }
                    if (list.Length > ms.Length)
                    {
                        Console.WriteLine("Ошибка: имеются лишние переходы.");
                    }

                    //Console.WriteLine();
                }
            }
        }

        /// <summary>
        /// Поиск всех вариантов движения, начиная с указанного отделения.
        /// </summary>
        /// <param name="firstDivision">Первое отделение (откуда старт)</param>
        /// <param name="ms">Движения между отделениями</param>
        /// <returns>Результат в виде списка индексов переходов между отделениями в исходном массиве</returns>
        static List<Movement> FindMovements(int firstDivision, Movement[] ms, List<List<int>> ways, List<List<int>> uniqueWays)
        {
            var counter = 0;

            var MyStack = new Stack<Movement>();
            var startedge = (ms.Where(edge => edge.From == firstDivision)).ToArray();
            Random random = new Random();
            int start2 = random.Next(0, startedge.Length);

            MyStack.Push(startedge[start2]); 
             
            List<int> sublist = new List<int>();
            while (MyStack.Count > 0 && counter != 29)
            {
                var edge = MyStack.Pop();

                sublist.Add(edge.index);
                counter++;
                edge.visited = true;
                var rnd = new Random();
                var adj = ms.Where(mov => mov.From == edge.To).OrderBy(x => rnd.Next()).ToList();
               // var shuffledList = adj.OrderBy(x => rnd.Next()).ToList();


                foreach (var neighbor in adj)
                {
                    if (neighbor.visited != true)
                    { 
                        MyStack.Push(neighbor);
                    }
                    else if (adj.Count() == 0 || adj.All(u => u.visited == true))
                    {
                        //var lastItem = ways[0].LastOrDefault();
                        //ways[0].RemoveAt(ways[0].Count()-1);  
                        // Console.WriteLine("out of options");
                        var adj1 = ms.Where(mov => mov.To == edge.From);
                        foreach (var neighbor1 in adj1)
                        { 
                            neighbor1.visited = false;
                        } 
                    }
                }
            }
            if (counter == 29)
            {

                if (!ways.Contains(sublist) && ways.Distinct().ToList().Count < 1944)
                { 
                    ways.Add(sublist); 
                     
                    foreach (var sub in ms)
                    {
                        sub.visited = false;
                    }
                     
                    var a =  ways.Distinct().ToList().Count;
                    foreach (var sub in ways.Distinct().ToList())
                    {  
                        bool isUnique = sub.Distinct().Count() == sub.Count();
                        if (isUnique == true)
                        {
                            foreach (var value in sub)
                            {
                                Console.WriteLine("unique ways number: "+ a);
                                //Console.Write(value);
                            }
                        }
                    }
                    // Console.WriteLine(ways.Where(x => x.Distinct().ToList().Count == x.Count).SelectMany(x => x).ToList()); 
                    FindMovements(firstDivision, ms, ways, uniqueWays);
                }
                 

            }
            return null;
        }

    }
}
