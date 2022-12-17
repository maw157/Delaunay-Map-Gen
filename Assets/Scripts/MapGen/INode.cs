using System.Collections.Generic;
using Delaunay;

public interface INode
{
    public Point Position { get; }
    public List<INode> Neighbors { get; }
    public List<HalfEdge> Connections { get; }

    public void AddNeighbor(INode newNode)
    {
        if (Neighbors.Exists(n => n.Position == newNode.Position))
            return;
        
        Neighbors.Add(newNode);
    }

    public void RemoveNeighbor(INode node)
    {
        if (!Neighbors.Exists(n => n.Position == node.Position))
            return;
        
        Neighbors.Remove(node);
    }

    public void AddLink(HalfEdge newLink)
    {
        if (Connections.Exists(e => e == newLink))
            return;
        
        Connections.Add(newLink);
    }

    public void RemoveLink(HalfEdge link)
    {
        if (!Connections.Exists(e => e == link))
            return;
        
        Connections.Remove(link);
    }
}