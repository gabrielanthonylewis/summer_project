using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AttackBox : MonoBehaviour {

    //public bool withinAttackBox = false;

    // LIST ON TRANSFORM INSIDE
    List<Transform> EnteredTransforms = new List<Transform>();

	// ON TRIGGERENTER = TRUE
    // ON EXIT = FALSE

    void OnTriggerEnter(Collider other)
    {
        if (!other.transform.GetComponent<ID>()) return;
        if (other.transform.GetComponent<ID>()._LifeForm == ID.Lifeform.NULL) return;

        EnteredTransforms.Add(other.transform);
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.transform.GetComponent<ID>()) return;
        if (other.transform.GetComponent<ID>()._LifeForm == ID.Lifeform.NULL) return;
        if (!EnteredTransforms.Contains(other.transform)) return;
        EnteredTransforms.Remove(other.transform);
    }


    public bool TargetWithinAttackBox(Transform target)
    {
        if (EnteredTransforms.Contains(target)) return true;

        return false;
    }
}
