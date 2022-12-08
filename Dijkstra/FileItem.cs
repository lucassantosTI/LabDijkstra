using SpecializedStructs;
using SpecializedStructs.Core.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dijkstra
{
    internal class Vertex
    {
        public Vertex(long vX, long vY, long distance)
        {
            this.Source = vX;
            this.Target = vY;
            this.Distance = distance;
        }

        public long Source { get; }
        public long Target { get; }
        public long Distance { get; }

        public static Vertex Create(string lineContent)
        {
            var columns = lineContent.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (columns.Length != 4) throw new Exception($"The line '{lineContent}' is not well formed.");

            long vX, vY, distance;

            if (!long.TryParse(columns[1], out vX)) throw new Exception($"The value of vertex X on line '{lineContent}' don't contains numeric value '{columns[1]}'.");
            if (!long.TryParse(columns[2], out vY)) throw new Exception($"The value of vertex Y on line ' {lineContent}' don't contains numeric value '{columns[2]}'.");
            if (!long.TryParse(columns[3], out distance)) throw new Exception($"The value of distance between X and Y on line '{lineContent}' don't contains numeric value '{columns[3]}'.");

            return new Vertex(vX, vY, distance);
        }
    }
}
