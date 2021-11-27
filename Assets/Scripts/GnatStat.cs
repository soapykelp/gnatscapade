using UnityEngine;

public class GnatStat : MonoBehaviour
{
    [SerializeField]
    public GnatLifecycle GnatLifecycleScript;

    void FixedUpdate()
    {
        GetComponent<UnityEngine.UI.Text>().text = string.Format("{0}", GnatLifecycleScript.getTotalGnatCount());
    }
}
