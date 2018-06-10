using UnityEngine; // Includes the "UnityEngine" namespace.
using System.Collections; // Includes the "System.Collections" namespace.

// Class provides functionality to change screens on the main menu.
public class MainMenu : MonoBehaviour
{
    private GameObject _currScreen; // The current screen being viewed.
    public GameObject StartScreen = null; // The screen of which the player first views.

    // Start gets called once the scene is loaded but after "Awake()", used for initialization.
    void Start()
    {
        _currScreen = StartScreen; // Sets the _currScreen to the the current scene which initally is the starting screen.
    }

    // Changes the screen depending on the GameObject (screen) given.
    public void ChangeScreenTo(GameObject gameObject)
    {
        _currScreen.SetActive(false); // Hides the current screen.
        gameObject.SetActive(true); // Shows the new screen.
        _currScreen = gameObject; // Sets the _currScreen to the new current screen.

     

    }



}
