using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour
{
    public void StartButtonClicked() {
        SceneManager.LoadScene("MainScene");
    }
}
