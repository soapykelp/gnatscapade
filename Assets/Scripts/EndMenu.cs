using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndMenu : MonoBehaviour
{
    public Text deathText;
    public Text dayText;
    public Text triviaText;

    private string[] trivias = new string[] {
        "Did you know? You respawn at the closest gnat's location if there is another gnat is alive.",
        "Did you know? Pots that have been sprayed twice become infertile.",
        "Did you know? Non-player gnats have limited lifespans too.",
        "Did you know? When a pot becomes infertile all eggs that have been laid in it die.",
        "Did you know? Clapping and spraying get faster the higher the day count.",
        "Did you know? Your lifespan is indicated by the red bar at the top-right corner of the screen.",
        "Did you know? Once a pot becomes infertile you cannot lay eggs in it anymore.",        
    };
    private static int currentTrivia = 0;

    void Start()
    {
        switch (GnatLifecycle.lastDeathReason) {
            case GnatLifecycle.DeathReason.Clapped:
                deathText.text = "flattened by sweaty human palms";
                break;
            case GnatLifecycle.DeathReason.Sprayed:
                deathText.text = "drowned in toxic rain";
                break;
            case GnatLifecycle.DeathReason.OldAge:
                deathText.text = "died of old age";
                break;
            default:
                deathText.text = "cosmic radiation flipped some bits, unlucky";
                break;
        }    

        dayText.text = GnatLifecycle.currentDay.ToString();

        triviaText.text = trivias[currentTrivia++ % trivias.GetLength(0)];
    }

    void Update()
    {
    }

    public void OnRestartButton()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene("MainScene");
    }

    public void OnMainMenuButton()
    {
        Time.timeScale = 1f;
    
        SceneManager.LoadScene("StartScene");
    }
}
