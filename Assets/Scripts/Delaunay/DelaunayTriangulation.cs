using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Sweep algorithm based on: http://paper.academicpub.org/Paper?id=15630
// Flip algorithm based on: https://people.eecs.berkeley.edu/~jrs/papers/meshbook/chapter2.pdf

namespace Delaunay
{
    public static class DelaunayTriangulation
    {
        public static List<Triangle> Triangulate2D(List<Vector2> pointsAsVec2)
        {
            List<Triangle> triangles = new List<Triangle>();   // list of all triangles in the DT
            List<HalfEdge> edgeStack = new List<HalfEdge>();   // list of all edges to be legalized
            LinkedList<HalfEdge> hull;                         // linked list of all points in convex hull (CCW)

            // sort points by x-value from low to high
            Quicksort(pointsAsVec2, 0, pointsAsVec2.Count - 1);

            // convert list of Vector2 points to list of Points
            List<Point> points = Vector2ToPoint(pointsAsVec2);

            // initialize first triangle with first three sorted points
            triangles.Add(ConstructTriangle(points[0], points[1], points[2]));

            // add edges of triangle to convex hull and edge stack
            hull = new LinkedList<HalfEdge>(triangles[0].Edges);
            edgeStack.AddRange(triangles[0].Edges);

            // triangulate new points one at a time
            for (int i = 3; i < points.Count; i++)
            {
                Point nextPoint = points[i];

                foreach (var edge in hull.ToList())
                {
                    if (pointIsRightOfEdge(edge, nextPoint))
                    {
                        Triangle nextTriangle = ConstructTriangle(edge.P, edge.Q, nextPoint);

                        // get base edge node in hull
                        LinkedListNode<HalfEdge> baseEdge = hull.Find(edge);

                        // check new edges of triangle:
                            // if newEdge.Reverse exists in hull, remove reverse edge from hull
                            // else add newEdge to hull
                        HalfEdge lowerEdge = nextTriangle.Edges[1];
                        HalfEdge upperEdge = nextTriangle.Edges[2];

                        // reverse of lower edge can only be precursor of base
                        if (baseEdge.Previous != null)
                        {
                            HalfEdge previous = baseEdge.Previous.Value;
                            if (previous == lowerEdge)
                            {
                                hull.Remove(baseEdge.Previous);
                            }
                            else
                            {
                                hull.AddBefore(baseEdge, lowerEdge);
                            }
                        }
                        else
                        {
                            hull.AddBefore(baseEdge, lowerEdge);
                        }

                        // reverse of upper edge can only be next from base
                        if (baseEdge.Next != null)
                        {
                            HalfEdge next = baseEdge.Next.Value;
                            if (next == upperEdge)
                            {
                                hull.Remove(baseEdge.Next);
                            }
                            else
                            {
                                hull.AddAfter(baseEdge, upperEdge);
                            }
                        }
                        else
                        {
                            hull.AddAfter(baseEdge, upperEdge);
                        }

                        // revove base edge node
                        hull.Remove(baseEdge);

                        // add triangle and its edges to lists
                        triangles.Add(nextTriangle);
                        edgeStack.AddRange(nextTriangle.Edges);

                        // legalize (flip) recursively
                        Legalize(edgeStack, triangles);
                    }
                }
            }

            return triangles;
        }

        public static void Legalize(List<HalfEdge> edgeStack, List<Triangle> triangles)
        {
            while (edgeStack.Count > 0)
            {
                foreach (var edge in edgeStack.ToList())
                {
                    edgeStack.Remove(edge);

                    List<Triangle> localTriangles = GetTrianglesFromEdge(edge, triangles);
                    bool edgeExists = localTriangles.Count > 0;

                    if (edgeExists)
                    {
                        bool illegal = notLegal(localTriangles);

                        if (illegal)
                        {
                            List<Triangle> newLocalTriangles = Flip(localTriangles);
                            triangles.AddRange(newLocalTriangles);

                            foreach (var t in localTriangles)
                            {
                                triangles.Remove(t);
                            }

                            edgeStack.AddRange(newLocalTriangles[0].Edges);
                            edgeStack.AddRange(newLocalTriangles[1].Edges);
                        }
                    }
                }
            }
        }

        public static List<Triangle> Flip(List<Triangle> twoTriangles)
        {
            if (twoTriangles.Count == 2)
            {
                List<Point> points = GetPointsOfTwoTriangles(twoTriangles);

                Point A = points[0];
                Point B = points[1];
                Point C = points[2];
                Point D = points[3];

                List<Triangle> newLocalTriangles = new List<Triangle>();

                newLocalTriangles.Add(ConstructTriangle(D, C, A));
                newLocalTriangles.Add(ConstructTriangle(C, D, B));

                return newLocalTriangles;
            }

            return new List<Triangle>();
        }

        /// <summary>
        /// returns true if set of triangles is illegal
        /// </summary>
        public static bool notLegal(List<Triangle> twoTriangles)
        {
            if (twoTriangles.Count == 2)
            {
                List<Point> points = GetPointsOfTwoTriangles(twoTriangles);

                Point A = points[0];
                Point B = points[1];
                Point C = points[2];
                Point D = points[3];

                Triangle first = ConstructTriangle(B, A, C);
                Triangle second = ConstructTriangle(A, B, D);

                bool isNotLegalFirst = pointInCircumcircle(first, D);
                bool isNotLegalSecond = pointInCircumcircle(second, C);

                return (isNotLegalFirst || isNotLegalSecond);
            }

            return false;
        }

