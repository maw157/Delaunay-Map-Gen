using System.Collections.Generic;

namespace Delaunay
{
    public struct Triangle
    {
        public List<Point> Points { get; private set; }
        public List<HalfEdge> Edges { get; private set; }

        public Triangle(List<Point> points)
        {
            Points = points;

            Edges = new List<HalfEdge>
            {
                new HalfEdge(points[0], points[1]),
                new HalfEdge(points[1], points[2]),
                new HalfEdge(points[2], points[0])
            };
        }

        public override string ToString()
        {
            return $"{Edges[0].ToString()}, {Edges[1].ToString()}, {Edges[2].ToString()}";
        }

        public override bool Equals(object obj)
        {
            if ((obj == null) || !this.GetType().Equals(obj.GetType()))
            {
                return false;
            }
            else
            {
                Triangle t = (Triangle) obj;

                HalfEdge AB = this.Edges[0];
                HalfEdge BC = this.Edges[1];
                HalfEdge CA = this.Edges[2];

                bool containsAB = t.Edges.Contains(AB) || t.Edges.Contains(AB.Reverse());
                bool containsBC = t.Edges.Contains(BC) || t.Edges.Contains(BC.Reverse());
                bool containsCA = t.Edges.Contains(CA) || t.Edges.Contains(CA.Reverse());
                
                return (containsAB && containsBC && containsCA);
            }
        }

        public static bool operator ==(Triangle left, Triangle right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Triangle left, Triangle right)
        {
            return !left.Equals(right);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}