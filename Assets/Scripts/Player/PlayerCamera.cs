using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour {

    public Vector3 debugpos = new Vector3();
    void OnDrawGizmos()
    {
        if (debugpos != new Vector3())
            Gizmos.DrawSphere(debugpos, 1f);
    }

    public new Rigidbody rigidbody;
    public Transform target;

    public float targetHeight = 1.7f;
    public float distance = 5.0f;

    public float maxDistance = 20;
    public float minDistance = .6f;

    public float xSpeed = 250.0f;
    public float ySpeed = 120.0f;

    public int yMinLimit = -80;
    public int yMaxLimit = 80;

    public int zoomRate = 40;

    public float rotationDampening = 3.0f;
    public float zoomDampening = 5.0f;
    public float followSpeed = 2f;
    public float collisionRadius = 5f;

    private float x = 0.0f;
    private float y = 0.0f;
    private float currentDistance;
    private float desiredDistance;
    private float correctedDistance;

    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        rigidbody = GetComponent<Rigidbody>();
        Vector3 angles = transform.eulerAngles;
        x = angles.x;
        y = angles.y;

        currentDistance = distance;
        desiredDistance = distance;
        correctedDistance = distance;

        // Make the rigid body not change rotation
        if (rigidbody)
            rigidbody.freezeRotation = true;

        movementEnabled = true;
    }

    private bool movementEnabled;
    public void DisableMovement()
    {
        movementEnabled = false;

    }
    public void EnableMovement()
    {
        movementEnabled = true;
    }

    /**
     * Camera logic on LateUpdate to only update after all character movement logic has been handled.
     */
    void LateUpdate()
    {
        // Don't do anything if target is not defined
        if (!target)
        {
            if(GameObject.FindGameObjectWithTag("Player"))
                target = GameObject.FindGameObjectWithTag("Player").transform;
            return;
        }

        if (!movementEnabled)
        {
            return;
        }
        
        //Rotation
        x += Input.GetAxis("Mouse X") * xSpeed * 0.015f;
        y -= Input.GetAxis("Mouse Y") * ySpeed * 0.015f;

        y = ClampAngle(y, yMinLimit, yMaxLimit);

        // set camera rotation
        Quaternion rotation = Quaternion.Euler(y, x, 0);

        // calculate the desired distance
        desiredDistance -= Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * zoomRate * Mathf.Abs(desiredDistance);
        //desiredDistance -= Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * zoomRate * Mathf.Abs(desiredDistance);
        desiredDistance = Mathf.Clamp(desiredDistance, minDistance, maxDistance);
        correctedDistance = desiredDistance;

        // calculate desired camera position
        Vector3 position = target.position - (rotation * Vector3.forward * desiredDistance + new Vector3(0, -targetHeight, 0));

        // check for collision using the true target's desired registration point as set by user using height
        RaycastHit collisionHit;
        Vector3 trueTargetPosition = new Vector3(target.position.x, target.position.y + targetHeight, target.position.z);

        // if there was a collision, correct the camera position and calculate the corrected distance
        bool isCorrected = false;
        int layerMask = ~((1 << 9) | (1 << 10));
        Debug.DrawLine(trueTargetPosition, position, Color.red, 2f);
        if (Physics.SphereCast(trueTargetPosition, collisionRadius, -transform.forward, out collisionHit, Vector3.Distance(target.position, transform.position), layerMask))
        {
            debugpos = collisionHit.point;

            position = collisionHit.point;
            correctedDistance = Vector3.Distance(trueTargetPosition, position);
            isCorrected = true;
        }

        // For smoothing, lerp distance only if either distance wasn't corrected, or correctedDistance is more than currentDistance
        currentDistance = !isCorrected || correctedDistance > currentDistance ? Mathf.Lerp(currentDistance, correctedDistance, Time.deltaTime * zoomDampening) : Mathf.Lerp(currentDistance, correctedDistance, Time.deltaTime * zoomDampening * 2f);

        // recalculate position based on the new currentDistance
        position = target.position - (rotation * Vector3.forward * currentDistance + new Vector3(0, -targetHeight, 0));

        transform.rotation = rotation;
        transform.position = position;
    }

    private static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360)
            angle += 360;
        if (angle > 360)
            angle -= 360;
        return Mathf.Clamp(angle, min, max);
    }
}
