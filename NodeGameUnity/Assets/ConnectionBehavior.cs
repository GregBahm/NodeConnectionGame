using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ConnectionBehavior : MonoBehaviour
{
    [SerializeField]
    private LineRenderer lineRenderer;

    private const int SEGMENT_COUNT = 10;

    public void Place(NodeState from, NodeState to)
    {
        Vector2 offset = to.Pos - from.Pos;
        transform.localPosition = new Vector3(from.Pos.x, 0, from.Pos.y);
        lineRenderer.SetPosition(1, new Vector3(offset.x, 0, offset.y));
    }
}
