using UnityEngine;

public class DayStat : MonoBehaviour
{
    void FixedUpdate()
    {
        GetComponent<UnityEngine.UI.Text>().text = string.Format("{0}", GnatLifecycle.currentDay);
    }
}
