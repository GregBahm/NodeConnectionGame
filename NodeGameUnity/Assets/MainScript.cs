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

    [SerializeField]
    private Transform debuggyBuddyA;
    [SerializeField]
    private Transform debuggyBuddyB;

    private void Start()
    {
        grid = GetComponent<GameGrid>();
        GameState initialState = grid.GetInitialGameState();
        gameStates.Add(initialState);
        grid.InitializeGrid(initialState);
    }

    private void Update()
    {
        Vector2 mousePlanePosition = GetMousePlanePosition();
        closestNodeToMouse = grid.GetClosestNodeToMouse(mousePlanePosition);
    }

    private Vector2 GetMousePlanePosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(Vector3.up, Vector3.zero);
        if (plane.Raycast(ray, out float distance))
        {
            Vector3 pos = ray.GetPoint(distance);
            return new Vector2(pos.x, pos.z);
        }
        return Vector2.zero;
    }
}

public record NodeState
{
    public string Identifier { get; init; }

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
