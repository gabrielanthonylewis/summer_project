using UnityEngine;
using System.Collections;

public class InitialiseAudioClip : MonoBehaviour {

    AudioSource _AudioSource = null;

	// Use this for initialization
	void Awake () {

        _AudioSource = this.GetComponent<AudioSource>();

        if (_AudioSource == null)
        {
            Debug.Log("RTE: No Audio Source found!");
            return;
        }

        _AudioSource.loop = true;
    }

    public void Play()
    {
        if (_AudioSource == null) return;

        _AudioSource.Play();

        float randStart = Random.Range(0f, 1f);

        _AudioSource.time = randStart * _AudioSource.clip.length;

   
	}

    public void Stop()
    {
        _AudioSource.Stop();
    }
	

}
