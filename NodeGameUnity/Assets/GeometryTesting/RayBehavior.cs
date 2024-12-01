using UnityEngine;

public class RayBehavior : MonoBehaviour
{
    public Vector2 StartPos { get; set; }
    public Vector2 Direction { get; set; }

    public Color ColorStart { get; set; }
    public Color ColorEnd { get; set; }

    [SerializeField]
    private LineRenderer theRenderer;
    private void Start()
    {
        theRenderer = GetComponent<LineRenderer>();
        theRenderer.positionCount = 2;
        theRenderer.startColor = ColorStart;
        theRenderer.endColor = ColorEnd;
    }
    private void Update()
    {
        Vector2 offset = StartPos + Direction * 2000;
        theRenderer.SetPosition(0, new Vector3(StartPos.x, 0, StartPos.y));
        theRenderer.SetPosition(1, new Vector3(offset.x, 0, offset.y));
    }
}
