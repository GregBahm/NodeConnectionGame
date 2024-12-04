using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Properties;
using UnityEngine;

public class ArcBehavior : MonoBehaviour
{
    public NodeState LineStart { get; set; }

    public NodeState LineEnd { get; set; }

    public Vector2 ArcStart { get; private set; }
    public Vector2 ArcEnd { get; private set; }

    public Vector2 Center { get; private set; }

    public Color ColorStart { get; set; }
    public Color ColorEnd { get; set; }

    private int resolution = 100;

    [SerializeField]
    private LineRenderer theRenderer;
    private void Start()
    {
        theRenderer = GetComponent<LineRenderer>();
        theRenderer.positionCount = resolution;
        theRenderer.startColor = ColorStart;
        theRenderer.endColor = ColorEnd;
    }
    private void Update()
    {
        Vector2? maybeIntersection = GetLineLineIntersection(LineStart.Pos, LineStart.Direction, LineEnd.Pos, LineEnd.Direction);
        if (!maybeIntersection.HasValue)
        {
            //TODO: Handle this case
            return;
        }

        SetArcStartAndEnd(maybeIntersection.Value);

        float sweep = GetSweep();
        float circleRadius = (ArcStart - Center).magnitude;
        float angleOffset = Vector2.SignedAngle(Vector2.right, ArcStart - Center);

        SetTheRenderer(sweep, circleRadius, angleOffset);
    }

    private void SetArcStartAndEnd(Vector2 intersection)
    {

        Vector2 theBisector = (LineStart.Direction + LineEnd.Direction).normalized;
        theBisector = Vector2.Perpendicular(theBisector);

        Vector2 fromPerpendicularDir = Vector2.Perpendicular(LineStart.Direction);
        Vector2 toPerpendicularDir = Vector2.Perpendicular(LineEnd.Direction);

        float fromToIntersection = (LineStart.Pos - intersection).magnitude;
        float toToIntersection = (LineEnd.Pos - intersection).magnitude;

        Vector2 offsetFromCenter = GetLineLineIntersection(LineStart.Pos, fromPerpendicularDir, intersection, theBisector).Value;
        Vector2 offsetToCenter = GetLineLineIntersection(LineEnd.Pos, toPerpendicularDir, intersection, theBisector).Value;

        Vector2 offsetEnd = ReflectPointOverLine(LineStart.Pos, intersection, theBisector);
        Vector2 offsetStart = ReflectPointOverLine(LineEnd.Pos, intersection, theBisector);

        float offsetDot = Vector2.Dot(offsetStart - LineStart.Pos, LineStart.Direction);
        if (offsetDot > 0)
        {
            ArcStart = LineStart.Pos;
            ArcEnd = offsetEnd;
        }
        else
        {
            ArcStart = offsetStart;
            ArcEnd = LineEnd.Pos;
        }
        Center = offsetToCenter;
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

    private void SetTheRenderer(float sweep, float circleRadius, float angleOffset)
    {
        theRenderer.SetPosition(0, new Vector3(LineStart.Pos.x, 0, LineStart.Pos.y));
        for (int i = 1; i < resolution - 1; i++)
        {
            float t = (i - 1) / (float)(resolution - 3);
            float angle = sweep * t + angleOffset;
            Vector2 pos = GetPointAtAngle(Center, angle, circleRadius);
            theRenderer.SetPosition(i, new Vector3(pos.x, 0, pos.y));
        }
        theRenderer.SetPosition(resolution - 1, new Vector3(LineEnd.Pos.x, 0, LineEnd.Pos.y));
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

    float GetSweep()
    {
        float sweep = Vector2.Angle(ArcStart - Center, ArcEnd - Center);

        Vector2 arcStartToArcEnd = ArcStart - ArcEnd;
        bool longWayAround = Vector2.Dot(LineStart.Direction, arcStartToArcEnd) < 0;
        sweep = longWayAround ? 360 - sweep : sweep;

        bool clockwise = IsTangentClockwise(Center, ArcStart, LineStart.Direction);
        return clockwise ? sweep : -sweep;
    }

    private Vector2 GetPointAtAngle(Vector2 circleCenter, float angle, float radius)
    {
        return circleCenter + radius * new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
    }

    public static bool IsTangentClockwise(Vector2 circleCenter, Vector2 tangentPointOnCircle, Vector2 directionOfTangent)
    {
        Vector2 centerToPoint = tangentPointOnCircle - circleCenter;
        Vector2 normalizedTangent = directionOfTangent.normalized;

        float crossProduct = centerToPoint.x * normalizedTangent.y - centerToPoint.y * normalizedTangent.x;
        return crossProduct < 0;
    }
}