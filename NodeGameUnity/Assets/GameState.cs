using System.Collections.Generic;
using System.Linq;

public record GameState
{
    public IReadOnlyList<NodeState> Nodes { get; init; }

    private Dictionary<string, NodeState> nodesById;

    public GameState(IReadOnlyList<NodeState> nodes)
    {
        Nodes = nodes;
        nodesById = nodes.ToDictionary(item => item.Identifier);
    }

    public NodeState GetNode(string id)
    {
        return nodesById[id];
    }

    public GameState GetWithConnection(NodeState fromNode, NodeState toNode)
    {
        List<NodeState> newNodes = new List<NodeState>(Nodes);
        newNodes.Remove(fromNode);
        newNodes.Remove(toNode);

        NodeConnection newConnection = new NodeConnection() { From = fromNode, To = toNode };
        NodeState newFrom = fromNode with { OutgoingConnections = new List<NodeConnection>(fromNode.OutgoingConnections) { newConnection }.AsReadOnly() };
        NodeState newTo = toNode with { IncomingConnections = new List<NodeConnection>(toNode.IncomingConnections) { newConnection }.AsReadOnly() };

        newNodes.Add(newFrom);
        newNodes.Add(newTo);

        return this with { Nodes = newNodes.AsReadOnly() };
    }
}
