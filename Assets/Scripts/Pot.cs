using UnityEngine;

public class Pot : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.name == "Gnat") {
            PotGenerator.processPotEnter(this.gameObject);
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if (col.name == "Gnat") {
            PotGenerator.processPotExit(this.gameObject);
        }
    }
}
