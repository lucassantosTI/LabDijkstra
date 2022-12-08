using SpecializedStructs.Core.Heap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpecializedStructs.Core.Graph
{
    public class GraphArc<T> : IHeapLeafKey
    {
        public GraphVertex<T> Source { get; }
        public GraphVertex<T> Target { get; }
        public long Distance { get; set; }

        public GraphArc(GraphVertex<T>? source, GraphVertex<T>? target, long distance)
        {
            if (source == null || source.Value.IsEmpty()) throw new ArgumentNullException(nameof(source));
            if (target == null || target.Value.IsEmpty()) throw new ArgumentNullException(nameof(target));

            this.Source = source.Value;
            this.Target = target.Value;
            this.Distance = distance;
        }

        public override string ToString() => $"{this.Source} -> {this.Target}";
        public override int GetHashCode() => this.ToString().GetHashCode();
        public override bool Equals(object? obj)
        {
            if (obj is null || obj is not GraphArc<T>) return false;

            return obj.GetHashCode() == this.GetHashCode();
        }

        public long GetKey() => this.Distance;
    }
}
