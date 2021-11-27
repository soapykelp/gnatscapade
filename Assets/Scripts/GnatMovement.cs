using UnityEngine;

public class GnatMovement : MonoBehaviour
{
    private bool freeze;
    private float MaxSpeed = 20f;
    private Rigidbody2D gnatBody;
    private Vector2 frozenMousePos;
    private Vector2 maxPos;
    private Vector2 minPos;
    private Vector2 mousePos;
    private Vector2 size;

    // Start is called before the first frame update
    void Start()
    {
        size = GetComponent<SpriteRenderer>().bounds.extents;
        minPos = (Vector2)Camera.main.ViewportToWorldPoint(new Vector2(0, 0)) + size;
        maxPos = (Vector2)Camera.main.ViewportToWorldPoint(new Vector2(1, 1)) - size;

        gnatBody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!freeze)
        {
            minPos = (Vector2)Camera.main.ViewportToWorldPoint(new Vector2(0, 0)) + size;
            maxPos = (Vector2)Camera.main.ViewportToWorldPoint(new Vector2(1, 1)) - size;

            mousePos = (Input.mousePosition);
            Vector2 targetPos = new Vector2(Camera.main.ScreenToWorldPoint(mousePos).x, Camera.main.ScreenToWorldPoint(mousePos).y);

            targetPos.x = Mathf.Clamp(targetPos.x, minPos.x, maxPos.x);
            targetPos.y = Mathf.Clamp(targetPos.y, minPos.y, maxPos.y);
            
            Vector2 direction = targetPos - (Vector2)this.transform.position;
            float speed = Mathf.Min(MaxSpeed, direction.magnitude * 5);
            gnatBody.velocity = direction.normalized * speed;
            if (gnatBody.velocity.x < 0f) {
                this.GetComponents<SpriteRenderer>()[0].flipX = true;
            } else {
                this.GetComponents<SpriteRenderer>()[0].flipX = false;
            }
        }
        else
        {
            if ((Vector2)Input.mousePosition != frozenMousePos) {
                freeze = false;
            }
        }
    }

    public void OnSwapEvent()
    {
        freeze = true;
        frozenMousePos = Input.mousePosition;
        gnatBody.velocity = Vector2.zero;
    }
}
