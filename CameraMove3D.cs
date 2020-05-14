using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove3D : MonoBehaviour
{
    [Range(0,3)]
    public float playerRotationDelay = 1.0f; //Set to 0 for instant player rotation

    public GameObject playerRoot;
    public GameObject playerModel;
    public GameObject head;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown("escape"))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }


        float h = 10f * Input.GetAxis("Mouse X");
        float v = -5 * Input.GetAxis("Mouse Y");

        playerRoot.transform.Rotate(playerRoot.transform.up, h * Time.deltaTime * 20, Space.Self);
        

        if (head.transform.localEulerAngles.x + v * Time.deltaTime * 20 > 70 && head.transform.localEulerAngles.x + v * Time.deltaTime * 20 <= 180)
        {
            head.transform.localEulerAngles = new Vector3(70, head.transform.localEulerAngles.y, head.transform.localEulerAngles.z);
        }
        else
        if (head.transform.localEulerAngles.x + v * Time.deltaTime * 20 < 280 && head.transform.localEulerAngles.x + v * Time.deltaTime * 20 > 180)
        {
            head.transform.localEulerAngles = new Vector3(280, head.transform.localEulerAngles.y, head.transform.localEulerAngles.z);
        }
        else
        {
            head.transform.localEulerAngles = new Vector3(head.transform.localEulerAngles.x + v * Time.deltaTime * 20, head.transform.localEulerAngles.y, head.transform.localEulerAngles.z);
        }
 
    }
}
