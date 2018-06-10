using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Cave : MonoBehaviour {

    public DayNightCycle _daynight;
    public List<GameObject> Habitats = new List<GameObject>();
  
    void Start()
    {
        _daynight = GameObject.FindGameObjectWithTag("Sun").GetComponent<DayNightCycle>();
    }

    void Update()
    {
     

        if (_daynight._DayState == DayNightCycle.DayState.Night)
        {
            for (int i = 0; i < Habitats.Count; i++)
            {
                Habitats[i].SetActive(true);
            }

   
            this.GetComponent<Cave>().enabled = false;
            // SET HABITATS TRUE!!!
            // maybe add Roamer option or dramatically increase cut off time for animals so that they can travel further.
            // I know! Increase time AND increase box collider!
        }
        else if (_daynight._DayState == DayNightCycle.DayState.PrepareDay)
        {
            /*
            for (int i = 0; i < Habitats.Count; i++)
            {
                Habitats[i].SetActive(false);
            }*/
        }
    }
}
