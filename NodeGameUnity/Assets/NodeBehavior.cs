using System;
using UnityEngine;

public class NodeBehavior : MonoBehaviour
{
    public string Identifier { get; private set; }

    public void Initialize(NodeState initialState)
    {
        Identifier = initialState.Identifier;
        transform.localPosition = new Vector3(initialState.Pos.x, 0, initialState.Pos.y);
        transform.localRotation = Quaternion.LookRotation(new Vector3(initialState.Direction.x, 0, initialState.Direction.y), Vector3.up);
    }
}
