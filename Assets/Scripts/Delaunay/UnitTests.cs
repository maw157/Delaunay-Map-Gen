using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Delaunay;

public class UnitTests : MonoBehaviour
{
    static Point A = new Point(0,0);
    static Point B = new Point(2,4);
    static Point C = new Point(4,1);
    static Point D = new Point(0,3);
    static Point E = new Point(7,5);

    static List<Point> testPoints = new List<Point> { A, B, C, D, E };

    static HalfEdge AB = new HalfEdge(A, B);
    static HalfEdge BC = new HalfEdge(B, C);

    static Triangle BAC = DelaunayTriangulation.ConstructTriangle(A, B, C);
    static Triangle ABD = DelaunayTriangulation.ConstructTriangle(A, B, D);
    static Triangle BCE = DelaunayTriangulation.ConstructTriangle(B, C, E);
    static Triangle CDA = DelaunayTriangulation.ConstructTriangle(C, D, A);
    static Triangle DCB = DelaunayTriangulation.ConstructTriangle(C, D, B);

    static List<Triangle> testTriangles = new List<Triangle> { BAC, ABD, BCE };
    static List<Vector2> testPointsAsVec2 = new List<Vector2> { new Vector2(4,1), new Vector2(2,4), new Vector2(0,3), new Vector2(7,5), new Vector2(0,0) };

    [SerializeField] GameObject _drawCirclePrefab;
    [SerializeField] GameObject _parent;
    private List<Vector2> _drawnPoints = new List<Vector2>();
    private List<HalfEdge> _drawnEdges = new List<HalfEdge>();
    private List<DrawCircle> _drawnCircles = new List<DrawCircle>();

    [Header("Display Settings")]
    [SerializeField] float _pointRadius = 0.03f;
    [SerializeField] float _lineWidth = 0.02f;
    [SerializeField] [Range(6,60)] public int _lineCount = 25;

    // data structure tests

    public void Test_TriangleEquals() // passed
    {
        print("Testing Triangle equals methods:");
        print("BAC == CBA: " + (BAC == DelaunayTriangulation.ConstructTriangle(C, B, A)));
        print("BAC != CBA: " + (BAC != DelaunayTriangulation.ConstructTriangle(C, B, A)));
        print("BAC == ABD: " + (BAC == ABD));
        print("BAC != ABD: " + (BAC != ABD));
    }

    public void Test_EdgeEquals() // passed
    {
        print("Testing HalfEdge equals methods:");
        print("AB == AB: " + (AB == new HalfEdge(A, B)));
        print("AB != AB: " + (AB != new HalfEdge(A, B)));
        print("AB == BC: " + (AB == BC));
        print("AB != BC: " + (AB != BC));
        print("AB == BA: " + (AB == AB.Reverse()));
    }

    public void Test_PointEquals() // passed
    {
        print("Testing Point equals methods:");
        print("A == A: " + (A == new Point(0,0)));
        print("A == B: " + (A == B));
        print("A != B: " + (A != B));
        print("A != A: " + (A != new Point(0,0)));
    }

    // helper method tests

    public void Test_Vector2ToPoint() // passed
    {
        print("Testing Vector2ToPoint:");
        PrintVecs(testPointsAsVec2);        
        
        List<Point> points = DelaunayTriangulation.Vector2ToPoint(testPointsAsVec2);
        PrintPoints(points);
    }

    public void Test_Quicksort() // passed
    {
        List<Vector2> sorted = new List<Vector2>(testPointsAsVec2);
        DelaunayTriangulation.Quicksort(sorted, 0, sorted.Count - 1);
        print("Testing Quicksort:");
        PrintVecs(testPointsAsVec2);
        PrintVecs(sorted);
    }

    public void Test_isOnRightSide() // passed
    {
        print("Testing isOnRightSide:");
        print("Point C is right of AB (true): " + DelaunayTriangulation.isOnRightSide(A, B, C));
        print("Point C is right of BA (false): " + DelaunayTriangulation.isOnRightSide(B, A, C));
        print("Point D is right of AB (false): " + DelaunayTriangulation.isOnRightSide(A, B, D));
        print("Point D is right of BA (true): " + DelaunayTriangulation.isOnRightSide(B, A, D));
    }

