using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleARCore;
using GoogleARCore.Examples.Common;
using UnityEngine.EventSystems;


#if UNITY_EDITOR
// Set up touch input propagation while using Instant Preview in the editor.
using Input = GoogleARCore.InstantPreviewInput;
#endif

/// <summary>
/// Controls the HelloAR example.
/// </summary>
public class ARController : MonoBehaviour
{
    /// <summary>
    /// The first-person camera being used to render the passthrough camera image (i.e. AR
    /// background).
    /// </summary>
    public Camera FirstPersonCamera;

    /// <summary>
    /// A prefab to place when a raycast from a user touch hits a vertical plane.
    /// </summary>
    public GameObject GameObjectVerticalPlanePrefab;

    /// <summary>
    /// A prefab to place when a raycast from a user touch hits a horizontal plane.
    /// </summary>
    public GameObject GameObjectHorizontalPlanePrefab;

    /// <summary>
    /// A prefab to place when a raycast from a user touch hits a feature point.
    /// </summary>
    public GameObject GameObjectPointPrefab;


    public GameObject TrunkPrefab;
    public GameObject ItemBoxPrefab;
    public GameObject VerticePrefab;
    /// <summary>
    /// The rotation in degrees need to apply to prefab when it is placed.
    /// </summary>
    private const float k_PrefabRotation = 180.0f;

    /// <summary>
    /// True if the app is in the process of quitting due to an ARCore connection error,
    /// otherwise false.
    /// </summary>
    private bool m_IsQuitting = false;


    /// <summary>
    /// An abstract car instance including UI pane and related information.
    /// </summary>
    public GameObject m_MyCar;

    /// <summary>
    /// The bottom four corners of an object.
    /// </summary>
    private List<Vector3> m_ObjectCorners;