        public static List<Point> GetPointsOfTwoTriangles(List<Triangle> twoTriangles)
        {
            if (twoTriangles.Count == 2)
            {
                Point A = new Point();
                Point B = new Point();
                Point C = new Point();
                Point D = new Point();

                // find shared half edge
                foreach (var e in twoTriangles[0].Edges)
                {
                    foreach (var f in twoTriangles[1].Edges)
                    {
                        if (e == f)
                        {
                            A = e.P;
                            B = e.Q;
                        }
                    }
                }

                // find first unique point
                foreach (var p in twoTriangles[0].Points)
                {
                    if (p != A && p != B)
                        C = p;
                }

                // find second unique point
                foreach (var p in twoTriangles[1].Points)
                {
                    if (p != A && p != B)
                        D = p;
                }

                return new List<Point> { A, B, C, D };
            }

            return new List<Point>();
        }

        public static bool pointInCircumcircle(Triangle triangle, Point P)
        {
            var ax = triangle.Points[0].X;
            var ay = triangle.Points[0].Y;
            var bx = triangle.Points[1].X;
            var by = triangle.Points[1].Y;
            var cx = triangle.Points[2].X;
            var cy = triangle.Points[2].Y;

            var px = P.X;
            var py = P.Y;

            var dx = ax - px;
            var dy = ay - py;
            var ex = bx - px;
            var ey = by - py;
            var fx = cx - px;
            var fy = cy - py;

            var ag = dx*dx + dy*dy;
            var bg = ex*ex + ey*ey;
            var cg = fx*fx + fy*fy;

            var det = dx * (ey*cg - fy*bg) -
                      dy * (ex*cg - fx*bg) +
                      ag * (ex*fy - fx*ey);

            return det > 0;
        }

        public static Vector2 GetCircumcenter(Triangle triangle)
        {
            var ax = triangle.Points[0].X;
            var ay = triangle.Points[0].Y;
            var bx = triangle.Points[1].X;
            var by = triangle.Points[1].Y;
            var cx = triangle.Points[2].X;
            var cy = triangle.Points[2].Y;

            var a2 = ax*ax + ay*ay;
            var b2 = bx*bx + by*by;
            var c2 = cx*cx + cy*cy;

            var d = 2 * (ax * (by - cy) + bx * (cy - ay) + cx * (ay - by));

            var ux = (a2 * (by - cy) + b2 * (cy - ay) + c2 * (ay - by)) / d;
            var uy = (a2 * (cx - bx) + b2 * (ax - cx) + c2 * (bx - ax)) / d;

            return new Vector2(ux, uy);
        }

        public static float GetCircumradius(Triangle triangle)
        {
            Vector2 center = GetCircumcenter(triangle);
            Vector2 point = PointToVec2(triangle.Points[0]);

            float dx = (point.x - center.x) * (point.x - center.x);
            float dy = (point.y - center.y) * (point.y - center.y);

            return Mathf.Sqrt(dx + dy);
        }

        public static List<Triangle> GetTrianglesFromEdge(HalfEdge edge, List<Triangle> triangles)
        {
            List<Triangle> tris = new List<Triangle>();

            foreach (var t in triangles)
            {
                foreach (var e in t.Edges)
                {
                    if (e.Equals(edge) || e.Equals(ReverseEdge(edge)))
                    {
                        tris.Add(t);
                    }
                }
            }

            return tris;
        }

        public static bool edgeExistsInDT(HalfEdge edge, List<Triangle> triangles)
        {
            return GetTrianglesFromEdge(edge, triangles).Count > 0;
        }

        public static Triangle ConstructTriangle(Point A, Point B, Point C)
        {
            if (isOnRightSide(A, B, C))
            {
                List<Point> trianglePoints = new List<Point> { B, A, C };
                return new Triangle(trianglePoints);
            }
            else
            {
                List<Point> trianglePoints = new List<Point> { A, B, C };
                return new Triangle(trianglePoints);
            }
        }

        public static HalfEdge ReverseEdge(HalfEdge edge)
        {
            return new HalfEdge(edge.Q, edge.P);
        }

        public static bool pointIsRightOfEdge(HalfEdge edge, Point test)
        {
            return isOnRightSide(edge.P, edge.Q, test);
        }

        public static bool isOnRightSide(Point a, Point b, Point test)
        {
            float crossProduct = (test.X - a.X) * (b.Y - a.Y) - (b.X - a.X) * (test.Y - a.Y);
            return crossProduct > 0;
        }

        public static void Quicksort(List<Vector2> pointSet, int low, int high)
        {
            if (low >= high || low < 0 || high < 0)
                return;
            
            int p = Partition(pointSet, low, high);

            Quicksort(pointSet, low, p - 1);
            Quicksort(pointSet, p + 1, high);
        }

        private static int Partition(List<Vector2> pointSet, int low, int high)
        {
            Vector2 pivot = pointSet[high];

            int i = low - 1;

            for (int j = low; j < high; j++)
            {
                if (j >= 0)
                {
                    if (pointSet[j].x <= pivot.x)
                    {
                        i++;

                        Vector2 temp1 = pointSet[i];
                        pointSet[i] = pointSet[j];
                        pointSet[j] = temp1;
                    }
                }
            }

            i++;

            Vector2 temp2 = pointSet[i];
            pointSet[i] = pointSet[high];
            pointSet[high] = temp2;

            return i;
        }

        public static List<Point> Vector2ToPoint(List<Vector2> pointSet)
        {
            List<Point> points = new List<Point>();

            for (int i = 0; i < pointSet.Count; i++)
            {
                Point point = new Point(pointSet[i].x, pointSet[i].y);
                points.Add(point);
            }

            return points;
        }

        public static Vector2 PointToVec2(Point p)
        {
            return new Vector2(p.X, p.Y);
        }
    }
}