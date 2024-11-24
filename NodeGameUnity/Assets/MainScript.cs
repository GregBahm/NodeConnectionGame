using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GameGrid))]
public class MainScript : MonoBehaviour
{
    private GameGrid grid;

    private readonly List<GameState> gameStates = new List<GameState>();

    private NodeBehavior closestNodeToMouse;
    public NodeBehavior ClosestNodeToMouse => closestNodeToMouse;

    private void Start()
    {
        grid = GetComponent<GameGrid>();
        GameState initialState = grid.GetInitialGameState();
        gameStates.Add(initialState);
        grid.InitializeGrid(initialState);
    }

    private void Update()
    {
        closestNodeToMouse = GetClosestNodeToMouse();
    }

    private NodeBehavior GetClosestNodeToMouse()
    {
        return null;
        //throw new NotImplementedException();
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
