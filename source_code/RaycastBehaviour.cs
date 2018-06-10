using UnityEngine;
using System.Collections;

public class RaycastBehaviour : MonoBehaviour {

    Color originalCol = Color.red;
    Color newCol = Color.red;

    MeshRenderer _MeshRenderer = null;

	// Use this for initialization
	void Start () {

        _MeshRenderer = this.GetComponent<MeshRenderer>();

        originalCol = _MeshRenderer.material.color;

        Color newColor = Color.white;
        newCol = newColor;
	}
	
    public void Hit(bool isHit)
    {
        if (isHit) _MeshRenderer.material.color = newCol;
        else _MeshRenderer.material.color = originalCol;
    }

}
