using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    [SerializeField] LayerMask groundLayer;
    [SerializeField] private bool gizmos;

    public Vector3 TargetPos { get; private set; }
    public Quaternion TargetRot { get; private set; }

    public Vector3 Normal { get; private set; }
    public bool ShouldOverstep { get; private set; }

    public bool NoTarget { get; private set; }

    private Vector3 idealPos;
    private Quaternion idealRot;

    private void Start()
    {
        idealPos = transform.position - transform.up;
        idealRot = transform.rotation;
    }
    private void Update()
    {
        CheckGround();
    }

    private void CheckGround()
    {
        ShouldOverstep = false;
         RaycastHit hit;

        var forward = transform.forward.normalized;
        var back = -forward;
        var right = transform.right.normalized;
        var left = -right;
        var up = transform.up.normalized;
        var down = -up;

        Debug.DrawRay(transform.position, forward, Color.cyan);
        Debug.DrawRay(transform.position, back, Color.cyan);
        Debug.DrawRay(transform.position, right, Color.blue);
        Debug.DrawRay(transform.position, left, Color.blue);
        Debug.DrawRay(transform.position, up, Color.green);
        Debug.DrawRay(transform.position, down, Color.green);
        //forward above
        if (Physics.SphereCast(transform.position + up, 0.1f, up, out hit, 0.5f, groundLayer))
        {
            TargetPos = hit.point;
            TargetRot = Quaternion.FromToRotation(transform.up, hit.normal);
            Normal = hit.normal;
        }
        // Wall in front
        else if (Physics.SphereCast(transform.position, 0.1f, forward, out hit, 0.5f, groundLayer))
        {
            TargetPos = hit.point;
            TargetRot = Quaternion.FromToRotation(transform.up, hit.normal);
            Normal = hit.normal * 5;
        }
        // Wall behind
        else if (Physics.SphereCast(transform.position, 0.1f, forward, out hit, 0.5f, groundLayer))
        {
            TargetPos = hit.point;
            TargetRot = Quaternion.FromToRotation(transform.up, hit.normal);
            Normal = hit.normal;
        }
        //Floor below
        else if (Physics.SphereCast(transform.position, 0.1f, down, out hit, 2f, groundLayer))
        {
            TargetPos = hit.point;
            TargetRot = Quaternion.FromToRotation(transform.up, hit.normal);
            Normal = hit.normal;
            ShouldOverstep = true;
        }
        //Floor infront 
        else if (Physics.SphereCast(transform.position + forward, 0.1f, down, out hit, 2f, groundLayer))
        {
            TargetPos = hit.point;
            TargetRot = Quaternion.FromToRotation(transform.up, hit.normal);
            Normal = hit.normal;
        }
        //Floor behind
        else if (Physics.SphereCast(transform.position + back, 0.1f, down, out hit, 2f, groundLayer))
        {
            TargetPos = hit.point;
            TargetRot = Quaternion.FromToRotation(transform.up, hit.normal);
            Normal = hit.normal;
        }
        //Round corner front
        else if (Physics.SphereCast(transform.position + forward / 2 + down, 0.1f, down, out hit, 2f, groundLayer))
        {
            TargetPos = hit.point;
            TargetRot = Quaternion.FromToRotation(transform.up, hit.normal);
            Normal = hit.normal;
        }
        //Round corner back
        else if (Physics.SphereCast(transform.position + back / 2 + down, 0.1f, down, out hit, 2f, groundLayer))
        {
            TargetPos = hit.point;
            TargetRot = Quaternion.FromToRotation(transform.up, hit.normal);
            Normal = hit.normal;
        }
        // Right
        else if (Physics.SphereCast(transform.position + right, 0.1f, right, out hit, 1f, groundLayer))
        {
            TargetPos = hit.point;
            TargetRot = Quaternion.FromToRotation(transform.up, hit.normal);
            Normal = hit.normal;
        }
        //Left
        else if (Physics.SphereCast(transform.position + left, 0.1f, left, out hit, 1f, groundLayer))
        {
            TargetPos = hit.point;
            TargetRot = Quaternion.FromToRotation(transform.up, hit.normal);
            Normal = hit.normal;
        }

        else {
            NoTarget = true;
            return;
        }
        NoTarget = false;
    }

    private void OnDrawGizmos()
    {
        if (!gizmos) return;
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(TargetPos, 0.1f);
        Gizmos.DrawRay(TargetPos, Normal);
        Gizmos.DrawSphere(transform.position, 0.1f);
    }
}
