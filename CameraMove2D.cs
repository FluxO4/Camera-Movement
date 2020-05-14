using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraMove2D : MonoBehaviour
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
    Vector3 newPos = Vector3.zero;
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
                viewSize = viewSize + MainCam.orthographicSize * deltaMagnitudeDiff / (Screen.height * 1.0f);
                if (viewSize < zoomInLimit) { viewSize = zoomInLimit; }
                MainCam.orthographicSize = viewSize;
            }
            else
            {
                viewSize = viewSize + MainCam.orthographicSize * deltaMagnitudeDiff / (Screen.height * 1.0f);
                if (viewSize > zoomOutLimit) { viewSize = zoomOutLimit; }
                MainCam.orthographicSize = viewSize;
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
            deltaPos = Input.mousePosition - lastPos;

            if (starteddragging)
            {
                float multiplier = -(viewSize * 2f * 1.0f) / (Screen.height * 1.0f);
                theta = transform.eulerAngles.y * Mathf.PI / 180;

                {
                    transform.position = transform.position + new Vector3(deltaPos.y * Mathf.Sin(theta) * multiplier, 0, deltaPos.y * Mathf.Cos(theta) * multiplier);
                    transform.position = transform.position + new Vector3(deltaPos.x * Mathf.Sin(theta + pi / 2) * multiplier, 0, deltaPos.x * Mathf.Cos(theta + pi / 2) * multiplier);
                }
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