    public void Test_pointIsRightOfEdge() // passed
    {
        print("Testing pointIsRightOfEdge:");
        print("Point C is right of AB (true): " + DelaunayTriangulation.pointIsRightOfEdge(AB, C));
        print("Point C is right of BA (false): " + DelaunayTriangulation.pointIsRightOfEdge(AB.Reverse(), C));
        print("Point D is right of AB (false): " + DelaunayTriangulation.pointIsRightOfEdge(AB, D));
        print("Point D is right of BA (true): " + DelaunayTriangulation.pointIsRightOfEdge(AB.Reverse(), D));
    }

    public void Test_ReverseEdge() // passed
    {
        print("Testing ReverseEdge:");
        print("Initial half edge: " + AB.ToString());
        print("DT.ReverseEdge: " + DelaunayTriangulation.ReverseEdge(AB).ToString());
        print("HalfEdge.Reverse: " + AB.Reverse().ToString());
    }

    public void Test_ConstructTriangle() // passed
    {
        print("Testing CreateTriangle:");
        PrintPoints(testPoints);
        print("Triangle [BA, AC, CB]: " + DelaunayTriangulation.ConstructTriangle(A, B, C));
        print("Triangle [AB, BD, DA]: " + DelaunayTriangulation.ConstructTriangle(A, B, D));
    }

    public void Test_pointInCircumcircle() // passed
    {
        _drawnPoints.Clear();
        _drawnEdges.Clear();
        ClearCircles();

        print("Testing pointInCircumcircle:");
        print("Point (2, 2) is in circumcircle (true): " + DelaunayTriangulation.pointInCircumcircle(BAC, new Point(2, 2)));
        print("Point (10, 10) is in circumcircle (false): " + DelaunayTriangulation.pointInCircumcircle(BAC, new Point(5, 5)));

        _drawnPoints.Add(new Vector2(2, 2));
        _drawnPoints.Add(new Vector2(5, 5));
        Draw(BAC);
    }

    public void Test_GetCircumcenter() // passed-ish... rounding errors
    {
        print("Testing GetCircumcenter:");
        print("Given equilateral triangle centered on the origin...");
        print("Known circumcenter: (0, 0)");

        float root3 = Mathf.Sqrt(3);
        float sin30 = Mathf.Sin(30 * Mathf.PI / 180);
        List<Point> pts = new List<Point> { new Point(0, root3/3),
                                            new Point(-0.5f, -sin30),
                                            new Point(0.5f, -sin30) };
        Triangle test = new Triangle(pts);

        Vector2 calculated = DelaunayTriangulation.GetCircumcenter(test);

        print($"Calculated circumcenter: " + calculated.ToString());
    }

    public void Test_GetCircumradius() // passed-ish... more rounding errors
    {
        print("Testing GetCircumradius:");
        print("Given equilateral triangle centered on the origin...");
        print("Known radius: sqrt(3)/3 [~0.58]");

        float root3 = Mathf.Sqrt(3);
        float sin30 = Mathf.Sin(30 * Mathf.PI / 180);
        List<Point> pts = new List<Point> { new Point(0, root3/3),
                                            new Point(-0.5f, -sin30),
                                            new Point(0.5f, -sin30) };
        Triangle test = new Triangle(pts);

        print($"Calculated radius: " + DelaunayTriangulation.GetCircumradius(test));
    }

    public void Test_GetTrianglesFromEdge() // passed
    {
        print("Testing GetTrianglesFromEdge:");

        print("HalfEdge AB: " + AB.ToString());
        List<Triangle> ABTriangles = DelaunayTriangulation.GetTrianglesFromEdge(AB, testTriangles);
        print("Triangles containing AB:");
        foreach (var t in ABTriangles)
        {
            print(t.ToString());
        }

        print("HalfEdge BC: " + BC.ToString());
        List<Triangle> BCTriangles = DelaunayTriangulation.GetTrianglesFromEdge(BC, testTriangles);
        print("Triangles containing BC:");
        foreach (var t in BCTriangles)
        {
            print(t.ToString());
        }
    }

