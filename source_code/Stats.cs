using UnityEngine;
using System.Collections;

public class Stats : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}

    bool routine = false;
    bool warmthRoutine = false;
    bool healingRoutine = false;

    public bool playerWithinWarmth = false;
    public bool playerWithinShelter = false;
	
	// Update is called once per frame
	void Update () {

        // if player is staying in cold
        if(!warmthRoutine)
        {
            StartCoroutine("ReduceWarmth");
        }

        
        if (!routine && (UIManager.Instance.GetBar(UIManager.Bars.Food).currentValue <= 0f || UIManager.Instance.GetWarmthValue() <= 0f))
        {
            StartCoroutine("WaitThenDamageHealth");
        }

        if(!healingRoutine 
             && playerWithinWarmth == true
            && UIManager.Instance.GetBar(UIManager.Bars.Food).currentValue >= 85f
            && UIManager.Instance.GetBar(UIManager.Bars.Hydration).currentValue >= 85f
            && UIManager.Instance.GetBar(UIManager.Bars.Health).currentValue < UIManager.Instance.GetBar(UIManager.Bars.Health).totalValue
            && UIManager.Instance.GetWarmthState() == UIManager.WarmthState.Hot)      
        {
            StartCoroutine("WaitThenHeal");
        }

        if(UIManager.Instance.GetBar(UIManager.Bars.Health).currentValue <= 0f)
        {
            //Debug.Log("TEMP");
            Cursor.visible = true;
            Application.LoadLevel(0);
        }
	}

    IEnumerator ReduceWarmth()
    {
        warmthRoutine = true;
        yield return new WaitForSeconds(1F);

        UIManager.Instance.AdjustWarmthValue(-1, false, ID.Specification.NULL);

        warmthRoutine = false;
    }

    IEnumerator WaitThenDamageHealth()
    {
        routine = true;
        yield return new WaitForSeconds(5.5f);


        if (UIManager.Instance.GetBar(UIManager.Bars.Food).currentValue > 0f && UIManager.Instance.GetWarmthValue() > 0f)
        {
            routine = false;
            StopCoroutine("WaitThenDamageHealth");
            yield return null;
        }

        UIManager.Instance.AdjustBar(-15F, UIManager.Bars.Health);
        routine = false;
       
    }

    IEnumerator WaitThenHeal()
    {
        healingRoutine = true;
        yield return new WaitForSeconds(3f);

        if (playerWithinWarmth == false
            || UIManager.Instance.GetBar(UIManager.Bars.Food).currentValue < 85f
            || UIManager.Instance.GetBar(UIManager.Bars.Hydration).currentValue < 85f
            || UIManager.Instance.GetBar(UIManager.Bars.Health).currentValue >= UIManager.Instance.GetBar(UIManager.Bars.Health).totalValue
            || UIManager.Instance.GetWarmthState() != UIManager.WarmthState.Hot)
        {
            healingRoutine = false;
            StopCoroutine("WaitThenHeal");
            yield return null;
        }

        UIManager.Instance.AdjustBar(+10F, UIManager.Bars.Health);

        healingRoutine = false;

    }
}
