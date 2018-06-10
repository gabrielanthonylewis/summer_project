using UnityEngine;
using System.Collections;

public class ConstructionMaster : MonoBehaviour {

     [SerializeField]
    private Construction[] Parts;

    private int constructedParts = 0;
    private int totalParts;

    [SerializeField]
    private GameObject[] _ActivateOnCompletion;

    void Awake()
    {
        totalParts = Parts.Length;
    }

    public void ConstructedPartComplete()
    {
        constructedParts++;
        if(constructedParts >= totalParts)
        {
            Construct();
        }
    }

    private void Construct()
    {
        for(int i = 0; i < _ActivateOnCompletion.Length; i++)
        {
            _ActivateOnCompletion[i].SetActive(true);
        }
    }
}
