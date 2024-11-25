using System;
using System.Collections.Generic;
using System.Linq;
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

    private readonly Dictionary<string, NodeBehavior> nodeGameObjects = new Dictionary<string, NodeBehavior>();
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
        obj.name = node.Identifier;
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
        return new GameState(states.AsReadOnly());
    }

    private NodeState CreateNodeState(int row, int column)
    {
        Vector2 direction = UnityEngine.Random.insideUnitCircle;
        Vector2 positionOffset = new Vector2(UnityEngine.Random.value, UnityEngine.Random.value) * margin;
        Vector2 position = new Vector2(column, row) + positionOffset;
        NodeState state = new NodeState()
        {
            Identifier = row + "," + column,
            Pos = position,
            Direction = direction,
            Type = (NodeType)UnityEngine.Random.Range(0, 3),
            IncomingConnections = new List<NodeConnection>().AsReadOnly(),
            OutgoingConnections = new List<NodeConnection>().AsReadOnly()
        };
        return state;
    }

    internal NodeBehavior GetClosestNodeToMouse(Vector2 mousePlanePosition)
    {
        int gridX = Mathf.FloorToInt(mousePlanePosition.x);
        int gridY = Mathf.FloorToInt(mousePlanePosition.y);
        NodeBehavior[] nodes = GetNodesAroundGrid(gridX, gridY).ToArray();
        NodeBehavior closestNode = null;
        float closestDistance = float.MaxValue;
        foreach (NodeBehavior node in nodes)
        {
            Vector2 pos = new Vector2(node.transform.localPosition.x, node.transform.localPosition.z);
            float distance = Vector2.Distance(pos, mousePlanePosition);
            if (distance < closestDistance)
            {
                closestNode = node;
                closestDistance = distance;
            }
        }
        return closestNode;
    }

    private IEnumerable<NodeBehavior> GetNodesAroundGrid(int gridX, int gridY)
    {
        for (int row = gridY - 1; row <= gridY + 1; row++)
        {
            for (int column = gridX - 1; column <= gridX + 1; column++)
            {
                if (row >= 0 && row < rows && column >= 0 && column < columns)
                {
                    NodeBehavior node = nodeGrid[row, column];
                    yield return node;
                }
            }
        }
    }
}
