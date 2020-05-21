
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

//STILL UNDER DEVELOPMENT
//Created by FluxO4


//Special feature: You can imagine the world as an infinite plane at y=0 and 'grab' points on the plane with the mouse button and
//Move the camera by apparently moving the points around on the plane
public class CameraMove2_5D : MonoBehaviour
{
    //if true, camera doesn't get triggered when clicking down over a UI raycast target
    public bool ignoreUIElements = false;

    //To check if screen is being touched by two fingers so dragging can be skipped
    private bool isZooming = false;

    //Camera can be rotated around the y axis by rotating the touch points or by pressing 9 or 0
    public bool rotatable = true;

    //size inherited from camera, changed while zooming
    public float viewSize;

    //view size in radians in case of perspective camera
    public float viewSizeR;

    //Camera component to extract viewSize from
    Camera MainCam;

    //pointer's position on screen, for the maths
    Vector3 deltaPos = Vector3.zero;
    Vector3 lastPos = Vector3.zero;

    //Value of pi, more accurate than Mathf.Pi
    float pi = 3.141592653589793f;

    /*//Number of degrees below horizon defining the maximum distance that can be 'grabbed' and pulled with the mouse pointer
    //Effectively, an wall as tall as to meet the apparent horizon 
    //is built at that distance, and points on the wall are pulled for grabbing distances larger than that distance
    float belowHorizonThreshhold = 10;*/

    //Camera's y angle, could change when maths is changed
    float theta;

    //Toggle to check if the mouse is being dragged, made true by clicking down
    bool starteddragging = false;

    //Zoom limit, orthro-size for orthrographic camera; fov for perspective camera
    [SerializeField]
    float zoomOutLimit = 70;
    [SerializeField]
    float zoomInLimit = 5;


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
            viewSizeR = viewSize * Mathf.Deg2Rad;
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
            return;
        }



        //FOR TESTING PURPOSES IN CASE YOUR SCREEN DOESN'T HAVE TOUCH SUPPORT

        if (Input.GetKey("9"))
        {
            transform.eulerAngles += new Vector3(0, 0.25f,0);
        }
        else if (Input.GetKey("0"))
        {
            transform.eulerAngles += new Vector3(0, -0.25f, 0);
        }


        if (Input.GetKey("]"))
        {
            if (MainCam.orthographic)
            {
                viewSize = viewSize - MainCam.orthographicSize * 0.01f;
                if (viewSize < zoomInLimit) { viewSize = zoomInLimit; }
                MainCam.orthographicSize = viewSize;
            }
            else
            {
                viewSize = viewSize - MainCam.fieldOfView * 0.01f;
                if (viewSize < zoomInLimit) { viewSize = zoomInLimit; }
                MainCam.fieldOfView = viewSize;
            }
        }
        else if (Input.GetKey("["))
        {
            if (MainCam.orthographic)
            {
                viewSize = viewSize + MainCam.orthographicSize * 0.01f;
                if (viewSize > zoomOutLimit) { viewSize = zoomOutLimit; }
                MainCam.orthographicSize = viewSize;
            }
            else
            {
                viewSize = viewSize + MainCam.fieldOfView * 0.01f;
                if (viewSize > zoomOutLimit) { viewSize = zoomOutLimit; }
                MainCam.fieldOfView = viewSize;
            }
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
                float m = 0;
                float m2 = 0;
                if (MainCam.orthographic)
                {
                    deltaPos = Input.mousePosition - lastPos;
                    float b = transform.localEulerAngles.x * Mathf.Deg2Rad;
                    float h = transform.position.y;
                    m = -(deltaPos.y * viewSize * 2.0f) / (Screen.height * 1.0f * Mathf.Sin(b));
                    m2 = -(deltaPos.x * viewSize * 2.0f) / (Screen.height * 1.0f);
                }                
                else
                {
                    
                    float a1 = Mathf.Atan(Mathf.Tan(viewSizeR / 2.0f) * (Input.mousePosition.y / (Screen.height * 1.0f) - 0.5f));
                    float a2 = Mathf.Atan(Mathf.Tan(viewSizeR / 2.0f) * (lastPos.y / (Screen.height * 1.0f) - 0.5f));
                    float h = transform.position.y;
                    float b = transform.localEulerAngles.x * Mathf.Deg2Rad;
                    float dist = h / Mathf.Sin(b - a1);

                     m = h * (1.0f / Mathf.Tan((b - a2)) - 1.0f / Mathf.Tan((b - a1)));
                     m2 = -(dist * 2 * Mathf.Tan(viewSizeR / 2.0f) / (Screen.height * 1.0f)) * (Input.mousePosition.x - lastPos.x);
                }
                transform.position += Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized * m;
                transform.position += transform.right * m2;

            }
            lastPos = Input.mousePosition;
        }
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
