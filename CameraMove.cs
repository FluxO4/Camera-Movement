using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class CameraMove : MonoBehaviour
{
    public enum MovementType {_3D, _2D, _2_5D};

    public MovementType currentType = MovementType._2D;

    public enum CameraType {Perspective, Orthrographic};

    public CameraType cameraType;

    public bool ignoreUIElements = false;


    ///To check if screen is being touched by two fingers so dragging can be skipped
    private bool isZooming = false;

    //size inherited from camera, changed while zooming
    public float viewSize;
    float screenFactor;

    //Camera component to extract viewSize from
    Camera MainCam;

    //pointer's position on screen, for the maths, could change when maths is changed
    Vector3 newPos;
    Vector3 deltaPos;
    Vector3 lastPos;
    //Zooming speed, linear, could change when I need to test on touch screens

    //Value of pi, more accurate than Mathf.Pi
    float pi = 3.141592653589793f;

    //Camera's y angle, could change when maths is changed
    float theta;

    //Turns on when dragging is started from screen area outside the side panel, so screen can't be dragged when mousebuttondown is on side panel

    bool starteddragging;

    [SerializeField]
    float zoomOutLimit = 50;
    [SerializeField]
    float zoomInLimit = 2;


    //Start frame is called once before the first frame


    void Start()
    {
        
        Input.simulateMouseWithTouches = true;
        lastPos = Vector3.zero;
        newPos = Vector3.zero;
        deltaPos = Vector3.zero;
        starteddragging = false;
        //Extracting camera component
        MainCam = transform.GetComponent<Camera>();
        if (cameraType == CameraType.Orthrographic)
        {
            MainCam.orthographic = true;
        }
        else
        {
            MainCam.orthographic = false;
        }

        screenFactor = 1280.0f / (Screen.width * 1f);
        //Extracting the viewSize
        // viewSize = MainCam.fieldOfView;
        viewSize = MainCam.orthographicSize;
    }

    private void OnEnable()
    {
        lastPos = Input.mousePosition;
        starteddragging = false;
    }
    void LateUpdate()
    {
        /*if (Game.Toggles.movingFig)
        {
            if (starteddragging == true)
            {
                starteddragging = false;
            }
            goto skipper;
        }*/


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


        if (Input.GetKey(","))
        {
            viewSize = viewSize - MainCam.orthographicSize * 0.01f * screenFactor;
            if (viewSize < zoomInLimit) { viewSize = zoomInLimit; }
            MainCam.orthographicSize = viewSize;
        }
        else if (Input.GetKey("."))
        {
            viewSize = viewSize + MainCam.orthographicSize * 0.01f * screenFactor;
            if (viewSize > zoomOutLimit) { viewSize = zoomOutLimit; }
            MainCam.orthographicSize = viewSize;
        }

        if (Input.GetMouseButtonUp(0))
        {
            starteddragging = false;

        }


        //On mouse down, it is detected if mouse is on side panel, and the screen movement is only allowed when it's not
        if (Input.GetMouseButtonDown(0) && (!IsPointerOverUIObject() || ignoreUIElements))
        {
            // Debug.Log("CAMERA MOVEMENT: MOUSE BUTTON DOWN TRIGGER" + Time.time);
            starteddragging = true;
            // Debug.Log(starteddragging + "" + Time.time);
            //last position
            lastPos = Input.mousePosition;
        }

        //On mouse button, screen moves linearly for now, I could change it
        if (Input.GetMouseButton(0))

        {
            deltaPos = Input.mousePosition - lastPos;

            if (starteddragging)
            { // NEED TO CHGANGE HOW THIS IS DONE
                float multiplier = -(viewSize * 2f * 1.0f) / (Screen.height * 1.0f);
                theta = transform.eulerAngles.y * Mathf.PI / 180;

                {
                    transform.position = transform.position + new Vector3(deltaPos.y * Mathf.Sin(theta) * multiplier, 0, deltaPos.y * Mathf.Cos(theta) * multiplier);
                    transform.position = transform.position + new Vector3(deltaPos.x * Mathf.Sin(theta + Mathf.PI / 2) * multiplier, 0, deltaPos.x * Mathf.Cos(theta + Mathf.PI / 2) * multiplier);
                }

            }


            //Last position
            lastPos = Input.mousePosition;
        }




    //End
    skipper:;
    }

    bool isShaking = false;
    Vector3 shakeDelta;
    float shakeAngle = 0;
    public void cameraShake()
    {
        shakeAngle = 0.1f;
        isShaking = true;
    }

    private bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }


    //Converters for the maths, before I realised they existed on Mathf
    float deg(float radians)
    {
        return radians * 180 / pi;
    }

    float rad(float degrees)
    {
        return degrees * pi / 180;
    }
}
