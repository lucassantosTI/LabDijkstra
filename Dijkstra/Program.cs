using SpecializedStructs;
using SpecializedStructs.Core.Graph;
using System.Net.Http.Headers;
using System.Security.Cryptography;

namespace Dijkstra
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Gray;

            if (args?.Length != 3 && args?.Length != 1)
            {
                WriteHelp();
                Environment.Exit(1);
            }

            else if (args.Length == 1)
            {
                try
                {
                    Console.Write("Digite o número de iterações de testes: ");
                    var nTest = int.Parse(Console.ReadLine());
                    Console.Write("Digite a quantidade de repetições por iteração: ");
                    var nRepeat = int.Parse(Console.ReadLine());

                    int sourceVertex, targetVertex;
                    var random = new Random();

                    var pow = 1;
                    do
                    {
                        sourceVertex = (int)Math.Pow(2, pow);
                        targetVertex = sourceVertex + 1;

                        for (int i = 0; i < nRepeat; i++)
                        {
                            var startDate = DateTime.Now;
                            ComputeDijkstra(sourceVertex, targetVertex, args[0]);
                            Console.WriteLine("Caminho;{0};{1};iteração;{2};tempo;{3:N2}", sourceVertex, targetVertex, i, (DateTime.Now - startDate).TotalMilliseconds / 1000);
                        }

                        pow++;
                    } while (pow <= nTest);
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(ex.Message);
                    Environment.Exit(1);
                }
            }
            else
            {
                try
                {

                    int sourceVertex, targetVertex;

                    if (!int.TryParse(args[0], out sourceVertex)) throw new ArgumentException($"The source vertex is invalid '{args[0]}', only accepted numeric values.");
                    if (!int.TryParse(args[1], out targetVertex)) throw new ArgumentException($"The target vertex is invalid '{args[1]}', only accepted numeric values.");

                    var graph = CreateGraph(args[2]);

                    ComputeDijkstra(sourceVertex, targetVertex, graph);
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(ex.Message);
                    Environment.Exit(1);
                }
            }

            Environment.Exit(0);
        }

        static Graph<long> CreateGraph(string fileSource)
        {
            if (!File.Exists(fileSource)) throw new FileNotFoundException(fileSource);

            var fileText = File.ReadAllText(fileSource);
            if (string.IsNullOrWhiteSpace(fileText)) throw new Exception($"The file '{fileSource}' don't contains a DIMACS content.");

            var start = DateTime.Now;
            Console.WriteLine("Reading file lines ...");
            var fileVertices = (from line in fileText.Split(new string[] { Environment.NewLine, "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries)
                                where line.StartsWith("a", StringComparison.OrdinalIgnoreCase)
                                select Vertex.Create(line)).ToList();

            var vertices = (from vertex in fileVertices
                            group vertex by vertex.Source into grp
                            orderby grp.Key
                            select new GraphVertex<long>(grp.Key))?.ToList()
                             ?? throw new Exception("The file don't contains a DIMACS content.");

            var connections = (from vertex in vertices
                               join fv in fileVertices on vertex.Data equals fv.Source
                               join tVertex in vertices on fv.Target equals tVertex.Data
                               select new GraphArc<long>(vertex, tVertex, fv.Distance)).ToList();

            Console.WriteLine($"File readed in {(DateTime.Now - start).TotalMilliseconds:0}ms ...");


            return new Graph<long>(vertices, connections);
        }

        static void ComputeDijkstra(int sourceVertex, int targetVertex, string graphFile) => ComputeDijkstra(sourceVertex, targetVertex, CreateGraph(graphFile));

        static void ComputeDijkstra(int sourceVertex, int targetVertex, Graph<long> graph)
        {
            try
            {
                var start = DateTime.Now;
                Console.WriteLine($"Comput the shortest path on generated graph between {sourceVertex} and {targetVertex} vertex ...");
                try
                {
                    var dijkstra = graph.ComputeDijkstra(sourceVertex, targetVertex);

                    Console.WriteLine($"Dijkstra runs at {(DateTime.Now - start).TotalMilliseconds:0}ms ...");

                    if (dijkstra?.Count > 0)
                    {
                        Console.WriteLine("Min part of the path: {0}", dijkstra.Min(i => i.Distance));
                        Console.WriteLine("Vertex count of the shortest path: {0}", dijkstra.Count);
                        Console.WriteLine("Sum distance of the shortest path: {0}", dijkstra.Sum(i => i.Distance));
                    }
                }
                catch { }

                Console.WriteLine("inf");
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                Environment.Exit(1);
            }
        }

        static (long vX, long vY, long distance) GetColumns(string lineContent, int lineIndex)
        {
            var columns = lineContent.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (columns.Length != 4) throw new Exception($"The line {lineIndex} is not well formed, line value '{lineContent}'.");

            long vX, vY, distance;

            if (!long.TryParse(columns[1], out vX)) throw new Exception($"The value of vertex X on line {lineIndex} don't contains numeric value '{columns[1]}'.");
            if (!long.TryParse(columns[2], out vY)) throw new Exception($"The value of vertex Y on line {lineIndex} don't contains numeric value '{columns[2]}'.");
            if (!long.TryParse(columns[3], out distance)) throw new Exception($"The value of distance between X and Y on line {lineIndex} don't contains numeric value '{columns[3]}'.");

            return (vX, vY, distance);
        }

        static void WriteHelp()
        {
            Console.ForegroundColor = ConsoleColor.Gray;

            Console.WriteLine("Can't compute Dijkstra, incorrect parameters were provided, see the help below.");
            Console.WriteLine("\t./dijkstra [Source Vertex] [Destination Vertex] [DIMACS file path]");
            Console.WriteLine("\t\tSource Vertex -> the source point to calculate shortest path.");
            Console.WriteLine("\t\tDestination Vertex -> the destination point to calculate shortest path.");
            Console.WriteLine("\t\tDIMACS file path -> location of the file that has DIMACS format.");
        }
    }
}