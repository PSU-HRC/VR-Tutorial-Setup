using UnityEngine;
// General XR (Extended Reality) support library. This one provides the XRHandSubsytem class
using UnityEngine.XR.Management;
// The library that provides the finger, hand, wrist, joint objects and classes.
using UnityEngine.XR.Hands;
using System.Collections.Generic;


public class XRHandsManager : MonoBehaviour
{
    // This is object that manages the hand tracking data
    XRHandSubsystem handSubsystem;
    [SerializeField]
    private SendData sendDataScript;
    void Start()
    {
        // Get the active XR loader and hand subsystem. 
        var xrManagerSettings = XRGeneralSettings.Instance.Manager;
        var xrLoader = xrManagerSettings.activeLoader;

        if (xrLoader != null)
        {
            // Initializes our handSubsytem by retrieving it from xrLoader
            handSubsystem = xrLoader.GetLoadedSubsystem<XRHandSubsystem>();

            // Subsystem was not found
            if (handSubsystem == null)
            {
                Debug.LogError("XRHandSubsystem not found");
            }
            // Success
            else
            {
                Debug.Log("XRHandSubsystem initialized");
            }
        }
        // Broader issue, no xrLoader, maybe no xrManagerSettings
        else
        {
            Debug.LogError("No XR Loader active");
        }
    }

    void Update()
    {
        if (handSubsystem != null)
        {
            // Gets data for left and right hands
            XRHand leftHand = handSubsystem.leftHand;
            XRHand rightHand = handSubsystem.rightHand;

            // Left hand is being tracked
            if (leftHand.isTracked)
            {
                //Debug.Log("Left hand is being tracked");
                HandData leftData = ProcessHandData(leftHand);
                sendDataScript.SendToArduino(leftData);
            }

            // Right hand is being tracked
            if (rightHand.isTracked)
            {
                //Debug.Log("Right hand is being tracked");
                HandData rightData = ProcessHandData(rightHand);
                sendDataScript.SendToArduino(rightData);
            }
        }
    }

    // Function to process hand data (joint positions and rotations)
    // https://docs.unity3d.com/Packages/com.unity.xr.hands@1.3/api/UnityEngine.XR.Hands.XRHandJointID.html
    private HandData ProcessHandData(XRHand hand)
    {
        HandData handData = new HandData
        {
            handedness = hand.handedness == Handedness.Left ? "Left" : "Right",
            positions = new Vector3[5],
            rotations = new Quaternion[5]
        };

        int index = 0;
        // Loop through all joint IDs by casting an enum value to indices(int), so that we can iterate
        // https://docs.unity3d.com/Packages/com.unity.xr.hands@1.3/api/UnityEngine.XR.Hands.XRHandJointID.html
        for (int i = XRHandJointID.BeginMarker.ToIndex(); i < XRHandJointID.EndMarker.ToIndex(); i++)
        {
            // Recasts indice back to Joint ID so we can use the joint data
            XRHandJointID jointID = XRHandJointIDUtility.FromIndex(i);
            // Only gets data for proximal joints of each finger
            if (isProximal(jointID)) {
                // Obtains joint
                XRHandJoint joint = hand.GetJoint(jointID);
                
                // Pose is position & rotation. This is attempting to get the post of the specified joint gameobject.
                if (joint.TryGetPose(out Pose pose))
                {
                    //Debug.Log($"{hand.handedness} Hand - {jointID}: Position: {pose.position}, Rotation: {pose.rotation}");
                    handData.positions[index] = pose.position;
                    handData.rotations[index] = pose.rotation;
                    index ++;
                }
                else
                {
                    Debug.LogWarning($"No valid pose for {jointID} in {hand.handedness} hand.");
                }
            }
            
        }
        return handData;
    }

    private bool isProximal(XRHandJointID jointID) {

        HashSet<XRHandJointID> proximalIndices = new HashSet<XRHandJointID> {
            XRHandJointID.ThumbDistal,
            XRHandJointID.IndexDistal,
            XRHandJointID.MiddleDistal,
            XRHandJointID.RingDistal,
            XRHandJointID.LittleDistal
        };

        return proximalIndices.Contains(jointID);

    }
}
