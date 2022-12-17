namespace Delaunay
{
    public struct HalfEdge
    {
        public Point P { get; private set; }
        public Point Q { get; private set; }

        public HalfEdge(Point p, Point q)
        {
            P = p;
            Q = q;
        }

        public HalfEdge Reverse()
        {
            return new HalfEdge(this.Q, this.P);
        }

        public override string ToString()
        {
            return $"{this.P.ToString()}-->{this.Q.ToString()}";
        }

        public override bool Equals(object obj)
        {
            if ((obj == null) || !this.GetType().Equals(obj.GetType()))
            {
                return false;
            }
            else
            {
                HalfEdge h = (HalfEdge) obj;
                bool p = (this.P.Equals(h.P)) || (this.P.Equals(h.Q));
                bool q = (this.Q.Equals(h.Q)) || (this.Q.Equals(h.P));
                return (p && q);
            }
        }

        public static bool operator ==(HalfEdge left, HalfEdge right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(HalfEdge left, HalfEdge right)
        {
            return !left.Equals(right);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}