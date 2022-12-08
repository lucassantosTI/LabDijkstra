using System.Runtime.CompilerServices;

namespace SpecializedStructs.Core.Graph
{
    public static class GraphExtensions
    {
        private const int MAX_COUNT = 1000;

        /// <summary>
        /// Compute the shortest path between two points on a graph using the dijkstra alghoritm.
        /// </summary>
        public static List<GraphArc<T>>? ComputeDijkstra<T>(this Graph<T> graph, T sourcePoint, T destPoint)
        {
            var source = graph.GetVertex(sourcePoint);
            if (source.IsEmpty()) throw new ArgumentException("Couldn't possible find the source point on the graph.");

            var dest = graph.GetVertex(destPoint);
            if (dest.IsEmpty()) throw new ArgumentException("Couldn't possible find the destination point on the graph.");

            return ComputeDijkstra<T>(graph, new HashSet<T>() { source.Data }, source, dest);
        }

        public static List<GraphArc<T>>? ComputeDijkstra<T>(Graph<T> graph, HashSet<T> visited, GraphVertex<T> source, GraphVertex<T> destination)
        {
            if (source.IsEmpty()) throw new ArgumentNullException(nameof(source));
            if (destination.IsEmpty()) throw new ArgumentNullException(nameof(destination));

            var connections = graph.GetConnections(source);
            if (connections == null || connections.Count == 0) return null;

            if (connections.Any(c => c.Target == destination))
            {
                return connections.Where(c => c.Target == destination).ToList();
            }


            var paths = new List<GraphArc<T>>();
            foreach (var item in connections)
            {
                if (visited.Contains(item.Target.Data))
                {
                    continue;
                }

                visited.Add(item.Target.Data);
                var pathsItem = ComputeDijkstra<T>(graph, visited, item.Target, destination);
                if (pathsItem?.Count > 0)
                {
                    paths.Add(item);
                    paths.AddRange(pathsItem);
                    break;
                }
                else if (visited.Count > MAX_COUNT)
                {
                    return null;
                }
            }

            return paths;
        }
    }
}
