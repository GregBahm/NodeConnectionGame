using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class GeometryTesterScene : MonoBehaviour
{
    [SerializeField]
    private Transform fromTransform;
    [SerializeField]
    private Transform toTransform;

    [SerializeField]
    private LineRenderer fromCircleLineRendererA;
    [SerializeField]
    private LineRenderer toCircleLineRendererA;
    [SerializeField]
    private LineRenderer fromCircleLineRendererB;
    [SerializeField]
    private LineRenderer toCircleLineRendererB;

    private const int SEGMENT_COUNT = 100;

    private void Start()
    {
        fromCircleLineRendererA.positionCount = SEGMENT_COUNT;
        toCircleLineRendererA.positionCount = SEGMENT_COUNT;
        fromCircleLineRendererB.positionCount = SEGMENT_COUNT;
        toCircleLineRendererB.positionCount = SEGMENT_COUNT;
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


        DrawLine(from, to, fromCircleLineRendererA, intersection, halfVector, true);
        DrawLine(to, from, toCircleLineRendererA, intersection, halfVector, true);
        DrawLine(from, to, fromCircleLineRendererB, intersection, halfVector, false);
        DrawLine(to, from, toCircleLineRendererB, intersection, halfVector, false);
    }

    private void DrawLine(NodeState from, NodeState to, LineRenderer renderer, Vector2 intersection, Vector2 halfVector, bool reverseArc)
    {
        Vector2 fromPerpendicular = new Vector2(-from.Direction.y, from.Direction.x);
        Vector2 toPerpendicular = new Vector2(-to.Direction.y, to.Direction.x);

        Vector2 fromCenter = GetLineLineIntersection(from.Pos, fromPerpendicular, intersection, halfVector).Value;
        Vector2 toIntersection = GetLineLineIntersection(fromCenter, toPerpendicular, to.Pos, to.Direction).Value;

        DrawCircle(renderer, from.Pos, to.Pos, toIntersection, fromCenter, reverseArc);
    }

    private void DrawCircle(LineRenderer renderer, Vector2 fromPos, Vector2 toPos, Vector2 arcEnd, Vector2 center, bool reverseArc)
    {
        float circleRadius = (fromPos - center).magnitude;
        float sweep = Vector2.SignedAngle(fromPos - center, arcEnd - center);
        float angleOffset = Vector2.SignedAngle(Vector2.right, fromPos - center);
        Vector3 startPos = new Vector3(fromPos.x, 0, fromPos.y);
        Vector3 endPos = new Vector3(toPos.x, 0, toPos.y);
        if (reverseArc)
        {
            if (sweep > 0)
            {
                sweep = 360 - sweep;
                angleOffset = Vector2.SignedAngle(Vector2.right, arcEnd - center);
                startPos = new Vector3(toPos.x, 0, toPos.y);
                endPos = new Vector3(fromPos.x, 0, fromPos.y);
            }
            else
            {
                sweep = (sweep + 360) % 360;
            }
        }

        renderer.SetPosition(0, startPos);
        for (int i = 1; i < SEGMENT_COUNT - 1; i++)
        {
            float t = i / (float)(SEGMENT_COUNT - 3);
            float angle = sweep * t + angleOffset;
            Vector2 pos = GetPointAtAngle(center, angle, circleRadius);
            renderer.SetPosition(i, new Vector3(pos.x, 0, pos.y));
        }
        renderer.SetPosition(SEGMENT_COUNT - 1, endPos);
    }

    private Vector2 GetPointAtAngle(Vector2 circleCenter, float angle, float radius)
    {
        return circleCenter + radius * new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
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
