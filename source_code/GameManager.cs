using UnityEngine;

public class GameManager {


    public int totalPlayers = 1;

	  private static GameManager _instance = null;

      protected GameManager() { }

    // Singleton pattern implementation
      public static GameManager Instance
    {
        get
        {
            if (GameManager._instance == null)
            {
                GameManager._instance = new GameManager();
            }
            return GameManager._instance;
        }
    }



    public void Die()
      {
          Cursor.visible = true;
          Application.LoadLevel(0); 
      }
 
}
