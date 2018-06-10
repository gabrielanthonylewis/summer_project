using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour {

    public Toggle refShowAllToggle = null;
    public GameObject refProximityCheckGO = null;
    public GameObject refCraftContent = null;
    public GameObject refSlotContent = null;
    public GameObject refProximityContent = null;

    public GameObject craftOptionPrefab = null;
    public GameObject slotPrefab = null;
    public GameObject proximityOptionPrefab = null;

	// Use this for initialization
	void Awake () {
	
        if(refShowAllToggle == null)
            Debug.LogError("refShowAllToggle == null");
        if (refProximityCheckGO == null)
        {
            refProximityCheckGO = GameObject.FindGameObjectWithTag("ProximityCheck");
            if(refProximityCheckGO == null)
              Debug.LogError("refProximityCheckGO == null");
        }
        if (refCraftContent == null)
            Debug.LogError("refCraftContent == null");
        if (refSlotContent == null)
            Debug.LogError("refSlotContent == null");
        if (refProximityContent == null)
            Debug.LogError("refProximityContent == null");
        if (craftOptionPrefab == null)
            Debug.LogError("craftOptionPrefab == null");
        if (slotPrefab == null)
            Debug.LogError("slotPrefab == null");
        if (proximityOptionPrefab == null)
            Debug.LogError("proximityOptionPrefab == null");
	}
	

}
