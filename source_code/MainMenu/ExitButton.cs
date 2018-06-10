using UnityEngine; // Includes the "UnityEngine" namespace.
using System.Collections; // Includes the "System.Collections" namespace.

// Class provides fuctionallity for the exit button and in turn manages the exit message GUI.
public class ExitButton : MonoBehaviour 
{

    // "Awake" function is called when the scene is first loaded.
    void Awake()
    {
        ShowExitMessage(false); // Initially hides the Exit message GUI
    }

    // "ExitGame" function exits the application completely.
    public void ExitGame()
    {
        Application.Quit(); // Properly exits the application.
    }

    // "ShowExitMessage" function allows the Exit message GUI to be shown or hidden depending on the value of a bool "showExitMessage".
    public void ShowExitMessage(bool showExitMessage)
    {
        this.gameObject.SetActive(showExitMessage); // Shows/Hides the Exit message GUI depending on the value of the bool "showExitMessage".
    }
}
