using System;
using System.Collections.Generic;
using UnityEngine;

public class GameGrid : MonoBehaviour
{
    [SerializeField]
    private GameObject nodePrefab;

    [SerializeField]
    private int rows;
    [SerializeField]
    private int columns;
    [SerializeField]
    private float margin;

    private readonly Dictionary<Guid, NodeBehavior> nodeGameObjects = new Dictionary<Guid, NodeBehavior>();
    private NodeBehavior[,] nodeGrid;

    public void InitializeGrid(GameState currentState)
    {
        nodeGrid = new NodeBehavior[rows, columns];
        foreach (NodeState node in currentState.Nodes)
        {
            InitializeNodeGameObject(node);
        }
    }

    private void InitializeNodeGameObject(NodeState node)
    {
        GameObject obj = Instantiate(nodePrefab);
        NodeBehavior behavior = obj.GetComponent<NodeBehavior>();
        behavior.Initialize(node);
        nodeGameObjects.Add(node.Identifier, behavior);
        int row = Mathf.FloorToInt(node.Pos.y);
        int column = Mathf.FloorToInt(node.Pos.x);
        nodeGrid[row, column] = behavior;
    }

    public GameState GetInitialGameState()
    {
        List<NodeState> states = new List<NodeState>();
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
        Vector2 positionOffset = new Vector2(UnityEngine.Random.value, UnityEngine.Random.value) * margin;
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
