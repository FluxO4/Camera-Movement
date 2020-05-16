using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.EventSystems;

//STILL UNDER DEVELOPMENT
public class CameraMove2_5D : MonoBehaviour
{
    //if true, camera doesn't get triggered when clicking down over a UI raycast target
    public bool ignoreUIElements = false;

    //To check if screen is being touched by two fingers so dragging can be skipped
    private bool isZooming = false;

    //size inherited from camera, changed while zooming
    public float viewSize;

    //Camera component to extract viewSize from
    Camera MainCam;

    //pointer's position on screen, for the maths
    Vector3 deltaPos = Vector3.zero;
    Vector3 lastPos = Vector3.zero;

    //Value of pi, more accurate than Mathf.Pi
    float pi = 3.141592653589793f;

    //Camera's y angle, could change when maths is changed
    float theta;

    //Toggle to check if the mouse is being dragged, made true by clicking down
    bool starteddragging = false;

    //Zoom limit, orthro-size for orthrographic camera; fov for perspective camera
    [SerializeField]
    float zoomOutLimit = 50;
    [SerializeField]
    float zoomInLimit = 2;


    void Start()
    {

        Input.simulateMouseWithTouches = true;

        //Extracting camera component
        MainCam = transform.GetComponent<Camera>();

        if (MainCam.orthographic)
        {
            viewSize = MainCam.orthographicSize;
        }
        else
        {
            viewSize = MainCam.fieldOfView;
        }
    }

    private void OnEnable()
    {
        lastPos = Input.mousePosition;
        starteddragging = false;
    }
    void LateUpdate()
    {
        /*if (false) // ADD CONDITION HERE TO SKIP CAMERA MOVEMENT; FOR EXAMPLE WHILE SELECTING LAUNCH ANGLE
        {
            goto skipper;
        }*/


        //Pinch To Zoom
        if (Input.touchCount == 2 && !IsPointerOverUIObject())
        {
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);


            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;


            float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;


            float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

            if (deltaMagnitudeDiff < 0)
            {
                if (MainCam.orthographic)
                {
                    viewSize = viewSize + MainCam.orthographicSize * deltaMagnitudeDiff / (Screen.height * 1.0f);
                    if (viewSize < zoomInLimit) { viewSize = zoomInLimit; }
                    MainCam.orthographicSize = viewSize;
                }
                else
                {
                    viewSize = viewSize + MainCam.fieldOfView * deltaMagnitudeDiff / (Screen.height * 1.0f);
                    if (viewSize < zoomInLimit) { viewSize = zoomInLimit; }
                    MainCam.fieldOfView = viewSize;
                }
            }

            lastPos = Input.mousePosition;
            isZooming = true;
        }


        //Screen Movement

        if (Input.GetMouseButtonUp(0))
        {
            starteddragging = false;
            isZooming = false;
        }

        //In case zooming with touch screen is in progress, skip to end of update function
        if (isZooming)
        {
            goto skipper;
        }

        //FOR TESTING PURPOSES IN CASE YOUR SCREEN DOESN'T HAVE TOUCH SUPPORT
        if (Input.GetKey(","))
        {
            viewSize = viewSize - MainCam.orthographicSize * 0.01f;
            if (viewSize < zoomInLimit) { viewSize = zoomInLimit; }
            MainCam.orthographicSize = viewSize;
        }
        else if (Input.GetKey("."))
        {
            viewSize = viewSize + MainCam.orthographicSize * 0.01f;
            if (viewSize > zoomOutLimit) { viewSize = zoomOutLimit; }
            MainCam.orthographicSize = viewSize;
        }
        //END OF TESTING SECTION

        //
        if (Input.GetMouseButtonUp(0))
        {
            starteddragging = false;
        }


        if (Input.GetMouseButtonDown(0) && (!IsPointerOverUIObject() || ignoreUIElements))
        {
            starteddragging = true;
            lastPos = Input.mousePosition;
        }

        
        if (Input.GetMouseButton(0))
        {
            if (starteddragging)
            {
                deltaPos = Input.mousePosition - lastPos;
                float a1 = viewSize * (Input.mousePosition.y / (Screen.height * 1.0f) - 0.5f);
                float h = transform.position.y;
                float b = transform.localEulerAngles.x;
                float h1 = h * 2;
                if (b - a1 > 1f)
                {
                    h1 = (h / Mathf.Sin((b - a1) * Mathf.Deg2Rad));
                    if (h1 > h * 2) h1 = h * 2;
                }
                float m = -2 * pi * h1 * (viewSize * deltaPos.y / (Screen.height * 1.0f)) / 360.0f;


                transform.position += Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized * m;
            }
            lastPos = Input.mousePosition;
        }

    skipper:;
    }

    private bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }
}
