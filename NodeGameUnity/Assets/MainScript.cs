using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainScript : MonoBehaviour
{
    [SerializeField]
    private GameObject nodePrefab;

    private readonly List<GameState> gameStates = new List<GameState>();

    private readonly Dictionary<Guid, NodeBehavior> nodeGameObjects = new Dictionary<Guid, NodeBehavior>();

    private void Start()
    {
        GameState state = GetInitialGameState();
        gameStates.Add(state);
        DrawState();
    }

    private void DrawState()
    {
        GameState currentState = gameStates[gameStates.Count - 1];
        foreach (NodeState node in currentState.Nodes)
        {
            if (nodeGameObjects.ContainsKey(node.Identifier))
            {
            }
            else
            {
                InitializeNodeGameObject(node);
            }
        }
    }

    private void InitializeNodeGameObject(NodeState node)
    {
        GameObject obj = Instantiate(nodePrefab);
        NodeBehavior behavior = obj.GetComponent<NodeBehavior>();
        behavior.Initialize(node);
        nodeGameObjects.Add(node.Identifier, behavior);
    }

    private GameState GetInitialGameState()
    {
        List<NodeState> states = new List<NodeState>();
        int rows = 20;
        int columns = 20;
        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                NodeState state = CreateNodeState(row, column);
                states.Add(state);
            }
        }
        return new GameState() { Nodes = states.AsReadOnly() };
    }

    private NodeState CreateNodeState(int row, int column)
    {
        Vector2 direction = UnityEngine.Random.insideUnitCircle;
        Vector2 positionOffset = new Vector2(UnityEngine.Random.value, UnityEngine.Random.value) * .8f; // to give it some margin
        Vector2 position = new Vector2(column, row) + positionOffset;
        NodeState state = new NodeState()
        {
            Identifier = Guid.NewGuid(),
            Pos = position, 
            Direction = direction,
            Type = (NodeType)UnityEngine.Random.Range(0, 3),
            IncomingConnections = new List<NodeConnection>().AsReadOnly(),
            OutgoingConnections = new List<NodeConnection>().AsReadOnly()
        };
        return state;
    }
}

public record GameState
{
    public IReadOnlyList<NodeState> Nodes { get; init; }

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

public record NodeState
{
    public Guid Identifier { get; init; }

    public Vector2 Pos { get; init; }
    public Vector2 Direction { get; init; }

    public NodeType Type { get; init; }

    public IReadOnlyList<NodeConnection> IncomingConnections { get; init; }
    public IReadOnlyList<NodeConnection> OutgoingConnections { get; init; }
}

public enum NodeType
{
    Red,
    Green,
    Blue
}

public record NodeConnection
{
    public NodeState From { get; init; }
    public NodeState To { get; init; }
}
