using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct HandData
{
    public string handedness;   // Left or Right hand
    public Vector3[] positions; // Array of joint positions
    public Quaternion[] rotations; // Array of joint rotations

    /*public HandData(string handedness, int jointCount)
    {
        this.handedness = handedness;
        positions = new Vector3[jointCount];
        rotations = new Quaternion[jointCount];
    }*/
}

