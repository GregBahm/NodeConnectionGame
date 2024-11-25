using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ConnectionBehavior : MonoBehaviour
{
    [SerializeField]
    private LineRenderer lineRenderer;

    private const int SEGMENT_COUNT = 100;

    private void Start()
    {
        lineRenderer.positionCount = SEGMENT_COUNT;
    }

    public void Place(NodeState from, NodeState to)
    {
        Vector2 fromPerpendicular = new Vector2(-from.Direction.y, from.Direction.x);
        Vector2 toPerpendicular = new Vector2(-to.Direction.y, to.Direction.x);
        
        Vector2? maybeIntersection = GetLineLineIntersection(from.Pos, fromPerpendicular, to.Pos, toPerpendicular);

        if(!maybeIntersection.HasValue)
        {
            return;
        }
        Vector2 intersection = maybeIntersection.Value;
        transform.localPosition = new Vector3(intersection.x, 0, intersection.y);

        float circleRadius = (from.Pos - intersection).magnitude;

        //TODO: ReplaceThisMath

        float fromAngle = Vector2.SignedAngle(Vector2.up, from.Pos - intersection);
        float toAngle = Vector2.SignedAngle(Vector2.up, to.Pos - intersection);

        for (int i = 0; i < SEGMENT_COUNT; i++)
        {
            float t = i / (float)(SEGMENT_COUNT - 1);
            float angle = Mathf.Lerp(0, 360, t);//Mathf.Lerp(fromAngle, toAngle, t);
            Vector2 pos = intersection + circleRadius * new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
            lineRenderer.SetPosition(i, new Vector3(pos.x, 0, pos.y));
        }
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
