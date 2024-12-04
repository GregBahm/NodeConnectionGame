using System.Collections;
using System.Collections.Generic;
using Unity.Properties;
using UnityEngine;

public class ArcBehavior : MonoBehaviour
{
    public Vector2 LineStart { get; set; }
    public Vector2 LineEnd { get; set; }
    public Vector2 ArcStart { get; set; }
    public Vector2 ArcEnd { get; set; }

    public Vector2 Center { get; set; }

    public Color ColorStart { get; set; }
    public Color ColorEnd { get; set; }

    public float BaseAngleOffset { get; private set; }

    private int resolution = 100;

    public float Sweep { get; private set; }

    public bool LongWayAround { get; set; }
    public bool CounterClockwise { get; set; }

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
        Sweep = Vector2.Angle(ArcStart - Center, ArcEnd - Center);
        BaseAngleOffset = Vector2.SignedAngle(Vector2.right, ArcStart - Center);
        float angleOffset = BaseAngleOffset;

        if(LongWayAround)
        {
            Sweep = 360 - Sweep;
        }

        theRenderer.SetPosition(0, new Vector3(LineStart.x, 0, LineStart.y));
        for (int i = 1; i < resolution - 1; i++)
        {
            float t = (i - 1) / (float)(resolution - 3);
            if(CounterClockwise)
            {
                t = -t;
            }
            float angle = Sweep * t + angleOffset;
            Vector2 pos = GetPointAtAngle(Center, angle, circleRadius);
            theRenderer.SetPosition(i, new Vector3(pos.x, 0, pos.y));
        }
        theRenderer.SetPosition(resolution - 1, new Vector3(LineEnd.x, 0, LineEnd.y));
    }

    private Vector2 GetPointAtAngle(Vector2 circleCenter, float angle, float radius)
    {
        return circleCenter + radius * new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
    }
}