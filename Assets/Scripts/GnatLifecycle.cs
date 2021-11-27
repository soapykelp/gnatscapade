using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class UnityEventVector2 : UnityEvent<Vector2>
{
}

public class GnatLifecycle : MonoBehaviour
{
    [SerializeField]
    private GameObject fakeGnatPrefab;

    public enum DeathReason {
        OldAge,
        Clapped,
        Sprayed,
    }

    public static DeathReason lastDeathReason;
    public static int currentDay = 0;

    public List<GnatScriptableObject> gnatList;
    public UnityEvent swapEvent;
    public UnityEventVector2 clapEvent;

    private const float currentGnatSecondsToLive = 30f; 
    private const float secondsPerDay = 4f;
    private const float secondsToLive = 12f;
    private const int maxNumSprays = 2;

    private (float min, float max) clapIntervalSecs = (2f, 5f);
    private bool spraying;
    private float clapTargetTime;
    private float clapTimer;
    private float currentGnatAliveTimer;   
    private float dayTimer;
    private float gnatTargetThreshold;
    private float screenHeight;
    private float screenWidth;
    private Vector2 maxPos;
    private Vector2 minPos;

    public void createGnat(Vector2 potPosition)
    {
        GnatScriptableObject gnat = ScriptableObject.CreateInstance<GnatScriptableObject>();
        gnat.position = potPosition;
        gnat.minBound = new Vector2(gnat.position.x - screenWidth*0.25f, minPos.y + screenHeight*0.1f);
        gnat.maxBound = new Vector2(gnat.position.x + screenWidth*0.25f, maxPos.y - screenHeight*0.1f);
        gnat.targetPos = new Vector2(Random.Range(gnat.minBound.x, gnat.maxBound.x),
                                     Random.Range(gnat.minBound.y, gnat.maxBound.y));
        gnatList.Add(gnat);
        Debug.Log("New gnat hatched at " + potPosition);
    }

    private void destroyGnat(GnatScriptableObject gnat)
    {
        Debug.Log("Gnat killed at " + gnat.position);
        if (gnat.gnatObject != null) {
            Destroy(gnat.gnatObject);
        }
        gnatList.Remove(gnat);
        ScriptableObject.Destroy(gnat);
    }

    private void newGnatPosition(GnatScriptableObject gnat)
    {
        // New target
        if (Vector2.Distance(gnat.position, gnat.targetPos) < gnatTargetThreshold) {
            gnat.targetPos = new Vector2(Random.Range(gnat.minBound.x, gnat.maxBound.x),
                                         Random.Range(gnat.minBound.y, gnat.maxBound.y));
        }

        Vector2 newPos = Vector2.Lerp(gnat.position, gnat.targetPos, 0.2f*Time.fixedDeltaTime);
        gnat.position = newPos;
    }

    private void swapGnat(DeathReason reason)
    {
        lastDeathReason = reason;

        if (gnatList.Count == 0)
        {
            Debug.Log("Game over");
            FindObjectOfType<GameManager>().EndGame();
            return;
        }

        float maxGnatX = gnatList[0].position.x;
        GnatScriptableObject maxGnat = gnatList[0];

        foreach (GnatScriptableObject gnat in gnatList)
        {
            if (gnat.position.x > maxGnatX) {
                maxGnatX = gnat.position.x;
                maxGnat = gnat;
            }
        }
        gnatList.Remove(maxGnat);
        ScriptableObject.Destroy(maxGnat);

        Debug.Log("Swapping Old gnat " + this.transform.position + " New gnat " + maxGnat.position);
        currentGnatAliveTimer = 0;
        this.transform.position = maxGnat.position;
        destroyGnat(maxGnat);

        swapEvent.Invoke();
    }

    private bool isInView(Vector2 position)
    {
        minPos = (Vector2)Camera.main.ViewportToWorldPoint(new Vector2(0, 0));
        maxPos = (Vector2)Camera.main.ViewportToWorldPoint(new Vector2(1, 1));
        return (position.x > minPos.x && position.x < maxPos.x);
    }

