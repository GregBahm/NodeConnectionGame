using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UIElements;

public class GeometryTesterTwo : MonoBehaviour
{
    [SerializeField]
    private Transform fromTransform;
    [SerializeField]
    private Transform toTransform;

    [SerializeField]
    private GameObject arcPrefab;

    [SerializeField]
    private Color fromColor;

    [SerializeField]
    private Color toColor;

    private ArcBehavior theArc;

    private void Start()
    {
        theArc = SpawnArc();
        theArc.ColorStart = fromColor;
        theArc.ColorEnd = toColor;
    }

    public ArcBehavior SpawnArc()
    {         
        GameObject arcObject = Instantiate(arcPrefab);
        ArcBehavior arc = arcObject.GetComponent<ArcBehavior>();
        return arc;
    }

    private void Update()
    {
        NodeState from = GetNodeFromTransform(fromTransform);
        NodeState to = GetNodeFromTransform(toTransform);

        theArc.LineStart = from;
        theArc.LineEnd = to;
    }

    private NodeState GetNodeFromTransform(Transform node)
    {
        Vector2 pos = new Vector2(node.position.x, node.position.z);
        Vector2 direction = new Vector2(node.forward.x, node.forward.z);
        return new NodeState() { Pos = pos, Direction = direction };
    }
}
