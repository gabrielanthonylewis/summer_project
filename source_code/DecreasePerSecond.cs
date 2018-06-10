using UnityEngine;
using System.Collections;

public class DecreasePerSecond : MonoBehaviour {

	public void StarDecrease() {
        StartCoroutine("WaitThenDecrease");
       // Debug.Log("ENDED");
	}

    IEnumerator WaitThenDecrease()
    {
        //Debug.Log("CALLED");
        for (; ; )
        {
            yield return new WaitForSeconds(0.5f);
            UIManager.Instance.AdjustBar(-10F, UIManager.Bars.E);
        }
      
    }

}
