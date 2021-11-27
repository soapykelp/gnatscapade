using UnityEngine;

public class LifespanBar : MonoBehaviour
{
    [SerializeField]
    public GnatLifecycle GnatLifecycleScript;

    void FixedUpdate()
    {
        GetComponent<UnityEngine.UI.Slider>().value = GnatLifecycleScript.getLifespan()/100f;
    }
}
