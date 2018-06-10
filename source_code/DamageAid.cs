using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DamageAid : MonoBehaviour {

    bool routine = false;

    Image _Image = null;

    void Start()
    {
        _Image = this.GetComponent<Image>();
        _Image.enabled = false;
    }

    public void Show()
    {
        if(routine == false)
        {
            _Image.enabled = true;
            StartCoroutine("WaitThenOff");
        }
        else
        {
            _Image.enabled = true;
            StopAllCoroutines();
            routine = false;
            StartCoroutine("WaitThenOff");
        }
    }

    IEnumerator WaitThenOff()
    {
        routine = true;

        yield return new WaitForSeconds(0.2f);
        _Image.enabled = false;

        routine = false;
    }
}