    public void Test_edgeExistsInDT() // passed
    {
        print("Testing edgeExistsInDT:");
        print("Edge DB exists in DT [true]: " + DelaunayTriangulation.edgeExistsInDT(new HalfEdge(D, B), testTriangles));
        print("Edge XX exists in DT [false]: " + DelaunayTriangulation.edgeExistsInDT(new HalfEdge(A, new Point(100,100)), testTriangles));
    }

    public void Test_notLegal() // passed
    {
        print("Testing notLegal:");
        print("Triangles BAC and ABD are illegal [true]: " + DelaunayTriangulation.notLegal(new List<Triangle> { BAC, ABD }));
        print("Triangles CDA and DCB are illegal [false]: " + DelaunayTriangulation.notLegal(new List<Triangle> { CDA, DCB }));
    }

    // core algorithm tests
    
    public void Test_Flip() // passed
    {
        print("Testing Flip:");

        List<Triangle> localTriangles = new List<Triangle> { BAC, ABD };
        List<Triangle> newLocalTriangles = new List<Triangle>();

        Triangle DCB = DelaunayTriangulation.ConstructTriangle(D, C, B);
        Triangle CDA = DelaunayTriangulation.ConstructTriangle(C, D, A);

        print("Triangle BAC: " + BAC.ToString());
        print("Triangle ABD: " + ABD.ToString());

        newLocalTriangles = DelaunayTriangulation.Flip(localTriangles);

        foreach (var t in newLocalTriangles)
        {
            print("New triangle: " + t.ToString());
        }

        bool out1 = (newLocalTriangles[0] == DCB || newLocalTriangles[1] == DCB);
        bool out2 = (newLocalTriangles[0] == CDA || newLocalTriangles[1] == CDA);

        print("New triangles match expected output: " + (out2 && out2));
    }

    public void Test_Legalize() // passed
    {
        print("Testing Legalize:");

        List<Triangle> localTriangles = new List<Triangle> { BAC, ABD };
        List<HalfEdge> edgeStack = new List<HalfEdge>();

        Triangle DCB = DelaunayTriangulation.ConstructTriangle(D, C, B);
        Triangle CDA = DelaunayTriangulation.ConstructTriangle(C, D, A);

        edgeStack.AddRange(localTriangles[0].Edges);
        edgeStack.AddRange(localTriangles[1].Edges);

        foreach (var t in localTriangles)
        {
            print("Triangle: " + t.ToString());
        }

        print("Stack length: " + edgeStack.Count);

        DelaunayTriangulation.Legalize(edgeStack, localTriangles);

        print("Stack length: " + edgeStack.Count);
        
        foreach (var t in localTriangles)
        {
            print("Triangle: " + t.ToString());
        }

        bool out1 = (localTriangles[0] == DCB || localTriangles[1] == DCB);
        bool out2 = (localTriangles[0] == CDA || localTriangles[1] == CDA);

        print("New triangles match expected output: " + (out2 && out2));
    }

