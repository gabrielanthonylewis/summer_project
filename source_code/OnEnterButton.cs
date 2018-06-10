using UnityEngine;
using System.Collections;

public class OnEnterButton : MonoBehaviour {

    public AudioClip ClickClip = null;

    AudioClip defClip = null;

	// Use this for initialization
	void Awake () {

        defClip = this.GetComponent<AudioSource>().clip;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void PlaySound()
    {
        this.GetComponent<AudioSource>().Play();
    }

    public void PlayPress()
    {
        Debug.Log("HEY");
        //this.GetComponent<AudioSource>().pitch = 0.4f;
       // this.GetComponent<AudioSource>().clip = ClickClip;
        this.GetComponent<AudioSource>().Play();
       // this.GetComponent<AudioSource>().clip = defClip;
        //this.GetComponent<AudioSource>().pitch = 0.8f;

    }
}
