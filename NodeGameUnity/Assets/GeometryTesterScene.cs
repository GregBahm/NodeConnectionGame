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

    [SerializeField]
    private Transform debugStart;
    [SerializeField]
    private Transform debugEnd;

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

        Vector2 intersectionToFrom = (from.Pos - intersection).normalized;
        Vector2 intersectionToTo = (to.Pos - intersection).normalized;
        Vector2 halfVector = (intersectionToFrom + intersectionToTo).normalized;


        Vector2 fromPerpendicular = new Vector2(-from.Direction.y, from.Direction.x);
        Vector2 toPerpendicular = new Vector2(-to.Direction.y, to.Direction.x);

        Vector2 fromCenter = GetLineLineIntersection(from.Pos, fromPerpendicular, intersection, halfVector).Value;
        Vector2 toIntersection = GetLineLineIntersection(fromCenter, toPerpendicular, to.Pos, to.Direction).Value;

        DrawCircle(fromCircleLineRenderer, from.Pos, toIntersection, fromCenter);

        //Vector2 toCenter = GetLineLineIntersection(to.Pos, toPerpendicular, intersection, halfVector).Value;
        //DrawCircle(toCircleLineRenderer, to, toCenter);
    }

    private void DrawCircle(LineRenderer renderer, Vector2 arcStart, Vector2 arcEnd, Vector2 center)
    {

        float circleRadius = (arcStart - center).magnitude;
        float angleStart = Vector2.SignedAngle(Vector2.right, arcStart - center);
        float angleEnd = Vector2.SignedAngle(Vector2.right, arcEnd - center);
        Debug.Log("Start:" + angleStart + " End:" + angleEnd);
        for (int i = 0; i < SEGMENT_COUNT; i++)
        {
            float t = i / (float)(SEGMENT_COUNT - 1);
            Vector2 pos = GetPos(center, angleStart, angleEnd, circleRadius, t);
            renderer.SetPosition(i, new Vector3(pos.x, 0, pos.y));
        }
    }

    private Vector2 GetPos(Vector2 circleCenter, float startAngle, float endAngle, float radius, float t)
    {
        float angle = Mathf.Lerp(startAngle, endAngle, t);
        return circleCenter + radius * new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
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