    public void Test_Triangulate2DVerbose() // passed
    {
        print("Begin triangulation...");

        List<Triangle> triangles = new List<Triangle>();   // list of all triangles in the DT
        List<HalfEdge> edgeStack = new List<HalfEdge>();   // list of all edges to be legalized
        LinkedList<HalfEdge> hull;                         // linked list of all points in convex hull (CCW)

        print("Sorting...");
        // sort points by x-value from low to high
        DelaunayTriangulation.Quicksort(testPointsAsVec2, 0, testPointsAsVec2.Count - 1);

        print("Converting Vector2 points to type Point...");
        // convert list of Vector2 points to list of Points
        List<Point> points = DelaunayTriangulation.Vector2ToPoint(testPointsAsVec2);

        PrintPoints(points);

        print("Initializing first triangle...");
        // initialize first triangle with first three sorted points
        triangles.Add(DelaunayTriangulation.ConstructTriangle(points[0], points[1], points[2]));
        foreach (var t in triangles)
        {
            print("Triangle: " + t.ToString());
        }
        
        print("Add edges of first triangle to hull and edge stack");
        // add edges of triangle to convex hull and edge stack
        hull = new LinkedList<HalfEdge>(triangles[0].Edges);
        edgeStack.AddRange(triangles[0].Edges);

        foreach (var e in hull)
        {
            print("Hull edge: " + e.ToString());
        }

        foreach (var e in edgeStack)
        {
            print("Edge stack edge: " + e.ToString());
        }

        print("Triangulating new points...");
        // triangulate new points one at a time
        for (int i = 3; i < points.Count; i++)
        {
            Point nextPoint = points[i];

            print("i = " + i);
            print("(for loop) edges in hull = " + hull.ToList().Count);

            foreach (var edge in hull.ToList())
            {
                print("(foreach loop) edges in hull = " + hull.ToList().Count);

                if (DelaunayTriangulation.pointIsRightOfEdge(edge, nextPoint))
                {
                    print($"Adding point {nextPoint.ToString()} to triangulation...");
                    Triangle nextTriangle = DelaunayTriangulation.ConstructTriangle(edge.P, edge.Q, nextPoint);

                    print("Next triangle: " + nextTriangle.ToString() + "...is right of edge: " + edge.ToString());

                    // get base edge node in hull
                    LinkedListNode<HalfEdge> baseEdge = hull.Find(edge);
                    
                    if (edge != null)
                        print($"Located edge {edge.ToString()} in hull");
                    else
                        print($"Edge {edge.ToString()} not found in hull");

                    // check new edges of triangle:
                        // if ReverseEdge(new edge) exists in hull, remove reverse edge from hull
                        // else add new edge to hull
                    HalfEdge lowerEdge = nextTriangle.Edges[1];
                    HalfEdge upperEdge = nextTriangle.Edges[2];

                    print("Lower edge: " + lowerEdge.ToString());
                    print("Upper edge: " + upperEdge.ToString());

                    // reverse of lower edge can only be precursor of base
                    if (baseEdge.Previous != null)
                    {
                        HalfEdge previous = baseEdge.Previous.Value;
                        if (previous == lowerEdge)
                        {
                            print("Removing previous edge...");
                            hull.Remove(baseEdge.Previous);
                            PrintEdges(hull.ToList());
                        }
                        else
                        {
                            print("Adding new edge before base...");
                            hull.AddBefore(baseEdge, lowerEdge);
                            PrintEdges(hull.ToList());
                        }
                    }
                    else
                    {
                        print("Adding new edge before base...");
                        hull.AddBefore(baseEdge, lowerEdge);
                        PrintEdges(hull.ToList());
                    }

                    // reverse of upper edge can only be next from base
                    if (baseEdge.Next != null)
                    {
                        HalfEdge next = baseEdge.Next.Value;
                        if (next == upperEdge)
                        {
                            print("Removing next edge...");
                            hull.Remove(baseEdge.Next);
                            PrintEdges(hull.ToList());
                        }
                        else
                        {
                            print("Adding new edge after base...");
                            hull.AddAfter(baseEdge, upperEdge);
                            PrintEdges(hull.ToList());
                        }
                    }
                    else
                    {
                        print("Adding new edge after base...");
                        hull.AddAfter(baseEdge, upperEdge);
                        PrintEdges(hull.ToList());
                    }

                    // remove base edge node
                    print("Removing base edge from hull...");
                    hull.Remove(baseEdge);

                    // add triangle and its edges to lists
                    print("Adding new triangle to triangle list...");
                    triangles.Add(nextTriangle);
                    edgeStack.AddRange(nextTriangle.Edges);

                    print("Triangle list:");
                    foreach (var t in triangles)
                    {
                        print("Triangle: " + t.ToString());
                    }

                    print("Edge stack:");
                    PrintEdges(edgeStack);

                    print("Convex hull:");
                    PrintEdges(hull.ToList());

                    // legalize (flip) recursively
                    print("Legalizing new edges...");
                    // DelaunayTriangulation.Legalize(edgeStack, triangles);

                    // the following code is from DT.Legalize(edgeStack, triangles)
                    int j = 0;
                    while (edgeStack.Count > 0)
                    {
                        print("While edgeStack.Count > 0: loop iteration number " + j);
                        print($"Edge stack contains {edgeStack.Count} edges...");
                        foreach (var e in edgeStack.ToList())
                        {
                            print($"Removing edge {e.ToString()} from stack...");
                            edgeStack.Remove(e);

                            print($"Finding triangles with edge {e.ToString()}...");
                            List<Triangle> localTriangles = DelaunayTriangulation.GetTrianglesFromEdge(e, triangles);
                            bool edgeExists = localTriangles.Count > 0;

                            print("Found triangles: " + edgeExists);
                            foreach (var t in localTriangles)
                            {
                                print("Found triangle: " + t.ToString());
                            }

                            if (edgeExists)
                            {
                                bool illegal = DelaunayTriangulation.notLegal(localTriangles);
                                print("Edge is not legal: " + illegal);

                                if (illegal)
                                {
                                    List<Triangle> newLocalTriangles = DelaunayTriangulation.Flip(localTriangles);
                                    triangles.AddRange(newLocalTriangles);

                                    foreach (var t in newLocalTriangles)
                                    {
                                        print("Adding triangle: " + t.ToString());
                                    }

                                    foreach (var t in localTriangles)
                                    {
                                        triangles.Remove(t);
                                        print("Removing triangle: " + t.ToString());
                                    }

                                    edgeStack.AddRange(newLocalTriangles[0].Edges);
                                    edgeStack.AddRange(newLocalTriangles[1].Edges);
                                }
                            }
                        }
                    }
                }
            }
        }

        foreach (var t in triangles)
        {
            print("Triangle: " + t.ToString());
        }
    }

