using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderLegStepper : MonoBehaviour
{
    [SerializeField] private GroundCheck groundCheck;
    [SerializeField] private Transform footIKTarget;

    [SerializeField] private float shouldStepDistance;
    [SerializeField] private float stepDuration;
    [SerializeField] private bool isLeftLeg;
    [SerializeField] private bool gizmos;

    public bool IsLeftLeg => isLeftLeg;
    public bool Moving { get; private set; }
    public bool ShouldOverstep => groundCheck.ShouldOverstep;

    public Vector3 TargetNormal => groundCheck.Normal;
    public Vector3 TargetPos => endPoint;

    private Vector3 endPoint;

    private void Awake()
    {
        endPoint = transform.position;
    }

    public void TryMove()
    {
        if (Vector3.Distance(footIKTarget.position, groundCheck.TargetPos) > shouldStepDistance)
        {
            if (Moving) return;
            StartCoroutine(StartMove());
        }
    }

    private IEnumerator StartMove()
    {
        Moving = true;
        if (groundCheck.NoTarget)
        {
            Moving = false;
            yield break;
        }
        Vector3 startPoint = footIKTarget.position;
        endPoint = groundCheck.TargetPos;

        Vector3 overStepDir = endPoint - startPoint;

        if (groundCheck.ShouldOverstep) endPoint += (overStepDir * 0.75f);

        Vector3 midPoint = (startPoint + endPoint) / 2;

        midPoint += groundCheck.transform.up;

        Quaternion startRot = footIKTarget.rotation;
        Quaternion endRot = Quaternion.FromToRotation(Vector3.up, groundCheck.Normal);// groundCheck.TargetRot;

        float timeElapsed = 0;
        do
        {
            timeElapsed += Time.deltaTime;
            float normalizedTime = timeElapsed / stepDuration;
            normalizedTime = Easing.Cubic.InOut(normalizedTime);
            // Quadratic bezier curve
            transform.position =
              Vector3.Lerp(
                 Vector3.Lerp(startPoint, midPoint, normalizedTime),
                Vector3.Lerp(midPoint, endPoint, normalizedTime),
                normalizedTime
              );

            transform.rotation = Quaternion.Slerp(startRot, endRot, normalizedTime);
            yield return null;
        }
        while (timeElapsed < stepDuration);

        Moving = false;
    }

    private void OnDrawGizmos()
    {
        if (!gizmos) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawCube(endPoint, new Vector3(0.1f, 0.025f, 0.1f));
    }
}
