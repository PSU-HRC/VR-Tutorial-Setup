using UnityEngine;
// General XR (Extended Reality) support library. This one provides the XRHandSubsytem class
using UnityEngine.XR.Management;
// The library that provides the finger, hand, wrist, joint objects and classes.
using UnityEngine.XR.Hands;

public class XRHandsManager : MonoBehaviour
{
    // This is object that manages the hand tracking data
    XRHandSubsystem handSubsystem;

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
                Debug.Log("Left hand is being tracked");
                ProcessHandData(leftHand);
            }

            // Right hand is being tracked
            if (rightHand.isTracked)
            {
                Debug.Log("Right hand is being tracked");
                ProcessHandData(rightHand);
            }
        }
    }

    // Function to process hand data (joint positions and rotations)
    // https://docs.unity3d.com/Packages/com.unity.xr.hands@1.3/api/UnityEngine.XR.Hands.XRHandJointID.html
    private void ProcessHandData(XRHand hand)
    {
        // Loop through all joint IDs by casting an enum value to indices(int), so that we can iterate
        // https://docs.unity3d.com/Packages/com.unity.xr.hands@1.3/api/UnityEngine.XR.Hands.XRHandJointID.html
        for (int i = XRHandJointID.BeginMarker.ToIndex(); i < XRHandJointID.EndMarker.ToIndex(); i++)
        {
            // Recasts indice back to Joint ID so we can use the joint data
            XRHandJointID jointID = XRHandJointIDUtility.FromIndex(i);
            // Obtains joint
            XRHandJoint joint = hand.GetJoint(jointID);
            
            // Pose is position & rotation. This is attempting to get the post of the specified joint gameobject.
            if (joint.TryGetPose(out Pose pose))
            {
                Debug.Log($"{hand.handedness} Hand - {jointID}: Position: {pose.position}, Rotation: {pose.rotation}");
            }
            else
            {
                Debug.LogWarning($"No valid pose for {jointID} in {hand.handedness} hand.");
            }
        }
    }
}
