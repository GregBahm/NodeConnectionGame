using UnityEngine;
using static Unity.VisualScripting.StickyNote;

public class LineBehavior : MonoBehaviour
{
    public Vector2 StartPos { get; set; }
    public Vector2 Direction { get; set; }

    public Color ColorStart { get; set; }
    public Color ColorEnd { get; set; }

    private float scale = 10;

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
        Vector2 startOffset = StartPos - Direction * scale;
        Vector2 endOffset = StartPos + Direction * scale;
        theRenderer.SetPosition(0, new Vector3(startOffset.x, 0, startOffset.y));
        theRenderer.SetPosition(1, new Vector3(endOffset.x, 0, endOffset.y));
    }
}