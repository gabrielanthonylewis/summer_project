using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Initialise : MonoBehaviour {

    public bool BerryColors = true;
    public bool UserInterface = true;

          List<Color> Colors = new List<Color>();
      public Color Color1 = Color.white;
      public Color Color2 = Color.white;

   
   

	// Use this for initialization
	void Awake () {

        if(UserInterface)
         UIManager.Instance.Initialise();

        if (!BerryColors) return;
        Colors.Add(Color.blue);
        Colors.Add(Color.cyan);
        Colors.Add(Color.green);
        Colors.Add(Color.magenta);
        Colors.Add(Color.red);
        Colors.Add(Color.yellow);

        Color1 = Colors[Random.Range(0, Colors.Count)];

        Color2 = Colors[Random.Range(0, Colors.Count)];
        while (Color2 == Color1)
        {
            Color2 = Colors[Random.Range(0, Colors.Count)];
        }
	}
	

}
