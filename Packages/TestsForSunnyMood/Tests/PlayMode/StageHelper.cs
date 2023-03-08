using UnityEditor;
using UnityEngine;

public class PlatformInfo : MonoBehaviour
{
    [HideInInspector] public Vector2 leftBottom, rightBottom, leftUp, rightUp;

    public Vector2 center;
    public Vector2 size;

    public bool reachable = false;

    private BoxCollider2D col;

    void Start()
    {
        col = GetComponent<BoxCollider2D>();
        center = col.bounds.center;
        size = col.size;
        rightUp = (center + new Vector2(size.x / 2 + col.edgeRadius, size.y / 2 + col.edgeRadius));
        rightBottom = (center + new Vector2(size.x / 2 + col.edgeRadius, -size.y / 2 - col.edgeRadius));
        leftUp = (center + new Vector2(-size.x / 2 - col.edgeRadius, size.y / 2 + col.edgeRadius));
        leftBottom = (center + new Vector2(-size.x / 2 - col.edgeRadius, -size.y / 2 - col.edgeRadius));
    }
}