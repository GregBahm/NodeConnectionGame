using System;
using UnityEditor.MPE;
using UnityEngine;

[RequireComponent(typeof(MainScript))]
public class BullshitPrototypingThing : MonoBehaviour
{
    private MainScript mainScript;

    private NodeBehavior selectedStart;

    private ProcessState state;

    [SerializeField]
    private Transform hoveredIndicator;

    [SerializeField]
    private ConnectionBehavior previewConnection;

    private void Start()
    {
        mainScript = GetComponent<MainScript>();
    }

    private void Update()
    {
        if(mainScript.ClosestNodeToMouse != null)
        {
            hoveredIndicator.localPosition = mainScript.ClosestNodeToMouse.transform.localPosition;
            HandleHoveredNode();
        }
        DrawState();
    }

    private void DrawState()
    {
        if(CanPreviewConnection())
        {
            NodeBehavior hoveredNode = mainScript.ClosestNodeToMouse;
            var startState = mainScript.GetCurrentNodeState(selectedStart.Identifier);
            var endState = mainScript.GetCurrentNodeState(hoveredNode.Identifier);
            previewConnection.Place(startState, endState);
        }
    }

    private bool CanPreviewConnection()
    {
        NodeBehavior hoveredNode = mainScript.ClosestNodeToMouse;
        return hoveredNode != null && selectedStart != null && hoveredNode != selectedStart;
    }

    private void HandleHoveredNode()
    {
        NodeBehavior hoveredNode = mainScript.ClosestNodeToMouse;
        if (state == ProcessState.NothingSelected)
        {
            if (Input.GetMouseButtonUp(0))
            {
                selectedStart = hoveredNode;
                state = ProcessState.StartSelected;
            }
        }
        else
        {
            if (Input.GetMouseButtonUp(0))
            {
                if (hoveredNode != selectedStart)
                {
                    CreateConnection(selectedStart, hoveredNode);
                }
                selectedStart = null;
                state = ProcessState.NothingSelected;
            }
        }
    }

    private void CreateConnection(NodeBehavior selectedStart, NodeBehavior hoveredNode)
    {
        // TODO: This
    }

    private enum ProcessState
    {
        NothingSelected,
        StartSelected
    }
}
