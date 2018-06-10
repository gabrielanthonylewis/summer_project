using UnityEngine;
using System.Collections;
using System.Xml; // Includes the "System.XML" namespace so that XML can be used to save and load.
using System.IO; // Includes the "System.IO" namespace so that files and directories can be created and manipulated.

public class ItemDataManager : MonoBehaviour {

    private string _folderPath = ""; // Stores the path to the folder "/GameData" where the player's information will be stored.
    private string _filePath = "";  // Stores the path to the folder, including file name which in this case is "gamedata.xml".


	// Use this for initialization
	void Start () {

        _folderPath = Application.dataPath + "/Resources/GameData"; // Assigns "_folderPath" with the folder path of the folder "/GameData".
        _filePath = Application.dataPath + "/Resources/GameData/itemdata.xml"; // Assigns "_filePath" with the file path of the file "gamedata.xml".

        // If the folder doesn't exist.
        if (!Directory.Exists(_folderPath))
            Directory.CreateDirectory(_folderPath); // Create the folder in the assigned directory.

        // If the "gamedata.xml" file doesnt exist, create it in the assigned directory. (Note: second parameter is the information about the directory so it can be searched)
        if (!CheckFile("itemdata.xml", new DirectoryInfo(_folderPath).GetFiles()))
            if (!CreateFile(_filePath)) Debug.Log("Error: File wasn't created"); // Output error to log if the file failed to be created.
     
    }

    // Creates the file depending on the directory given.
    private bool CreateFile(string filePath)
    {
        System.IO.File.WriteAllText(filePath, "<Items>" + "\n" + "</Items>"); // Creates the new file and writes the inital xml structure for later storage of player data.

        XmlDocument xmlDoc = new XmlDocument(); // Creates empty xml document.
        xmlDoc.Load(filePath); // Information regarding the game data xml file is stored into the xmlDoc variable.
        XmlElement elmRoot = xmlDoc.DocumentElement; // The root element is created and assigned to the root element of the xml file.
        elmRoot.RemoveAll(); // Removes all of the data inside the root element.

        //0__ = WEAPONS
        //1__ = TOOLS
        //2__ = CONSUMABLES

        // write function that takes the type, works into the ID using the current count.
        // 1 line to add an item. 

        // AddItem(Inventory.type.Weapon, "Spear", new Vector3(0f, 0f, 0f) ); 

        //ps. maybe merge tool and weapon into one? dunno, some weapons may not be used for anything but killing?

        XmlElement elmPlayerID = xmlDoc.CreateElement("ID_001"); // Create the "PlayerID" element. This will store all of the information about the current player.
        elmRoot.AppendChild(elmPlayerID); // Append the element inside the "PlayerID_" element.

        XmlElement elmPlayerName = xmlDoc.CreateElement("Name"); // Create the "Name" element to store the the players name.
        elmPlayerName.InnerText = "Spear"; // Player's name stored in the element.
        elmPlayerID.AppendChild(elmPlayerName); // "Name" element appended inside the "PlayerID_" element. 

        XmlElement elmRotation = xmlDoc.CreateElement("Rotation");
        elmPlayerID.AppendChild(elmRotation);

        XmlElement elmRotX = xmlDoc.CreateElement("x");
        elmRotX.InnerText = "358.6967";
        elmRotation.AppendChild(elmRotX);

        XmlElement elmRotY = xmlDoc.CreateElement("y");
        elmRotY.InnerText = "11.51559";
        elmRotation.AppendChild(elmRotY);

        XmlElement elmRotZ = xmlDoc.CreateElement("z");
        elmRotZ.InnerText = "294.5846";
        elmRotation.AppendChild(elmRotZ);

        xmlDoc.Save(filePath); // Saves the new changes to the xml file.

        return true; // Return true signaling that the file has been created properly.
    }

    // Checks whether the file exists in the folder files given as "FileInfo[]".
    private bool CheckFile(string filename, FileInfo[] fileInfo)
    {
        // Loop through all of the files information stored in "fileInfo".
        for (int i = 0; i < fileInfo.Length; i++)
        {
            // If the current file name is equal to the file name inputted.
            if (fileInfo[i].Name == filename)
                return true; // Return try signaling that the file exists.
        }
        return false; // Return try signaling that the file doesn't exist.
    }
}
