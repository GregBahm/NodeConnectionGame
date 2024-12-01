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
    private GameObject linePrefab;
    [SerializeField]
    private GameObject rayPrefab;
    [SerializeField]
    private GameObject arcPrefab;

    [SerializeField]
    private Color fromColor;
    [SerializeField]
    private Color fromPerpendicularColor;

    [SerializeField]
    private Color toColor;
    [SerializeField]
    private Color toPerpendicularColor;

    private const int SEGMENT_COUNT = 100;

    private LineBehavior fromLine;
    private LineBehavior toLine;
    private LineBehavior fromPerpendicular;
    private LineBehavior toPerpendicular;
    private ArcBehavior theArc;

    private RayBehavior intersectionBisector;

    private void Start()
    {
        fromLine = SpawnLine();
        fromLine.ColorStart = fromColor;
        fromLine.ColorEnd = fromColor;

        fromPerpendicular = SpawnLine();
        fromPerpendicular.ColorStart = fromPerpendicularColor;
        fromPerpendicular.ColorEnd = fromPerpendicularColor;

        toLine = SpawnLine();
        toLine.ColorStart = toColor;
        toLine.ColorEnd = toColor;

        toPerpendicular = SpawnLine();
        toPerpendicular.ColorStart = toPerpendicularColor;
        toPerpendicular.ColorEnd = toPerpendicularColor;

        intersectionBisector = SpawnRay();
        intersectionBisector.ColorStart = new Color(1, 0, 0, 1);
        intersectionBisector.ColorEnd = new Color(1, 1, 0, 1);

        theArc = SpawnArc();
        theArc.ColorStart = fromColor;
        theArc.ColorEnd = toColor;
    }

    public LineBehavior SpawnLine()
    {
        GameObject lineObject = Instantiate(linePrefab);
        LineBehavior line = lineObject.GetComponent<LineBehavior>();
        return line;
    }

    public RayBehavior SpawnRay()
    {
        GameObject rayObject = Instantiate(rayPrefab);
        RayBehavior ray = rayObject.GetComponent<RayBehavior>();
        return ray;
    }

    public ArcBehavior SpawnArc()
    {         
        GameObject arcObject = Instantiate(arcPrefab);
        ArcBehavior arc = arcObject.GetComponent<ArcBehavior>();
        return arc;
    }

    [SerializeField]
    private Transform debugger;

    private void Update()
    {
        NodeState from = GetNodeFromTransform(fromTransform);
        NodeState to = GetNodeFromTransform(toTransform);

        fromLine.StartPos = from.Pos;
        fromLine.Direction = from.Direction;
        toLine.StartPos = to.Pos;
        toLine.Direction = to.Direction;

        Vector2? maybeIntersection = GetLineLineIntersection(from.Pos, from.Direction, to.Pos, to.Direction);
        if (!maybeIntersection.HasValue)
        {
            return;
        }
        Vector2 intersection = maybeIntersection.Value;

        Vector2 theBisector = GetAngleBisector(from.Pos, intersection, to.Pos);

        Vector2 fromPerpendicularDir = Vector2.Perpendicular(from.Direction);
        Vector2 toPerpendicularDir = Vector2.Perpendicular(to.Direction);
        Vector2 bisectorPerpendicular = Vector2.Perpendicular(theBisector);

        intersectionBisector.StartPos = intersection;
        intersectionBisector.Direction = theBisector;

        Vector2 fromOnBisector = ProjectPointOnLine(from.Pos, intersection, theBisector);
        Vector2 toOnBisector = ProjectPointOnLine(to.Pos, intersection, theBisector);

        fromPerpendicular.StartPos = from.Pos;
        fromPerpendicular.Direction = fromPerpendicularDir;
        toPerpendicular.StartPos = toPerpendicularDir;
        toPerpendicular.Direction = toPerpendicularDir;

        float fromToIntersection = (from.Pos - intersection).magnitude;
        float toToIntersection = (to.Pos - intersection).magnitude;
        if(fromToIntersection < toToIntersection)
        {
            Vector2 perpendicularBisectorIntersection = GetLineLineIntersection(from.Pos, fromPerpendicularDir, intersection, theBisector).Value;
            Vector2 arcEnd = ReflectPointOverLine(from.Pos, intersection, theBisector);
            theArc.StartPoint = from.Pos;
            theArc.EndPoint = arcEnd;
            theArc.Center = perpendicularBisectorIntersection;
        }
        else
        {
            Vector2 perpendicularBisectorIntersection = GetLineLineIntersection(to.Pos, toPerpendicularDir, intersection, theBisector).Value;
            Vector2 arcStart = ReflectPointOverLine(to.Pos, intersection, theBisector);
            theArc.StartPoint = arcStart;
            theArc.EndPoint = to.Pos;
            theArc.Center = perpendicularBisectorIntersection;
        }


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

    [SerializeField]
    private bool flipA;
    [SerializeField]
    private bool flipB;

    private Vector2 GetAngleBisector(Vector2 pointA, Vector2 pointB, Vector2 pointC)
    {
        Vector2 ab = (pointA - pointB).normalized;
        Vector2 bc = (pointC - pointB).normalized;
        ab = flipA ? -ab : ab;
        bc = flipB ? -bc : bc;
        return (ab + bc).normalized;
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
