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
    private Color rayColor;

    [SerializeField]
    private Color fromColor;

    [SerializeField]
    private Color toColor;

    private const int SEGMENT_COUNT = 100;

    private ArcBehavior theArc;

    [SerializeField]
    private bool invertSweep;

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

    [SerializeField]
    private Transform debuggyBuddy;

    private void Update()
    {
        NodeState from = GetNodeFromTransform(fromTransform);
        NodeState to = GetNodeFromTransform(toTransform);

        Vector2? maybeIntersection = GetLineLineIntersection(from.Pos, from.Direction, to.Pos, to.Direction);
        if (!maybeIntersection.HasValue)
        {
            return;
        }
        Vector2 intersection = maybeIntersection.Value;

        Vector2 theBisector = (from.Direction + to.Direction).normalized;
        theBisector = Vector2.Perpendicular(theBisector);

        Vector2 fromPerpendicularDir = Vector2.Perpendicular(from.Direction);
        Vector2 toPerpendicularDir = Vector2.Perpendicular(to.Direction);

        float fromToIntersection = (from.Pos - intersection).magnitude;
        float toToIntersection = (to.Pos - intersection).magnitude;

        Vector2 offsetFromCenter = GetLineLineIntersection(from.Pos, fromPerpendicularDir, intersection, theBisector).Value;
        Vector2 offsetToCenter = GetLineLineIntersection(to.Pos, toPerpendicularDir, intersection, theBisector).Value;

        Vector2 offsetEnd = ReflectPointOverLine(from.Pos, intersection, theBisector);
        Vector2 offsetStart = ReflectPointOverLine(to.Pos, intersection, theBisector);

        float offsetDot = Vector2.Dot(offsetStart - from.Pos, from.Direction);
        if(offsetDot > 0)
        {
            theArc.ArcStart = from.Pos;
            theArc.ArcEnd = offsetEnd;
            theArc.Center = offsetFromCenter;

            debuggyBuddy.position = new Vector3(offsetEnd.x, 0, offsetEnd.y);
            debuggyBuddy.rotation = toTransform.rotation;
        }
        else
        {
            theArc.ArcStart = offsetStart;
            theArc.ArcEnd = to.Pos;
            theArc.Center = offsetToCenter;

            debuggyBuddy.position = new Vector3(offsetStart.x, 0, offsetStart.y);
            debuggyBuddy.rotation = fromTransform.rotation;
        }

        theArc.LineStart = from.Pos;
        theArc.LineEnd = to.Pos;

        float arcDot = Vector2.Dot(from.Direction, Vector2.Perpendicular(to.Direction));

        theArc.InvertSweep = arcDot > 0;
    }

    private static Vector2 ReflectPointOverLine(Vector2 point, Vector2 lineStart, Vector2 lineDirection)
    {
        Vector2 pointToStart = point - lineStart;
        float projectionLength = Vector2.Dot(pointToStart, lineDirection);
        Vector2 projection = projectionLength * lineDirection;
        Vector2 closestPointOnLine = lineStart + projection;
        Vector2 reflectedPoint = closestPointOnLine * 2 - point;
        return reflectedPoint;
    }

    private static Vector2 ProjectPointOnLine(Vector2 point, Vector2 lineStart, Vector2 lineDirection)
    {
        Vector2 pointToStart = point - lineStart;
        float dot = Vector2.Dot(pointToStart, lineDirection);
        return lineStart + dot * lineDirection;
    }

    private NodeState GetNodeFromTransform(Transform node)
    {
        Vector2 pos = new Vector2(node.position.x, node.position.z);
        Vector2 direction = new Vector2(node.forward.x, node.forward.z);
        return new NodeState() { Pos = pos, Direction = direction };
    }

    public static Vector2? GetLineLineIntersection(Vector2 lineAStart, Vector2 lineADirection, Vector2 lineBStart, Vector2 lineBDirection)
    {
        float crossDir = lineADirection.x * lineBDirection.y - lineADirection.y * lineBDirection.x;

        if (Mathf.Abs(crossDir) < Mathf.Epsilon)
        {
            return null; // No intersection
        }

        Vector2 diffStart = lineBStart - lineAStart;
        float t = (diffStart.x * lineBDirection.y - diffStart.y * lineBDirection.x) / crossDir;

        Vector2 intersection = lineAStart + t * lineADirection;
        return intersection;
    }
}
