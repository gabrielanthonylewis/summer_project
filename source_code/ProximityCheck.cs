using UnityEngine;
using System.Collections;

public class ProximityCheck : MonoBehaviour {

    // Component References
    private Inventory _Inv = null;

    void Awake()
    {
        _Inv = Camera.main.GetComponent<Inventory>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != 8) return;

        ID _ID = other.transform.GetComponent<ID>();
        if (!_ID) return;
        if (_ID.isRooted) return;

        _Inv.AddProximityItem(_ID.specification, _ID.type, _ID.variation, other.gameObject, _ID.subMenu, _ID.Prefab);
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer != 8) return;

        ID _ID = other.transform.GetComponent<ID>();
        if (!_ID) return;
        if (_ID.isRooted) return;

        _Inv.RemoveProximityItem(other.gameObject);
    }
}
