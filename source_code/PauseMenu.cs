using UnityEngine;
using System.Collections;

public class PauseMenu : MonoBehaviour {

    public GameObject PauseMenuGO = null;

    bool show = false;

    private CameraMovement _CameraMovementA = null;
    private CameraMovement _CameraMovementB = null;
    private Raycast _Raycast = null;
    private GameObject _InventoryUI = null;

	void Awake () {

        _CameraMovementA = Camera.main.transform.parent.GetComponent<CameraMovement>();
        _CameraMovementB = Camera.main.GetComponent<CameraMovement>();
        _Raycast = Camera.main.GetComponent<Raycast>();

        _InventoryUI = GameObject.FindGameObjectWithTag("InventoryHolder");
	}
	
	// Update is called once per frame
	void Update () {
	
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            show = !show;
            PauseMenuGO.SetActive(show);
       

            //bool prevState = _CameraMovementA.enabled;
            if(show)
            {
                _CameraMovementA.enabled = false;
                _CameraMovementB.enabled = false;
                Cursor.visible = true;
            }

            if (!show && !_InventoryUI.activeSelf)
            {
                _CameraMovementA.enabled = true;//prevState
                _CameraMovementB.enabled = true;
                _Raycast.enabled = true;
                Cursor.visible = false;
            }
        
        }
	}
}
