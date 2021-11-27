using UnityEngine;

public class GnatScriptableObject : ScriptableObject
{
    public float aliveTimer = 0f;
    public GameObject gnatObject = null;
    public Vector2 maxBound = Vector2.zero;
    public Vector2 minBound = Vector2.zero;
    public Vector2 position = Vector2.zero;
    public Vector2 targetPos = Vector2.zero;
}
