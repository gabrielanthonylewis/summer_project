using UnityEngine; // Includes the "UnityEngine" namespace.
using System.Collections; // Includes the "System.Collections" namespace.
using UnityEngine.UI; // Includes the "UnityEngine.UI" namespace.

// Class allows the scene/level to be changed. For example, from the Main menu to the Game.
public class ChangeScene : MonoBehaviour
{
    
    // "ChangeSceneTo" function changes the scene/level depending on "int SceneNumber"
    public void ChangeSceneTo(int SceneNumber)
    {
        Cursor.visible = true;
        Application.LoadLevel(SceneNumber); // Changes scene depending on the integar "SceneNumber" that has been entered.
    }

}
