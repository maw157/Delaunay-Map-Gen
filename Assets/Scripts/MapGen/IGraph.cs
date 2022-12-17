using System.Collections.Generic;
using Delaunay;

// TODO: replace HalfEdge with IEdge (why?)

public interface IGraph
{
     public List<INode> AdjacencyList { get; }

     public void AddConnection(HalfEdge link)
     {
          AdjacencyList.Find(c => c.Position == link.P || c.Position == link.Q).AddLink(link);

          AdjacencyList.Find(c => c.Position == link.P)
               .AddNeighbor(AdjacencyList.Find(n => n.Position == link.Q));

          AdjacencyList.Find(c => c.Position == link.Q)
               .AddNeighbor(AdjacencyList.Find(n => n.Position == link.P));
     }

     public void RemoveConnection(HalfEdge link)
     {
          AdjacencyList.Find(c => c.Position == link.P).RemoveLink(link);
          
          AdjacencyList.Find(c => c.Position == link.P)
               .RemoveNeighbor(AdjacencyList.Find(n => n.Position == link.Q));

          AdjacencyList.Find(c => c.Position == link.Q)
               .RemoveNeighbor(AdjacencyList.Find(n => n.Position == link.P));
     }
}