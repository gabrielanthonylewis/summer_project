using UnityEngine;
using System.Collections;

public class CameraMovement : MonoBehaviour
{

    [SerializeField]
    bool LockX = false;
    [SerializeField]
    bool LockY = false;
    [SerializeField]
    private float _VerticalSpeed = 2.5f;
    [SerializeField]
    private float _HorizontalSpeed = 2.5f;

    private float _VertLimitLow = 0.5f;
    private float _VertLimitHigh = -0.4f;

    private Transform _Transform = null;

    private bool _StopOper = false;
    public bool StopOperation
    {
        get { return _StopOper; }
        set { _StopOper = value; }
    }

    void Awake()
    {
        _Transform = this.transform;
    }

    void Update()
    {
        if (_StopOper) return;

        if (!LockX)
            UpdateHorizontalRot();
        if (!LockY)
            UpdateVerticalRot();
    }   

    private void UpdateHorizontalRot()
    {
        float mouseX = Input.GetAxis("Mouse X") * _HorizontalSpeed;

        Quaternion horizontalRot = _Transform.rotation * Quaternion.Euler(new Vector3(0f, mouseX, 0f));
        horizontalRot.x = 0f;
        horizontalRot.z = 0f;

        _Transform.rotation = horizontalRot;
    }

    private void UpdateVerticalRot()
    {
        float mouseY = Input.GetAxis("Mouse Y") * _VerticalSpeed;

        Quaternion verticalRot = Quaternion.Euler(new Vector3(-mouseY, 0f, 0f));
        verticalRot.x = Mathf.Clamp(verticalRot.x, Quaternion.Euler(-65f, 0, 0).x, Quaternion.Euler(65f, 0, 0).x);
        verticalRot.y = 0f;
        verticalRot.z = 0f;

        if (_Transform.localRotation.x + verticalRot.x > _VertLimitLow || _Transform.localRotation.x + verticalRot.x < _VertLimitHigh)
            return;

        _Transform.rotation *= verticalRot;
    }
}
