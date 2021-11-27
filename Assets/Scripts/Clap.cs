using UnityEngine;
using UnityEngine.Events;

public class Clap : MonoBehaviour
{
    [SerializeField]
    public GnatLifecycle gnatLifecycle;
    
    public UnityEvent hitEvent;

    private Animator animator;
    private Collider2D handCollider;
    private Collider2D gnatCollider;

    void Start()
    {
        AnimationEvent evt = new AnimationEvent();

        animator = gameObject.GetComponent<Animator>();

        foreach (AnimationClip clip in animator.runtimeAnimatorController.animationClips) {
            if (clip.name == "Clapped") {
                evt.time = 0f;
                evt.functionName = "ProcessClappedEvent";
                clip.AddEvent(evt);
                break;
            }
        }

        gnatCollider = gnatLifecycle.gameObject.GetComponent<BoxCollider2D>();
        handCollider = GetComponent<BoxCollider2D>();

        if (hitEvent == null) {
            hitEvent = new UnityEvent();
        }
    }

    public void ProcessClappedEvent()
    {
        if (Physics2D.IsTouching(handCollider, gnatCollider)) {
            hitEvent.Invoke();
        }
        animator.speed = animator.speed * 1.05f;
    }

    public void onClapEvent(Vector2 gnatPosition)
    {
        transform.position = gnatPosition;
        animator.SetTrigger("StartClapping");
    }
}
