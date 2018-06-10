using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Camp : MonoBehaviour {

    private int currentPlayers = 0;

    public List<int> EnteredPlayerIDs = new List<int>();

	void OnTriggerEnter(Collider other)
    {
        if(other.transform.tag == "Player")
        {
            if (EnteredPlayerIDs.Contains(other.gameObject.GetInstanceID())) return;
            EnteredPlayerIDs.Add(other.gameObject.GetInstanceID());
            PlayerEnter();
           
        }
    }

    void OnTriggerExit(Collider other)
    {
      
        if(other.transform.tag == "Player")
        {
            if (!EnteredPlayerIDs.Contains(other.gameObject.GetInstanceID())) return;
            EnteredPlayerIDs.Remove(other.gameObject.GetInstanceID());
            PlayerExit(); // maybe pass the gameobject?
        }
    }

    void Update()
    {
       if(currentPlayers == GameManager.Instance.totalPlayers)
       {
           //DISPLAY END ROUND COUNTDOWN?
           UIManager.Instance.ShowHint_EndRound(true);

           if(Input.GetKeyDown(KeyCode.Return))
           {
               EndRound();
           }
       }
        else
           UIManager.Instance.ShowHint_EndRound(false);
    }

    private void EndRound()
    {
        this.GetComponent<ChangeScene>().ChangeSceneTo(0);

    }

    //NOTE OBVO NEED "TOTAL PLAYERS" AND 
    private void PlayerEnter()
    {

        currentPlayers++;
        UIManager.Instance.SetFinishedPlayers(currentPlayers, GameManager.Instance.totalPlayers);
    }

    private void PlayerExit()
    {
        currentPlayers--;
        UIManager.Instance.SetFinishedPlayers(currentPlayers, GameManager.Instance.totalPlayers);
    }


}
