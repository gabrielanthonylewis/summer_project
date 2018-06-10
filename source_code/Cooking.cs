using UnityEngine;
using System.Collections;

public class Cooking : MonoBehaviour {

    
    struct CookingSlot
    {
        public bool used;
        public int slotIndex;
        public GameObject obj;
        public Transform slot;
        public bool isActive;
    };
    public Transform[] CookingSlots;
    private CookingSlot[] _CookingSlots;

    void Awake()
    {
        _CookingSlots = new CookingSlot[CookingSlots.Length];

        for (int i = 0; i < _CookingSlots.Length; i++)
        {
            _CookingSlots[i].used = false;
            _CookingSlots[i].slotIndex = i;
            _CookingSlots[i].slot = CookingSlots[i];
            _CookingSlots[i].isActive = false;
        }

      
    }


    public void SetSlotsActive(bool value)
    {
        for (int i = 0; i < _CookingSlots.Length; i++)
        {
            _CookingSlots[i].isActive = value;
        }
    }

    public void CookItem(GameObject obj, bool passIF)
    {
        if (!passIF)
        {
            if (!CookingSlotFree()) return;
        }

        // DROP ITEM.

        Consumable objConsumable = obj.GetComponent<Consumable>();

        obj.layer = 0;
        obj.GetComponent<Rigidbody>().isKinematic = true;
        obj.GetComponent<Rigidbody>().useGravity = false;

        for (int i = 0; i < obj.GetComponent<ID>().Colliders.Length; i++)
        {
            obj.GetComponent<ID>().Colliders[i].enabled = false;
        }
        //obj.transform.position = 

        int index = FirstFreeCookingSlot();
        
        _CookingSlots[index].used = true;
        _CookingSlots[index].obj = obj;


        obj.transform.position = _CookingSlots[index].slot.position; // + new Vector3(0f, obj.GetComponentInChildren<Collider>().bounds.extents.y, 0f)?

        StartCoroutine(CookAfter(objConsumable.timeToCook, obj, objConsumable, index));

       


    }

    public bool CookingSlotFree()
    {
        for(int i = 0; i < CookingSlots.Length; i++)
        {
            if (_CookingSlots[i].used == false && _CookingSlots[i].isActive) return true; 
        }
        Debug.Log("FALSE!");
        return false;
    }

    public int FirstFreeCookingSlot()
    {
        for (int i = 0; i < CookingSlots.Length; i++)
        {
            if (_CookingSlots[i].used == false && _CookingSlots[i].isActive) return i;  
        }
        return -1;
    }

    IEnumerator CookAfter(float time, GameObject Obj, Consumable consu, int index)
    {
        FirePlace _fp = this.GetComponentInChildren<FirePlace>();
        float iterator = time;
        //Debug.Log(iterator);
        bool isRoutine = false;
        bool stop = false;
        while(iterator > 0)
        {
            Debug.Log("DOWHILE: " + iterator.ToString());
            if (isRoutine == false)
            {
                if (_fp.isLit == false)
                {
                    Obj.layer = 8;

                    // COULD MAKE SO IT CARRIES ON IF LIT INSTEAD OF THROWING IT OFF.
                    _CookingSlots[index].used = false;
                    _CookingSlots[index].obj = null;

                    for (int i = 0; i < Obj.GetComponent<ID>().Colliders.Length; i++)
                    {
                        Obj.GetComponent<ID>().Colliders[i].enabled = true;
                    }

                    Obj.GetComponent<Rigidbody>().isKinematic = false;
                    Obj.GetComponent<Rigidbody>().useGravity = true;
                         stop = true;
                    yield return null;
                    StopCoroutine("CookAfter");
                    break;
                }

                isRoutine = true;
                yield return new WaitForSeconds(1);
                iterator--;
                consu.timeToCook--; // remove line if want to start from start
                isRoutine = false;
            }
        }

        // every second -= 1
        // check if fireplace still lit
        if (stop == false)
        {
            Debug.Log("enterpass");
            consu.enabled = true;
            consu.Cook();
            Obj.layer = 8;

            _CookingSlots[index].used = false;
            _CookingSlots[index].obj = null;

            for (int i = 0; i < Obj.GetComponent<ID>().Colliders.Length; i++)
            {
                Obj.GetComponent<ID>().Colliders[i].enabled = true;
            }
        }
      
        
    }
}
