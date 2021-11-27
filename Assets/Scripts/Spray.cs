using UnityEngine;
using UnityEngine.Events;

public class Spray : MonoBehaviour
{
    private Animator animator;

    public UnityEvent sprayedEvent;

    void Start()
    {
        animator = gameObject.GetComponent<Animator>();

        AnimationEvent evt = new AnimationEvent();

        foreach (AnimationClip clip in animator.runtimeAnimatorController.animationClips) {
            if (clip.name == "Sprayed") {
                evt.time = 0f;
                evt.functionName = "ProcessSprayedEvent";
                clip.AddEvent(evt);
                break;
            }
        }

        if (sprayedEvent == null) {
            sprayedEvent = new UnityEvent();
        }
    }

    public void ProcessSprayedEvent()
    {
        sprayedEvent.Invoke();
        animator.speed = animator.speed * 1.02f;
    }

    public void onSprayEvent(PotScriptableObject pot)
    {
        Vector2 sprayPosition = new Vector2(pot.potObject.transform.position.x - pot.potObject.GetComponent<SpriteRenderer>().bounds.extents.x*1.25f,
                                            pot.potObject.transform.position.y + pot.potObject.GetComponent<SpriteRenderer>().bounds.extents.y*1.25f);
        transform.position = sprayPosition;
        animator.SetTrigger("StartSpraying");
    }
}
