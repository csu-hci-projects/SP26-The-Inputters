using UnityEngine;

public class PlayerLocomotion : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float turnSpeed = 90f;

    private Transform _eyeAnchor;

    private void Start()
    {
        var rig = GetComponentInChildren<OVRCameraRig>();
        if (rig != null)
            _eyeAnchor = rig.centerEyeAnchor;

        if (_eyeAnchor == null)
            Debug.LogWarning("[PlayerLocomotion] CenterEyeAnchor not found.");
        else
            Debug.Log("[PlayerLocomotion] Ready.");
    }

    private void Update()
    {
        if (_eyeAnchor == null) return;

        // Left stick — move
        Vector2 moveAxis = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick);
        if (moveAxis.sqrMagnitude > 0.01f)
        {
            Vector3 forward = _eyeAnchor.forward;
            forward.y = 0f;
            forward.Normalize();

            Vector3 right = _eyeAnchor.right;
            right.y = 0f;
            right.Normalize();

            transform.position += (forward * moveAxis.y + right * moveAxis.x) * moveSpeed * Time.deltaTime;
        }

        // Right stick — smooth turn
        Vector2 turnAxis = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick);
        if (Mathf.Abs(turnAxis.x) > 0.1f)
        {
            // Rotate around the eye position so the view doesn't swing off-pivot
            transform.RotateAround(_eyeAnchor.position, Vector3.up, turnAxis.x * turnSpeed * Time.deltaTime);
        }
    }
}
