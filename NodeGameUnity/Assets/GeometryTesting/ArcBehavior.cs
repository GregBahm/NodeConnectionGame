using System.Collections;
using System.Collections.Generic;
using Unity.Properties;
using UnityEngine;

public class ArcBehavior : MonoBehaviour
{
    public NodeState LineStart { get; set; }
    public NodeState LineEnd { get; set; }
    public Vector2 ArcStart { get; set; }
    public Vector2 ArcEnd { get; set; }

    public Vector2 Center { get; set; }

    public Color ColorStart { get; set; }
    public Color ColorEnd { get; set; }

    public float BaseAngleOffset { get; private set; }

    private int resolution = 100;

    public bool LongWayAround { get; set; }

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
        float circleRadius = (ArcStart - Center).magnitude;
        float angleOffset = Vector2.SignedAngle(Vector2.right, ArcStart - Center);
        float sweep = GetSweep();
        SetPositions(circleRadius, sweep, angleOffset);
    }

    private float GetSweep()
    {
        float sweep = Vector2.Angle(ArcStart - Center, ArcEnd - Center);
        bool clockwise = IsTangentClockwise(Center, ArcStart, LineStart.Direction);
        Vector2 arcStartToArcEnd = ArcStart - ArcEnd;
        bool longWayAround = Vector2.Dot(LineStart.Direction, arcStartToArcEnd) < 0;

        if (LongWayAround)
            sweep = 360 - sweep;
        if (!clockwise)
            sweep = -sweep;
        return sweep;
    }
    private static bool IsTangentClockwise(Vector2 circleCenter, Vector2 tangentPointOnCircle, Vector2 directionOfTangent)
    {
        Vector2 centerToPoint = tangentPointOnCircle - circleCenter;
        Vector2 normalizedTangent = directionOfTangent.normalized;

        float crossProduct = centerToPoint.x * normalizedTangent.y - centerToPoint.y * normalizedTangent.x;
        return crossProduct < 0;
    }

    private void SetPositions(float circleRadius, float sweep, float angleOffset)
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

    private Vector2 GetPointAtAngle(Vector2 circleCenter, float angle, float radius)
    {
        return circleCenter + radius * new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
    }
}