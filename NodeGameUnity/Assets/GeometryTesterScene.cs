using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeometryTesterScene : MonoBehaviour
{
    [SerializeField]
    private Transform fromTransform;
    [SerializeField]
    private Transform toTransform;

    [SerializeField]
    private LineRenderer fromCircleLineRenderer;
    [SerializeField]
    private LineRenderer toCircleLineRenderer;

    [SerializeField]
    private Transform intersectionPoint;

    private const int SEGMENT_COUNT = 100;

    private void Start()
    {
        fromCircleLineRenderer.positionCount = SEGMENT_COUNT;
        toCircleLineRenderer.positionCount = SEGMENT_COUNT;
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
        intersectionPoint.localPosition = new Vector3(intersection.x, 0, intersection.y);

        Vector2 intersectionToFrom = (from.Pos - intersection).normalized;
        Vector2 intersectionToTo = (to.Pos - intersection).normalized;
        Vector2 halfVector = (intersectionToFrom + intersectionToTo).normalized;


        Vector2 fromPerpendicular = new Vector2(-from.Direction.y, from.Direction.x);
        Vector2 toPerpendicular = new Vector2(-to.Direction.y, to.Direction.x);

        Vector2 fromCenter = GetLineLineIntersection(from.Pos, fromPerpendicular, intersection, halfVector).Value;
        Vector2 toCenter = GetLineLineIntersection(to.Pos, toPerpendicular, intersection, halfVector).Value;

        DrawCircle(fromCircleLineRenderer, from, fromCenter);
        DrawCircle(toCircleLineRenderer, to, toCenter);
    }

    private void DrawCircle(LineRenderer renderer, NodeState pointOnCircle, Vector2 center)
    {
        float circleRadius = (pointOnCircle.Pos - center).magnitude;
        for (int i = 0; i < SEGMENT_COUNT; i++)
        {
            float t = i / (float)(SEGMENT_COUNT - 1);
            float angle = Mathf.Lerp(0, 360, t);//Mathf.Lerp(fromAngle, toAngle, t);
            Vector2 pos = center + circleRadius * new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
            renderer.SetPosition(i, new Vector3(pos.x, 0, pos.y));
        }
    }

    private NodeState GetNodeFromTransform(Transform node)
    {
        Vector2 pos = new Vector2(node.position.x, node.position.z);
        Vector2 direction = new Vector2(node.forward.x, node.forward.z);
        return new NodeState() { Pos =pos, Direction = direction };
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