    /// <summary>
    /// The up-vector of a plane where objects are placed
    /// </summary>
    private Vector3 m_PlaneUp;
    /// <summary>
    /// The Unity Awake() method.
    /// </summary>
    public void Awake()
    {
        // Enable ARCore to target 60fps camera capture frame rate on supported devices.
        // Note, Application.targetFrameRate is ignored when QualitySettings.vSyncCount != 0.
        Application.targetFrameRate = 60;
        Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.None);
        m_ObjectCorners = new List<Vector3>();
    }

    /// <summary>
    /// The Unity Update() method.
    /// </summary>
    public void Update()
    {
        _UpdateApplicationLifecycle();

        // If the player has not touched the screen, we are done with this update.

        AppState state = ARApplication.ActiveInstance.GetState();
        if (state == AppState.S1_FindSpace)
        {
            //Check if the user found a planar space to place a trunk. If so, proceed to next state to place the trunk
            bool SpaceExist = CheckSpace();
            if (SpaceExist) ARApplication.ActiveInstance.SetStatus(AppState.S2_PlaceTruck);
        } else if(state==AppState.S2_PlaceTruck)
        {
            // place a trunk on a plane
            PlaceTrunk();
        } else if (state == AppState.S4_MeasureObject)
        {
            // semi-automatic estimation of an object dimension. user identifies the bottom four corners of an object. 
            MeasureObject();
            
        }
    }

    /// <summary>
    /// Esitmate the size of an object.
    /// </summary>
    private void MeasureObject()
    {
        
        Touch touch;
        if (Input.touchCount < 1 || (touch = Input.GetTouch(0)).phase != TouchPhase.Began)
        {
            return;
        }
        // Should not handle input if the player is pointing on UI.
        if (EventSystem.current.IsPointerOverGameObject(touch.fingerId))
        {
            return;
        }

        TrackableHit hit;
        TrackableHitFlags raycastFilter = TrackableHitFlags.PlaneWithinPolygon;

        if (Frame.Raycast(touch.position.x, touch.position.y, raycastFilter, out hit))
        {
            // Use hit pose and camera pose to check if hittest is from the
            // back of the plane, if it is, no need to create the anchor.
            if ((hit.Trackable is DetectedPlane) &&
                Vector3.Dot(FirstPersonCamera.transform.position - hit.Pose.position,
                    hit.Pose.rotation * Vector3.up) < 0)
            {
                Debug.Log("Hit at back of the current DetectedPlane");
            }
            else
            {
                GameObject prefab;
                if (hit.Trackable is DetectedPlane)
                {
                    DetectedPlane detectedPlane = hit.Trackable as DetectedPlane;
                    m_PlaneUp = detectedPlane.CenterPose.up;
                    if (detectedPlane.PlaneType == DetectedPlaneType.HorizontalUpwardFacing)
                    {
                        prefab = VerticePrefab;
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    return;
                }


                m_ObjectCorners.Add(hit.Pose.position);
                while (m_ObjectCorners.Count > 4)
                    m_ObjectCorners.RemoveAt(0);                
                
                // Instantiate prefab at the hit pose.
                var vertex = Instantiate(prefab, hit.Pose.position, hit.Pose.rotation);

                // Compensate for the hitPose rotation facing away from the raycast (i.e.
                // camera).
                vertex.transform.Rotate(0, k_PrefabRotation, 0, Space.Self);

                // Create an anchor to allow ARCore to track the hitpoint as understanding of
                // the physical world evolves.
                var anchor = hit.Trackable.CreateAnchor(hit.Pose);
                // Make game object a child of the anchor.
                vertex.transform.parent = anchor.transform;

                if(m_ObjectCorners.Count==4)
                {
                    Vector3[] object_points = new Vector3[8];
                    Vector3 center = Vector3.zero;
                    for (int i = 0; i < 4; i++)
                    {
                        object_points[i * 2 + 0] = m_ObjectCorners[i];
                        

                        // The system assumes the height of all objects as 0.3m. 
                        // TODO: estimation of heights or user control to modify the height
                        object_points[i * 2 + 1] = m_ObjectCorners[i] + m_PlaneUp.normalized * 0.3f; //Fixed Height   

                        center += object_points[i * 2 + 0];
                        center += object_points[i * 2 + 1];
                    }

                    //the center of the 3D bounding box of an object.
                    center /= 8f;                    

                    //estimate the 3D bounding box
                    Bounds bounds = GeometryUtility.CalculateBounds(object_points, transform.localToWorldMatrix);
                    //Debug.Log("[BMWAR] Bounds: " + bounds.center + "    " + bounds.size);

                    // Instantiate prefab at the hit pose.
                    var ItemBox = Instantiate(this.ItemBoxPrefab, center, hit.Pose.rotation);


                    // set the size of the object bounding box.
                    ItemBox.transform.localScale = bounds.size;
                    

                    // Create an anchor to allow ARCore to track the hitpoint as understanding of
                    // the physical world evolves.
                    var ItemAnchor = hit.Trackable.CreateAnchor(hit.Pose);
                    // Make game object a child of the anchor.
                    ItemBox.transform.parent = ItemAnchor.transform;

                    // Add the object meta to the item inventory
                    m_MyCar.GetComponent<MyCar>().AddItem(ItemBox, bounds.size);
                    ARApplication.ActiveInstance.SetStatus(AppState.S0_Idle);
                    m_ObjectCorners.Clear();
                }

            }
        }

    }


    /// <summary>
    /// Check if the user found a planar space to put a virtual trunk.
    /// </summary>
    private bool CheckSpace()
    {
        if (Session.Status != SessionStatus.Tracking)
        {
            return false;
        }
        List<DetectedPlane> m_DetectedPlanes = new List<DetectedPlane>();
        Session.GetTrackables<DetectedPlane>(m_DetectedPlanes, TrackableQueryFilter.All);
        foreach (DetectedPlane plane in m_DetectedPlanes)
        {
            if (plane.TrackingState == TrackingState.Tracking)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Place a virtual 3D trunk into the environment.
    /// </summary>
    private void PlaceTrunk()
    {
        Touch touch;
        if (Input.touchCount < 1 || (touch = Input.GetTouch(0)).phase != TouchPhase.Began)
        {
            return;
        }
        // Should not handle input if the player is pointing on UI.
        if (EventSystem.current.IsPointerOverGameObject(touch.fingerId))
        {
            return;
        }

        TrackableHit hit;
        TrackableHitFlags raycastFilter = TrackableHitFlags.PlaneWithinPolygon;

        if (Frame.Raycast(touch.position.x, touch.position.y, raycastFilter, out hit))
        {
            // Use hit pose and camera pose to check if hittest is from the
            // back of the plane, if it is, no need to create the anchor.
            if ((hit.Trackable is DetectedPlane) &&
                Vector3.Dot(FirstPersonCamera.transform.position - hit.Pose.position,
                    hit.Pose.rotation * Vector3.up) < 0)
            {
                Debug.Log("Hit at back of the current DetectedPlane");
            }
            else
            {
                // Choose the prefab based on the Trackable that got hit.
                GameObject prefab;
                if (hit.Trackable is DetectedPlane)
                {
                    DetectedPlane detectedPlane = hit.Trackable as DetectedPlane;
                    if (detectedPlane.PlaneType == DetectedPlaneType.HorizontalUpwardFacing)
                    {
                        
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    return;
                }

                //discard the exisiting trunk object and create a new.
                m_MyCar.GetComponent<MyCar>().DestroyTrunkObject();

                Debug.Log("[BMWAR] A trunk placed");
                // Instantiate prefab at the hit pose.-
                var trunk = Instantiate(TrunkPrefab, hit.Pose.position, hit.Pose.rotation);

                //update the size of the trunk

                Vector3 trunkSize = m_MyCar.GetComponent<MyCar>().GetTrunkSize();
                trunk.transform.localScale = trunkSize;
                
                // Compensate for the hitPose rotation facing away from the raycast (i.e.
                // camera).
                trunk.transform.Rotate(0, k_PrefabRotation, 0, Space.Self);

                // Create an anchor to allow ARCore to track the hitpoint as understanding of
                // the physical world evolves.
                var anchor = hit.Trackable.CreateAnchor(hit.Pose);

                // Make game object a child of the anchor.
                trunk.transform.parent = anchor.transform;

                // adjust the center position, taking into account the pivot of 3D mesh.                
                trunk.transform.localPosition = new Vector3(0, trunkSize.y / 2f,0);

                // Update the trunk size
                trunk.GetComponent<Trunk3D>().SetDimension(trunkSize);

                // Update the trunk info in the main car instance
                m_MyCar.GetComponent<MyCar>().SetTrunkGameObject(trunk);
                ARApplication.ActiveInstance.SetStatus(AppState.S0_Idle);
            }
        }
    }

    /// <summary>
    /// Check and update the application lifecycle.
    /// </summary>
    private void _UpdateApplicationLifecycle()
    {
        // Exit the app when the 'back' button is pressed.
        if (Input.GetKey(KeyCode.Escape))
        {
            Application.Quit();
        }

        // Only allow the screen to sleep when not tracking.
        if (Session.Status != SessionStatus.Tracking)
        {
            Screen.sleepTimeout = SleepTimeout.SystemSetting;
        }
        else
        {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
        }

        if (m_IsQuitting)
        {
            return;
        }

        // Quit if ARCore was unable to connect and give Unity some time for the toast to
        // appear.
        if (Session.Status == SessionStatus.ErrorPermissionNotGranted)
        {
            _ShowAndroidToastMessage("Camera permission is needed to run this application.");
            m_IsQuitting = true;
            Invoke("_DoQuit", 0.5f);
        }
        else if (Session.Status.IsError())
        {
            _ShowAndroidToastMessage(
                "ARCore encountered a problem connecting.  Please start the app again.");
            m_IsQuitting = true;
            Invoke("_DoQuit", 0.5f);
        }
    }

    /// <summary>
    /// Actually quit the application.
    /// </summary>
    private void _DoQuit()
    {
        Application.Quit();
    }

    /// <summary>
    /// Show an Android toast message.
    /// </summary>
    /// <param name="message">Message string to show in the toast.</param>
    private void _ShowAndroidToastMessage(string message)
    {
        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject unityActivity =
            unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

        if (unityActivity != null)
        {
            AndroidJavaClass toastClass = new AndroidJavaClass("android.widget.Toast");
            unityActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
            {
                AndroidJavaObject toastObject =
                    toastClass.CallStatic<AndroidJavaObject>(
                        "makeText", unityActivity, message, 0);
                toastObject.Call("show");
            }));
        }
    }

}
