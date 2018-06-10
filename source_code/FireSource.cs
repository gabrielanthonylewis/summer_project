using UnityEngine;
using System.Collections;

public class FireSource : MonoBehaviour {

    public bool isLit = false;

    public bool externalFireAdded = false;

    public Light _Light;

    public bool onFireSurface = false;

    public Transform _FireSurface;
    FirePlaceManager _FirePlaceManager = null;

    void Start()
    {
        Debug.Log(this.transform.name);
        if (isLit)
            LightFire();
    }

    void Update()
    {
        // if not on fire surface then burn out.
        if(isLit == true && onFireSurface == false)
            BurnOut();
    }


    void BurnOut()
    {
        isLit = false;
        Debug.Log("BURNOUT");
    }


    public void LightFire()
    {
        isLit = true;

        _FirePlaceManager =  _FireSurface.gameObject.AddComponent<FirePlaceManager>();
        _Light = _FirePlaceManager.Initialise(_Light);
        _Light.enabled = true;
        Debug.Log("ISLIT!!");
    }

    // ON COLLISION ENTER..? J,,
    void OnTriggerStay(Collider other)
    {
   
   

        if (other.GetComponent<SpawnBatch>()) return;
        if (!other.GetComponent<ID>() || other.gameObject.layer.ToString() == "Terrain")
        {
            //Debug.Log(other.name);
            onFireSurface = false;
            return;
        }
        //Debug.Log(other.name);
        /*
        if (externalFireAdded)
        {
            this.GetComponent<FireSource>().LightFire();
            externalFireAdded = false;
        }*/

        if (other.GetComponent<ID>().isFireSurface)
        {
            onFireSurface = true;
            _FireSurface = other.transform;
            if(isLit)
            {

            }

        }else
        {
          //  onFireSurface = false;
        }
    }

    void OnTriggerExit(Collider other)
    {
      
        if (!other.GetComponent<ID>()) return;
       // if (!isLit) return;

        if (other.GetComponent<ID>().isFireSurface)
        {
            //BurnOut();
            //onFireSurface = false;
            //put out fire?
            // I NEED TO WORK THIS ALL OUT AGAIN
        }
    }

}
