using UnityEngine;
using System.Collections;

public class Durabillity : MonoBehaviour {

    public int currentDurabillity = 100;
    public int totalDmgTaken = 0;

    public void Decrease(int amount)
    {
        currentDurabillity -= amount;
        totalDmgTaken += amount;

        int index = Camera.main.GetComponent<Inventory>().IndexOfItemObject(this.transform.gameObject);

        if(index > -1)
        {
            Debug.Log("TOALDMGTAKEN: " + totalDmgTaken);
            Camera.main.GetComponent<Inventory>().SetDurAtSlotIndex(index, totalDmgTaken);
        }

        if(currentDurabillity <= 0)
        {
            Break();
        }
    }

    public void Break()
    {
        Camera.main.GetComponent<Inventory>().RemoveItem(this.gameObject, 1);
    }
}
