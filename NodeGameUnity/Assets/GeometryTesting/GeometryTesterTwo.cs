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
    private Color rayColor;

    [SerializeField]
    private Color fromColor;

    [SerializeField]
    private Color toColor;

    private const int SEGMENT_COUNT = 100;

    private ArcBehavior theArc;

    private RayBehavior intersectionBisector;

    [SerializeField]
    private bool invertSweep;
    [SerializeField]
    private bool startFromFrom;

    private void Start()
    {
        intersectionBisector = SpawnRay();
        intersectionBisector.ColorStart = rayColor;
        intersectionBisector.ColorEnd = Color.black;

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

        intersectionBisector.StartPos = intersection;
        intersectionBisector.Direction = theBisector;

        float fromToIntersection = (from.Pos - intersection).magnitude;
        float toToIntersection = (to.Pos - intersection).magnitude;
        if((fromToIntersection < toToIntersection) == startFromFrom)
        {
            Vector2 perpendicularBisectorIntersection = GetLineLineIntersection(from.Pos, fromPerpendicularDir, intersection, theBisector).Value;
            Vector2 arcEnd = ReflectPointOverLine(from.Pos, intersection, theBisector);
            theArc.ArcStart = from.Pos;
            theArc.ArcEnd = arcEnd;
            theArc.Center = perpendicularBisectorIntersection;
        }
        else
        {
            Vector2 perpendicularBisectorIntersection = GetLineLineIntersection(to.Pos, toPerpendicularDir, intersection, theBisector).Value;
            Vector2 arcStart = ReflectPointOverLine(to.Pos, intersection, theBisector);
            theArc.ArcStart = arcStart;
            theArc.ArcEnd = to.Pos;
            theArc.Center = perpendicularBisectorIntersection;
        }

        theArc.LineStart = from.Pos;
        theArc.LineEnd = to.Pos;

        theArc.InvertSweep = invertSweep;
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
