using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Aligner : MonoBehaviour
{
    [SerializeField] private bool gizmos;

    public Vector3 TargetPos { get; private set; }
    public Vector3 AverageNormal { get; private set;}

    private Vector3 previousPos = Vector3.negativeInfinity; 
    private Vector3 previousNormal;

    private List<SpiderLegStepper> allLegs;

    public void Init(List<SpiderLegStepper> allLegs)
    {
        this.allLegs = new List<SpiderLegStepper>(allLegs);
    }

    public void CalculateHeightAndNormal()
    {
        bool lerp = true;
        if (previousPos == Vector3.negativeInfinity) lerp = false;

        previousNormal = AverageNormal;
        previousPos = TargetPos;

        AverageNormal = new Vector3();
        TargetPos = new Vector3();

        for (int i = 0; i < allLegs.Count; i++)
        {
            AverageNormal += allLegs[i].TargetNormal;
            TargetPos += allLegs[i].TargetPos;
        }

        TargetPos /= allLegs.Count;

        AverageNormal.Normalize();

        if (!lerp) return;

        TargetPos = Vector3.Lerp(previousPos, TargetPos, 0.5f);
        AverageNormal = Vector3.Lerp(previousNormal, AverageNormal, 0.5f);
    }

    private void OnDrawGizmos()
    {
        if (!gizmos) return; ;
        Gizmos.DrawRay(TargetPos, AverageNormal);
    }
}
