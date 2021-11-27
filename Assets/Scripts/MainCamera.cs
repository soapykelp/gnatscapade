using UnityEngine;

public class MainCamera : MonoBehaviour
{
    public Transform followTarget;

    private bool freeze;
    private bool pastThreshold;
    private float cameraLerpSpeed = 3f;
    private float cameraMovementSpeed = 6f;
    private Vector2 frozenMousePos;
    private Vector2 maxPos;
    private Vector2 minPos;

    void Start()
    {
        minPos = (Vector2)Camera.main.ViewportToWorldPoint(new Vector2(0, 0));
        maxPos = (Vector2)Camera.main.ViewportToWorldPoint(new Vector2(1, 1));
    }

    void FixedUpdate()
    {
        minPos = (Vector2)Camera.main.ViewportToWorldPoint(new Vector2(0, 0));
        maxPos = (Vector2)Camera.main.ViewportToWorldPoint(new Vector2(1, 1));

        float gnatX = followTarget.position.x;
        float screenWidth = maxPos.x - minPos.x;
        float threshold = screenWidth*0.75f + minPos.x;
        float stopThreshold = screenWidth*0.7f + minPos.x;

        if (!freeze && (gnatX > threshold || pastThreshold))
        {
            pastThreshold = true;
            transform.position = new Vector3(this.transform.position.x + cameraMovementSpeed*Time.fixedDeltaTime, 
                                            this.transform.position.y,
                                            this.transform.position.z);
            if (gnatX < stopThreshold) {
                pastThreshold = false;
            }
        }

        if (freeze)
        {
            if ((Vector2)Input.mousePosition != frozenMousePos) {
                if (Mathf.Abs(transform.position.x - followTarget.position.x) < screenWidth*0.1f) {
                    freeze = false;
                }
            }

            transform.position = new Vector3(Vector2.Lerp(transform.position, 
                                                          followTarget.position, 
                                                          cameraLerpSpeed * Time.fixedDeltaTime).x,
                                             transform.position.y,
                                             transform.position.z);
        }
    }

    public void OnSwapEvent()
    {
        freeze = true;
        frozenMousePos = Input.mousePosition;
    }
}
