using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class UnityEventPot : UnityEvent<PotScriptableObject>
{
}

public class PotGenerator : MonoBehaviour
{
    [SerializeField]
    private GameObject potPrefab;
    [SerializeField]
    private GameObject potInfertilePrefab;

    public static List<PotScriptableObject> potList;
    public static int currentLayProgress;
    public UnityEventPot sprayEvent;

    private static LayProgressBar progressBar;
    private const int maxNumSprays = 2;
    private const float secondsToHatch = 4f;
    private const float secondsToLay = 2f; 
    private (float min, float max) sprayIntervalSecs = (2f, 5f);
    private float screenWidth = 0f;
    private float sprayTargetTime;
    private float sprayTimer;
    private int totalEggCount;
    private Vector2 maxPos;
    private Vector2 minPos;
    private Vector2 nextPotPos;
    private Vector2 potSize;

    private Vector2 nextPotPosition(Vector2 current)
    {
        float fixedDistance = screenWidth * 0.2f;
        float randomDistance = Random.Range(0.0f, screenWidth);
        return new Vector2(fixedDistance + randomDistance + current.x, current.y);
    }

    private void randomlyRenderPlant(GameObject parent)
    {
        SpriteRenderer[] childrenList = parent.GetComponentsInChildren<SpriteRenderer>(true);
        int count = childrenList.GetLength(0);
        int random = Random.Range(1, count);
        // Start from 1 because list includes self
        for (int i = 1; i < count; i++) {
            childrenList[i].enabled = false;
        }
        childrenList[random].enabled = true;
    }

    private void createPot(Vector2 position)
    {
        PotScriptableObject pot = ScriptableObject.CreateInstance<PotScriptableObject>();
        pot.potObject = GameObject.Instantiate(potPrefab, position, Quaternion.identity);
        randomlyRenderPlant(pot.potObject);
        potList.Add(pot);
        nextPotPos = nextPotPosition(position);
    }

    private void spray(PotScriptableObject pot)
    {
        Debug.Log("Spray!");
        pot.spraying = true;
        if (sprayIntervalSecs.min > 1f) {
            sprayIntervalSecs.min -= 0.02f;
        }
        if (sprayIntervalSecs.max > 1f) {
            sprayIntervalSecs.max -= 0.02f;
        }
        
        sprayEvent.Invoke(pot);
    }

    void Start()
    {
        potList = new List<PotScriptableObject>();

        minPos = (Vector2)Camera.main.ViewportToWorldPoint(new Vector2(0, 0));
        maxPos = (Vector2)Camera.main.ViewportToWorldPoint(new Vector2(1, 1));
        potSize = potPrefab.GetComponent<SpriteRenderer>().bounds.extents;
        screenWidth = maxPos.x - minPos.x;

        // Generate a pot 
        Vector2 initialStart = new Vector2(screenWidth * 0.5f + minPos.x, minPos.y + 0.4f + potSize.y);
        createPot(initialStart);

        progressBar = FindObjectOfType<LayProgressBar>();
        progressBar.DisableLayProgress();

        if (sprayEvent == null) {
            sprayEvent = new UnityEventPot();
        }
    }

    void FixedUpdate()
    {
        minPos = (Vector2)Camera.main.ViewportToWorldPoint(new Vector2(0, 0));
        maxPos = (Vector2)Camera.main.ViewportToWorldPoint(new Vector2(1, 1));

        // Generate
        if (maxPos.x + potSize.x * 3f > nextPotPos.x)
        {
            createPot(nextPotPos);
        }

        totalEggCount = 0;
        foreach (PotScriptableObject pot in potList)
        {
            // Egg laying progress
            if (pot.hasGnat && pot.sprayedTimes < maxNumSprays) {
                pot.layTimer += Time.fixedDeltaTime;
                pot.layProgress = (pot.layTimer / secondsToLay) * 100f;
                if (pot.layTimer > secondsToLay) {
                    Debug.Log("New egg at " + pot.potObject.transform.position);
                    pot.layTimer = 0f;
                    pot.eggCount++;
                }

                if (sprayTimer == 0f) {
                    sprayTargetTime = Random.Range(sprayIntervalSecs.min, sprayIntervalSecs.max);
                } else if (sprayTimer > sprayTargetTime) {
                    spray(pot);
                    sprayTimer = 0f;
                    sprayTargetTime = Random.Range(sprayIntervalSecs.min, sprayIntervalSecs.max);
                }
                sprayTimer += Time.fixedDeltaTime;

                currentLayProgress = (int)(pot.layTimer / secondsToLay * 100);
            } 

            totalEggCount += pot.eggCount;
        }
    }

    public void destroyPots(float farthestGnatPos)
    {
        foreach (PotScriptableObject pot in potList)
        {
            float potX = pot.potObject.transform.position.x + potSize.x;
            if (potX < minPos.x
               && pot.eggCount == 0
               && farthestGnatPos > potX + screenWidth)
            {
                Destroy(pot.potObject);
                potList.Remove(pot);
                ScriptableObject.Destroy(pot);
                break;
            }
        }
    }

    public List<Vector2> checkHatched()
    {
        List<Vector2> hatchList = new List<Vector2>();
        foreach (PotScriptableObject pot in potList)
        {
            // Hatching
            if (pot.eggCount > 0) {
                pot.hatchTimer += Time.fixedDeltaTime;
                if (pot.hatchTimer > secondsToHatch) {
                    // Instantiate clone adult gnat
                    hatchList.Add(pot.potObject.transform.position);
                    pot.eggCount--;
                    pot.hatchTimer = 0;
                }
            }
        }

        return hatchList;
    }

    public int getTotalEggCount()
    {
        return totalEggCount;
    }

    public bool processSprayedEventPot()
    {
        bool sprayedGnat = false;
        foreach (PotScriptableObject pot in potList) {
            if (pot.spraying) {
                pot.spraying = false;

                if (pot.hasGnat) {
                    sprayedGnat = true;
                }

                pot.sprayedTimes++;
                if (pot.sprayedTimes == maxNumSprays) {
                    Debug.Log("Pot dead");
                    Vector2 potPosition = pot.potObject.transform.position;
                    int enabledPlant = 0;
                    SpriteRenderer[] childrenList = pot.potObject.GetComponentsInChildren<SpriteRenderer>(true);
                    int count = childrenList.GetLength(0);
                    for (int i = 1; i < count; i++) {
                        if (childrenList[i].enabled) {
                            enabledPlant = i;
                        }
                    }
                    Destroy(pot.potObject);

                    pot.potObject = GameObject.Instantiate(potInfertilePrefab, potPosition, Quaternion.identity);
                    childrenList =  pot.potObject.GetComponentsInChildren<SpriteRenderer>(true);
                    count = childrenList.GetLength(0);
                    for (int i = 1; i < count; i++) {
                        childrenList[i].enabled = false;
                    }
                    childrenList[enabledPlant].enabled = true;

                    // Kill all eggs
                    pot.eggCount = 0;
                }
                break;
            }
        }
        return sprayedGnat;
    }

    public static void processPotEnter(GameObject collided)
    {
        foreach (PotScriptableObject pot in potList)
        {
            if (collided == pot.potObject) {
                pot.hasGnat = true;
                progressBar.EnableLayProgress(pot.potObject.transform.position);
                break;
            }
        }
    }

    public static void processPotExit(GameObject collided)
    {
        foreach (PotScriptableObject pot in potList)
        {
            if (collided == pot.potObject) {
                pot.hasGnat = false;
                progressBar.DisableLayProgress();
                currentLayProgress = 0;
                break;
            }
        }
    }
}
