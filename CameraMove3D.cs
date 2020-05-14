using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove3D : MonoBehaviour
{
    [Range(0.05f,1f)]
    public float playerRotationDelay = 0.3f; //Set to 1 for instant player rotation

    [Range(10, 90)]
    public int LookDownAngleLimit = 80;

    [Range(10, 90)]
    public int LookUpAngleLimit = 80;

    public GameObject playerRoot;
    public GameObject playerModel;
    public GameObject head;

    void Start()
    {
        LockAndCentreCursor(true);
    }

 

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown("escape"))
        {
            LockAndCentreCursor(false);
        }


        float h = 10f * Input.GetAxis("Mouse X");
        float v = -5 * Input.GetAxis("Mouse Y");

        playerRoot.transform.Rotate(playerRoot.transform.up, h * Time.deltaTime * 20, Space.Self);
        playerModel.transform.Rotate(playerRoot.transform.up, -h * Time.deltaTime * 20, Space.Self);

        playerModel.transform.Rotate(playerRoot.transform.up, -Vector3.SignedAngle(playerRoot.transform.forward, playerModel.transform.forward, playerRoot.transform.up)*playerRotationDelay, Space.Self);

        if (head.transform.localEulerAngles.x + v * Time.deltaTime * 20 > LookDownAngleLimit && head.transform.localEulerAngles.x + v * Time.deltaTime * 20 <= 180)
        {
            head.transform.localEulerAngles = new Vector3(LookDownAngleLimit, head.transform.localEulerAngles.y, head.transform.localEulerAngles.z);
        }
        else
        if (head.transform.localEulerAngles.x + v * Time.deltaTime * 20 < (360- LookUpAngleLimit) && head.transform.localEulerAngles.x + v * Time.deltaTime * 20 > 180)
        {
            head.transform.localEulerAngles = new Vector3((360 - LookUpAngleLimit), head.transform.localEulerAngles.y, head.transform.localEulerAngles.z);
        }
        else
        {
            head.transform.localEulerAngles = new Vector3(head.transform.localEulerAngles.x + v * Time.deltaTime * 20, head.transform.localEulerAngles.y, head.transform.localEulerAngles.z);
        }

        if (Input.GetMouseButtonDown(0))
        {
            LockAndCentreCursor(true);
        }
    }

    void LockAndCentreCursor(bool yes)
    {
        if (yes)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
