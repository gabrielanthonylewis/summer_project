using UnityEngine;
using System.Collections;

public class Shelter : MonoBehaviour {
	
    void OnTriggerEnter(Collider other)
    {
        if (!other.GetComponent<Stats>()) return;

        other.GetComponent<Stats>().playerWithinShelter = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.GetComponent<Stats>()) return;

        other.GetComponent<Stats>().playerWithinShelter = false;
    }

}
