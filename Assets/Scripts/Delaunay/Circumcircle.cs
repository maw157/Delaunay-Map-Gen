using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Delaunay
{    
    public struct Circumcircle
    {
        public Vector2 Center { get; private set; }
        public float Radius { get; private set; }

        public Circumcircle(Triangle triangle)
        {
            Center = DelaunayTriangulation.GetCircumcenter(triangle);
            Radius = DelaunayTriangulation.GetCircumradius(triangle);
        }
    }
}