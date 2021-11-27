using UnityEngine;

public class EggStat : MonoBehaviour
{
    [SerializeField]
    public PotGenerator potGenerator;

    void FixedUpdate()
    {
        GetComponent<UnityEngine.UI.Text>().text = string.Format("{0}", potGenerator.getTotalEggCount());
    }
}
