using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Construction : MonoBehaviour {
  

      [System.Serializable]
    public struct InputOutput
    {

        public GameObject[] Inputs;
        public bool[] InputCheck;
        public GameObject[] Outputs;
    }

      [SerializeField]
      private ConstructionMaster _ConsMang;

      public InputOutput IO; //  public InputOutput[] InputOuts;

    void Awake()
      {
          IO.InputCheck = new bool[IO.Inputs.Length];
      }

    public bool AddToConstruction(ID.Specification spec, ID.Type type, int var)
    {
        for(int i = 0; i < IO.Inputs.Length; i++)
        {
            if(IO.Inputs[i].GetComponent<ID>().Compare(spec, type, var))
            {
                if(IO.InputCheck[i] == false)
                {
                    IO.InputCheck[i] = true;

                    /*
                    bool requirementsComplete = true;
                    for (int j = 0; j < IO.InputCheck.Length; j++)
                    {
                        if (IO.InputCheck[j] == false) requirementsComplete = false;
                    }

                    if(requirementsComplete)
                    {
                        Construct();
                    }*/
                    Construct();

                    return true;
                }

            }
        }


        return false;
    }

    private void Construct()
    {
        for (int i = 0; i < IO.Outputs.Length; i++)
        {
            IO.Outputs[i].SetActive(true);
        }

        if(_ConsMang)
        _ConsMang.ConstructedPartComplete();

        Destroy(this.gameObject);
    }

}
