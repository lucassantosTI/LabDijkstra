using SpecializedStructs.Core.Graph;
using System.Xml.Linq;

namespace SpecializedStructs
{
    /// <summary>
    /// Struct to define a connected graph for <typeparamref name="T"/> instances.
    /// </summary>
    public class Graph<T> : IDisposable
    {
        #region Properties & Fields
        protected List<GraphVertex<T>> vertices;
        protected List<GraphArc<T>> arcs;
        public int Count => this.vertices?.Count ?? 0;
        public int ArcsCount => this.arcs?.Count ?? 0;
        #endregion

        #region Constructors
        /// <summary>
        /// Craetes a new instance of <see cref="Graph"/>.
        /// </summary>
        public Graph() : this(null, null)
        {
        }

        public Graph(IEnumerable<GraphVertex<T>>? vertices, IEnumerable<GraphArc<T>>? arcs)
        {
            if (vertices != null)
                this.vertices = new List<GraphVertex<T>>(vertices);
            else
                this.vertices = new List<GraphVertex<T>>();

            if (arcs != null)
                this.arcs = new List<GraphArc<T>>(arcs);
            else
                this.arcs = new List<GraphArc<T>>();
        }
        #endregion

        #region Methods
        /// <summary>
        /// Creats a new vertex using <see cref="GraphVertex{T}.GraphVertex(T)"/> and append it to this graph.
        /// </summary>
        /// <param name="data">Vertex data</param>
        public GraphVertex<T> CreateVertex(T data)
        {
            var vertex = new GraphVertex<T>(data);

            this.vertices.Add(vertex);

            return vertex;
        }

        /// <summary>
        /// Creats a new vertex usign <see cref="GraphVertex{T}.GraphVertex(string, T)"/> and append it to this graph.
        /// </summary>
        /// <param name="label">Label to describe the vertex.</param>
        /// <param name="data">vertex data</param>
        public GraphVertex<T> Createvertex(string label, T data)
        {
            if (this.HasVertex(data))
            {
                throw new ArgumentException("The data has ben added as vertex on this graph.");
            }

            var vertex = new GraphVertex<T>(label, data);

            this.vertices.Add(vertex);

            return vertex;
        }

        /// <summary>
        /// Checks if this graph has a vertex for the <paramref name="data"/> and, when it' not contains, creates a new vertex using <see cref="Createvertex(T)"/>.
        /// </summary>
        /// <returns>Returns a instance of vertex associated of the <paramref name="data"/>.</returns>
        public GraphVertex<T> CreatevertexIfNotExists(T data)
        {
            var vertex = this.GetVertex(data);

            if (vertex == GraphVertex<T>.Empty)
            {
                vertex = this.CreateVertex(data);
            }

            return vertex;
        }

        /// <summary>
        /// Checks if the <paramref name="data"/> already exists in this graph vertices.
        /// </summary>
        public bool HasVertex(T data) => this.GetVertex(data) == GraphVertex<T>.Empty;

        /// <summary>
        /// Gets the vertex associated with the <paramref name="data"/>.
        /// </summary>
        public GraphVertex<T> GetVertex(T data) => this.vertices.FirstOrDefault(vertex => vertex.Data?.Equals(data) ?? false);

        /// <summary>
        /// Checks the <paramref name="source"/> vertex has an arch connecting it directaly to the <paramref name="target"/>.
        /// </summary>
        public bool IsConnected(GraphVertex<T> source, GraphVertex<T> target) =>
            this.arcs.Any(i => i.Source == source && i.Target == target);

        /// <summary>
        /// Connect <paramref name="source"/> to <paramref name="target"/>.
        /// </summary>
        public bool Connect(GraphVertex<T> source, GraphVertex<T> target, long distance, bool ignoreCircularReference = false)
        {
            if (this.vertices.Contains(source) == false) throw new ArgumentException("The parent vertex don't attached on this graph.");
            else if (this.vertices.Contains(target) == false) throw new ArgumentException("The target vertex don't attached on this graph.");
            else if (this.IsConnected(source, target)) return false;
            else if (ignoreCircularReference == false && this.IsConnected(target, source))
                throw new OverflowException($"The vertex '{source.Label}' already has a arc origining in '{target.Label}'.");

            this.arcs.Add(new GraphArc<T>(source, target, distance));

            return true;
        }

        /// <summary>
        /// Gets the arcs originated on <paramref name="vertex"/>.
        /// </summary>
        public List<GraphArc<T>> GetConnections(GraphVertex<T> vertex) => this.arcs.Where(i => i.Source == vertex).ToList();

        /// <inheritdoc/>
        public void Dispose()
        {
            this.vertices.Clear();
            this.vertices.TrimExcess();
            this.vertices = null;
        }
        #endregion
    }
}
