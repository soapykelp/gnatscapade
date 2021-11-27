using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private PotGenerator potGenerator;
    [SerializeField]
    private GnatLifecycle gnatLifecycle;

    private bool gameHasEnded = false;

    void FixedUpdate()
    {
        potGenerator.destroyPots(gnatLifecycle.getFarthestGnatPosition());

        List<Vector2> hatchList = potGenerator.checkHatched();
        foreach (Vector2 pos in hatchList) {
            gnatLifecycle.createGnat(pos);
        }
    }
    
    private IEnumerator ShowEndGame(float time)
    {
        yield return new WaitForSecondsRealtime(time);

        SceneManager.LoadScene("GameOverScene");
    }

    public void EndGame()
    {
        if (!gameHasEnded)
        {
            gameHasEnded = true;
            Debug.Log("Game is really over");
            Time.timeScale = 0f;
            StartCoroutine(ShowEndGame(1.2f));
        }
    }

    public void onSprayedEvent()
    {
        if (potGenerator.processSprayedEventPot()) {
            gnatLifecycle.sprayedGnat();
        }
    }
}
