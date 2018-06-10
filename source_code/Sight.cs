using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Sight : MonoBehaviour {

    public List<ID.Lifeform> Predators = new List<ID.Lifeform>();

    public bool runFromPredator = true; // make into list in the future (possibly unless "Predators" is seperated into other lists.
   GameObject PreviousPredator = null;

    void Awake()
   {
        if(this.transform.parent.GetComponent<ID>()._LifeForm == ID.Lifeform.Rabbit)
        {
            Predators.Add(ID.Lifeform.Human);
            Predators.Add(ID.Lifeform.Bear);
            Predators.Add(ID.Lifeform.Wolf);
        }

        if (this.transform.parent.GetComponent<ID>()._LifeForm == ID.Lifeform.Wolf)
        {
            Predators.Add(ID.Lifeform.Bear);
            runFromPredator = false;
        }

        if (this.transform.parent.GetComponent<ID>()._LifeForm == ID.Lifeform.Bear)
        {
            Predators.Add(ID.Lifeform.Wolf);
            runFromPredator = false;
        }
   }

    void OnTriggerEnter(Collider other)
    {
  
        if(!other.GetComponent<ID>()) return;

        ID otherID = other.GetComponent<ID>();

        if (otherID._LifeForm == ID.Lifeform.NULL) return;


        // WILL HAVE TO BE 
        // CHANGED!!! 
        // MAYBE BEAR ATTACK BEAR?
        // OR AT LEAST
        // HUMAN ATTACK HUMAN
        // SO DEPENDNGS ON NATURE/BEHAVIOUR

        if (this.transform.parent.GetComponent<ID>()._LifeForm == otherID._LifeForm) return;
        ////
        ////
        ////

        //if (tempID.gameObject == PreviousPredator) return;

        // if predator, run
        if (runFromPredator && Predators.Contains(otherID._LifeForm))
        {
            PreviousPredator = otherID.gameObject;
            this.transform.parent.GetComponent<AINavigation>().PrepareToFlee();
            return;
        }

        // NEEDS TO BE A DECISION WHETHER TO CHASE THE NEW ENTRY OR NOT.
        this.transform.parent.GetComponent<AINavigation>().PrepareToAttack(other.transform);

    

    }

    /*
    void OnTriggerExit(Collider other)
    {
        if (!other.GetComponent<ID>()) return;

        ID tempID = other.GetComponent<ID>();

        if (tempID._LifeForm == ID.Lifeform.NULL) return;

        // 
        Debug.Log("stopattack");
        this.transform.parent.GetComponent<AINavigation>().StopAttack();
    }*/
}