    private void clap(Vector2 position)
    {
        Debug.Log("Clap!");
        if (clapIntervalSecs.min > 1f) {
            clapIntervalSecs.min -= 0.02f;
        }
        if (clapIntervalSecs.max > 2f) {
            clapIntervalSecs.max -= 0.02f;
        }
        clapEvent.Invoke(position);
    }

    void Start()
    {
        currentGnatAliveTimer = 0f;
        gnatList = new List<GnatScriptableObject>();

        if (swapEvent == null) {
            swapEvent = new UnityEvent();
        }

        if (clapEvent == null) {
            clapEvent = new UnityEventVector2();
        }

        minPos = (Vector2)Camera.main.ViewportToWorldPoint(new Vector2(0, 0));
        maxPos = (Vector2)Camera.main.ViewportToWorldPoint(new Vector2(1, 1));
        screenWidth = maxPos.x - minPos.x;
        screenHeight = maxPos.y - minPos.y;
        gnatTargetThreshold = (new Vector2(screenWidth*0.1f, screenHeight*0.1f)).magnitude;

        currentDay = 0;
    }

    void FixedUpdate()
    {
        // Day count
        dayTimer += Time.fixedDeltaTime;
        if (dayTimer > secondsPerDay) 
        {
            currentDay++;
            dayTimer = 0;
        }

        foreach (GnatScriptableObject gnat in gnatList)
        {
            // Age gnat
            gnat.aliveTimer += Time.fixedDeltaTime;

            // Move gnat
            Vector2 oldPosition = gnat.position;
            newGnatPosition(gnat);
            if (gnat.gnatObject != null) {
                gnat.gnatObject.transform.position = gnat.position;
            }

            // Render gnat
            if (gnat.gnatObject == null && isInView(gnat.position)) {
                gnat.gnatObject = GameObject.Instantiate(fakeGnatPrefab, gnat.position, Quaternion.identity);
            }

            if (isInView(gnat.position)) {
                if (gnat.position.x - oldPosition.x < 0) {
                    gnat.gnatObject.GetComponents<SpriteRenderer>()[0].flipX = true;
                } else {
                    gnat.gnatObject.GetComponents<SpriteRenderer>()[0].flipX = false;
                }
            }

            // Unrender gnat
            if (gnat.gnatObject != null && !isInView(gnat.position)) {
                Destroy(gnat.gnatObject);
            }

            // Kill gnat
            if (gnat.aliveTimer > secondsToLive)
            {
                destroyGnat(gnat);
                break;
            }
        }

        currentGnatAliveTimer += Time.fixedDeltaTime;
        if (currentGnatAliveTimer > currentGnatSecondsToLive) 
        {
            swapGnat(DeathReason.OldAge);
        }

        if (!spraying) {
            if (clapTimer == 0f) {
                clapTargetTime = Random.Range(clapIntervalSecs.min, clapIntervalSecs.max);
            } else if (clapTimer > clapTargetTime) {
                clap(transform.position);
                clapTimer = 0f;
                clapTargetTime = Random.Range(clapIntervalSecs.min, clapIntervalSecs.max);
            }
            clapTimer += Time.fixedDeltaTime;   
        }  
    }

    public int getTotalGnatCount()
    {
        return gnatList.Count + 1;
    }

    public Vector2 getCurrentGnatPosition()
    {
        return transform.position;
    }

    public int getLifespan()
    {
        return (int)(100 - (currentGnatAliveTimer/currentGnatSecondsToLive * 100f));
    }

    public void onHitEvent()
    {
        swapGnat(DeathReason.Clapped);
    }

    public void sprayedGnat()
    {
        swapGnat(DeathReason.Sprayed);
    }

    public float getFarthestGnatPosition()
    {
        if (gnatList.Count > 0) {
            float minX = float.MaxValue;
            foreach (GnatScriptableObject gnat in gnatList) {
                if (gnat.position.x < minX) {
                    minX = gnat.position.x;
                }
            }
            return minX;
        } else {
            return float.MaxValue;
        }
    }
  
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.name == "Pot(Clone)") {
            clapTimer = 0f;
            spraying = true;
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if (col.name == "Pot(Clone)") {
            clapTimer = 0f;
            spraying = false;
        }
    }
}
