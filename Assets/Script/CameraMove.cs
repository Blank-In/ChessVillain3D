using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    public Transform CameraVector;
    public float rotateSpeed = 500;
    public float turnSpeed = 5;

    private bool turnFlg = false, clickFlg = false, uiFlag=true;
    private float cnt = 0;
    private Vector3 nowRotate = new Vector3(0, 0, 0);

    private void Start()
    {
        nowRotate = CameraVector.rotation.eulerAngles;
    }

    private void FixedUpdate()
    {
        CameraTurn(false);
        CameraRotation();
    }

    public void CameraTurn(bool a)
    {
        if (a)
        {
            turnFlg = true;
        }
        if (turnFlg)
        {
            cnt += turnSpeed;
            nowRotate.y += turnSpeed;
            CameraVector.transform.rotation = Quaternion.Euler(nowRotate);
        }
        if (cnt == 180)
        {
            cnt = 0;
            turnFlg = false;
        }
    }

    private void CameraRotation()
    {
        if(Input.GetMouseButton(0))
        {
            if(uiFlag && clickFlg)
            {
                nowRotate.x += Input.GetAxis("Mouse Y") * rotateSpeed * -1 * Mathf.Deg2Rad;
                nowRotate.y += Input.GetAxis("Mouse X") * rotateSpeed * Mathf.Deg2Rad;
                CameraVector.transform.rotation = Quaternion.Euler(nowRotate);
            }
            clickFlg = true;
        }
        else
        {
            clickFlg = false;
        }
    }
   
    public void UIclick()
    {
        uiFlag = false;
        Invoke(nameof(UIflagChange), 0.3f);
    }
   
    private void UIflagChange()
    {
        uiFlag = true;
    }
}
