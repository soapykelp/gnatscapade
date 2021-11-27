using UnityEngine;

public class LayProgressBar : MonoBehaviour
{
    void FixedUpdate()
    {
        GetComponent<UnityEngine.UI.Slider>().value = PotGenerator.currentLayProgress/100f;
    }

    public void EnableLayProgress(Vector2 position) {
        this.gameObject.SetActive(true);
        this.transform.position = new Vector2(position.x, this.transform.position.y);
    }

    public void DisableLayProgress() {
        this.gameObject.SetActive(false);
    }
}
