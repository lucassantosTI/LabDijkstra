using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.Security;
using System.Text;
using System.Threading.Tasks;

namespace SpecializedStructs.Core.Graph
{
    /// <summary>
    /// Struct to define a node of a <see cref="Graph{T}"/> instance, that contains the data for <typeparamref name="T"/> instance and a list with connected nodes to this node.
    /// </summary>
    public struct GraphVertex<T>
    {
        #region Constants
        /// <summary>
        /// Empty instance of <see cref="GraphVertex{T}"/>
        /// </summary>
        public static GraphVertex<T> Empty = new();
        #endregion

        #region Properties & Fields
        /// <summary>
        /// The data, of type <typeparamref name="T"/>, associated to this node.
        /// </summary>
        public T? Data { get; }
        /// <summary>
        /// The label to identify this node.
        /// </summary>
        public string Label { get; set; }
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new instance of <see cref="GraphVertex{T}"/> when the label is defined by the <paramref name="data"/>.ToString() value.
        /// </summary>
        public GraphVertex(T data) :
            this(data?.ToString(), data)
        {
        }

        /// <summary>
        /// Creats a new instance of <see cref="GraphVertex{T}"/> with the data and respective label to identify this node.
        /// </summary>
        public GraphVertex(string? label, T data)
        {
            if (string.IsNullOrWhiteSpace(label))
            {
                throw new ArgumentNullException(nameof(label));
            }

            this.Data = data ?? throw new ArgumentNullException(nameof(data));
            this.Label = label;
        }
        #endregion

        #region Methods
        ///// <summary>
        ///// Chekcs if the <paramref name="node"/> is connceted to this node.
        ///// </summary>
        //public bool IsConnected(GraphVertex<T> node) => this.Connections.Any(nd => nd.ConnectedTo?.Equals(node.Data) ?? false);
        //public void ConnectTo(GraphVertex<T> node, long distance, bool ignoreCircularReference = false)
        //{
        //    if (ignoreCircularReference == false && node.IsConnected(this)) throw new OverflowException($"O nó '{this.Label}' já está conectado ao nó '{node.Label}', não são permitidas conexões circulares.");
        //    else if (this.IsConnected(node)) return;

        //    var idxInsert = this.Connections.FindIndex(i => (i.Distance - distance) > 0);
        //    var connection = new NodeConnection<T>(node.Data, distance);

        //    if (idxInsert == -1)
        //        this.Connections.Add(connection);
        //    else
        //        this.Connections.Insert(idxInsert, connection);
        //}

        //public GraphVertex<T> ResetConnections(IEnumerable<GraphArc<T>>? newConnections)
        //{
        //    if (this.Connections != null)
        //    {
        //        this.Connections.Clear();
        //        this.Connections.TrimExcess();
        //    }

        //    if (newConnections != null)
        //        this.Connections = new List<GraphArc<T>>(newConnections.OrderBy(i => i.Distance));

        //    return this;
        //}

        /// <inheritdoc/>
        public override int GetHashCode() => this.Data?.GetHashCode() ?? -1;
        /// <inheritdoc/>
        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            if (obj == null || obj is not GraphVertex<T>) return false;

            return this.GetHashCode() == obj.GetHashCode();
        }
        /// <inheritdoc/>
        public override string ToString() => this.Label;

        /// <summary>
        /// Checks this object represents an empty instance of <see cref="GraphVertex{T}"/>.
        /// </summary>
        /// <returns></returns>
        public bool IsEmpty() => this == Empty;
        #endregion

        #region Operators
        /// <summary>
        /// Creates a new instance of <see cref="GraphVertex{T}"/> using the <paramref name="data"/>´as parameter for struct constructor <see cref="GraphVertex{T}.GraphVertex(T)"/>.
        /// </summary>
        public static explicit operator GraphVertex<T>(T data) => new GraphVertex<T>(data);

        /// <summary>
        /// Returns a value indicating the label and data of the <paramref name="left"/> are equal to the <paramref name="right"/>.
        /// </summary>
        public static bool operator ==(GraphVertex<T> left, GraphVertex<T> right) =>
            ((left.Label == null && right.Label == null) || left.Label.Equals(right.Label))
            && left.GetHashCode() == right.GetHashCode();
        /// <summary>
        /// Returns a value indicating the label and data of the <paramref name="left"/> are not equal to the <paramref name="right"/>.
        /// </summary>
        public static bool operator !=(GraphVertex<T> left, GraphVertex<T> right) => !(left == right);
        #endregion
    }
}
