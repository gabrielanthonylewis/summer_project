using UnityEngine;
using System.Collections;

public class TreeOptimisation : MonoBehaviour {

    Transform Player = null;


    ObjectHealth _ObjectHealth = null;
    BoxCollider _BoxCollider = null;
	// Use this for initialization
	void Start () {

  

        Player = Camera.main.transform.parent.transform;

        _BoxCollider = this.transform.GetComponent<BoxCollider>();
        _ObjectHealth = this.transform.GetComponent<ObjectHealth>();

        OffOn(false);
        StartCoroutine("DistanceCheck");
	}


    IEnumerator DistanceCheck()
    {

        //Debug.Log("CHECK");
        yield return new WaitForSeconds(3f);
        //Debug.Log("CHECK3");
        if ((Player.position.x - this.transform.position.x) < 3f
           && (Player.position.z - this.transform.position.z) < 3f)
            OffOn(true);
        else
            OffOn(false);

        StartCoroutine("DistanceCheck");

    }

    void OffOn(bool boolean)
    {
        if(_ObjectHealth.enabled != boolean)
        _ObjectHealth.enabled = boolean;

        if (_BoxCollider.enabled != boolean)
        _BoxCollider.enabled = boolean;
    }
	

    // 11, 15, 15
    // 14, 15, 16

    // -
    // 3, 0, 1

}
