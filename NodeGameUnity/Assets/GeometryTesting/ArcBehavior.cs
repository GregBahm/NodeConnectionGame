using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcBehavior : MonoBehaviour
{
    public Vector2 StartPoint { get; set; }
    public Vector2 EndPoint { get; set; }

    public Vector2 Center { get; set; }

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
        float circleRadius = (StartPoint - Center).magnitude;
        float sweep = Vector2.SignedAngle(StartPoint - Center, EndPoint - Center);
        float angleOffset = Vector2.SignedAngle(Vector2.right, StartPoint - Center);

        for (int i = 0; i < resolution; i++)
        {
            float t = i / (float)(resolution - 1);
            float angle = sweep * t + angleOffset;
            Vector2 pos = GetPointAtAngle(Center, angle, circleRadius);
            theRenderer.SetPosition(i, new Vector3(pos.x, 0, pos.y));
        }
    }

    private Vector2 GetPointAtAngle(Vector2 circleCenter, float angle, float radius)
    {
        return circleCenter + radius * new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
    }
}