    public void Test_Triangulate2D() // passed
    {
        print("Testing Triangulate2D:");
        List<Triangle> DT = DelaunayTriangulation.Triangulate2D(testPointsAsVec2);
        foreach (var t in DT)
        {
            print("Triangle: " + t.ToString());
        }
    }

    // unity messages

    private void OnDrawGizmos()
    {
        if (_drawnEdges.Count > 0 && _drawnPoints.Count > 0)
        {   
            foreach (var e in _drawnEdges)
            {
                Gizmos.DrawLine(PointToVec2(e.P), PointToVec2(e.Q));
            }

            foreach (var p in _drawnPoints)
            {
                Gizmos.DrawSphere(p, _pointRadius);
            }
        }
    }
    
    // unit testing helper methods

    private void Draw(Triangle triangle)
    {
        _drawnPoints.AddRange(new List<Vector2> { PointToVec2(triangle.Points[0]),
                                                  PointToVec2(triangle.Points[1]),
                                                  PointToVec2(triangle.Points[2]) });
        _drawnEdges.AddRange(triangle.Edges);
        ClearCircles();
        
        Circumcircle circumcircle = new Circumcircle(triangle);
        Vector3 center = circumcircle.Center;
        float circumradius = circumcircle.Radius;

        DrawCircle temp = Instantiate(_drawCirclePrefab, center, Quaternion.identity, _parent.transform).GetComponent<DrawCircle>();
        _drawnCircles.Add(temp);

        temp.SetDrawParams(circumradius, _lineWidth, _lineCount, Color.magenta);
        temp.Draw();
    }

    public void PrintEdges(List<HalfEdge> edges)
    {
        string estr = "Edge list: ";
        foreach (var e in edges)
        {
            string s = $"{e.ToString()}, ";
            estr += s;
        }

        estr = estr.Remove(estr.Length - 2);
        print(estr);
    }

    public void PrintPoints(List<Point> points)
    {
        string pts = "Points list: ";
        foreach (var p in points)
        {
            string s = $"{p.ToString()}, ";
            pts += s;
        }

        pts = pts.Remove(pts.Length - 2);
        print(pts);
    }

    public void PrintVecs(List<Vector2> vectors)
    {
        string vecs = "Vector list: ";
        foreach (var v in vectors)
        {
            string s = $"{v.ToString()}, ";
            vecs += s;
        }

        vecs = vecs.Remove(vecs.Length - 2);
        print(vecs);
    }

    private Vector2 PointToVec2(Point p)
    {
        return new Vector2(p.X, p.Y);
    }

    public void ClearCircles()
    {
        if (_drawnCircles.Count > 0)
        {
            foreach (var c in _drawnCircles)
            {
                if (c.gameObject != null)
                    Destroy(c.gameObject);
            }

            _drawnCircles.Clear();
        }
    }
}