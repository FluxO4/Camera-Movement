using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//CREATED BY FluxO4 -- Simple 3D Camera movement script from the viewpoint of the player or behind the player

public class CameraMove3D : MonoBehaviour
{
    public enum ViewType  {ThirdPerson, FirstPerson};

    public ViewType viewType = ViewType.ThirdPerson; // If first person, camera will be moved to wherever head is,
    //and in this case adding a layermask to hide the playerModel from the camera is recommended


    public bool ReAdjustCamera = false;

    //Number of degrees down from the horizontal the head can rotate
    [Range(10, 90)]
    public int LookDownAngleLimit = 80;

    //Number of degrees up from the horizontal the head can rotate
    [Range(10, 90)]
    public int LookUpAngleLimit = 80;

    //This is the root player object
    public GameObject playerRoot;

    //Needs to be a child of playerRoot, and needs to have the same euler-angles
    public GameObject playerModel;

    //Needs to be a child of playerRoot, and preferably higher up, but with the same rotation
    public GameObject head;

    //What value to multiply the angle-delta between the playerRoot and playerModel at every frame
    [Range(0.05f, 1f)]
    public float playerRotationDelay = 0.3f; //Set to 1 for instant player rotation

    //The maximum difference between root and model forward angle before the model is rotated with delta
    [Range(0, 20)]
    public int maxRootModelAngleError = 1;


    //Ideally put the camera directly behind the player, looking at the back of the head, looking straight ahead
    //If camera is angled at the start, the lookAngleLimit values might not work correctly
    //These problems will be automatically fixed if ReAdjustCamera is on
    

    void Start()
    {
        if(viewType == ViewType.FirstPerson)
        {
            transform.position = head.transform.position;
            transform.eulerAngles -= new Vector3(transform.eulerAngles.x, 0, 0);
        }
        else if(ReAdjustCamera)
        {
            transform.localPosition = new Vector3(transform.localPosition.x, 0, transform.localPosition.y);
            transform.eulerAngles -= new Vector3(transform.eulerAngles.x, 0, 0);
        }

        

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

        float deltaAngle = -Vector3.SignedAngle(playerRoot.transform.forward, playerModel.transform.forward, playerRoot.transform.up) * playerRotationDelay;

        if (deltaAngle > maxRootModelAngleError)
        {
            playerModel.transform.Rotate(playerRoot.transform.up, deltaAngle, Space.Self);
        }

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
