using UnityEngine;
//using Meta.XR.Util;
using System;
//using Oculus.Interaction.Input;
public class GazeTracking : MonoBehaviour
{

    public Transform leftEye;
    public Transform rightEye;
    //public OVREyeGaze leftEyeGaze;
    //public OVREyeGaze rightEyeGaze;
    void Update()
    {
       /* if (leftEye != null)
            Debug.Log("Left Eye Direction: " + leftEye.forward);

        if (rightEye != null)
            Debug.Log("Right Eye Direction: " + rightEye.forward);*/
    }


    public Vector3 GetGazeDirection()
    {

        if (leftEye != null)
             return leftEye.forward.normalized;
         else if (rightEye != null)
             return rightEye.forward.normalized;

       return Camera.main.transform.forward;
    }
}
