using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderController : MonoBehaviour
{
    [SerializeField] private SpiderLegStepper[] legGroupOne;
    [SerializeField] private SpiderLegStepper[] legGroupTwo;
    [SerializeField] private SpiderLegStepper[] legGroupThree;

    [SerializeField] private Transform model;
    [SerializeField] private Transform body;
    [SerializeField] private Transform bodyModel;
    [SerializeField] float rotationOffset = 50f; //Rotate by 50 units
    [SerializeField] private float heightOffFloor;

    [SerializeField] private Aligner aligner;

    [SerializeField] private float flatGroundSpeed = 20;
    [SerializeField] private float unevenMoveSpeed = 5;
    [SerializeField] private float turnSpeed = 20;
    [SerializeField] private float surfaceAlignmentSpeed = 10;

    private float rotationAmount;

    float leftLegHeightFromGround = 0;
    float rightLegHeightFromGround = 0;

    Vector3 bodyStartAngle;

    float previousLeftLegHeightFromGround = 0;
    float previousRightLegHeightFromGround = 0;

    private List<SpiderLegStepper> allLegs;

    private void Awake()
    {
        allLegs = new List<SpiderLegStepper>();
        allLegs.AddRange(legGroupOne);
        allLegs.AddRange(legGroupTwo);
        allLegs.AddRange(legGroupThree);
    }
    private void Start()
    {
        aligner.Init(allLegs);
        bodyStartAngle = bodyModel.localEulerAngles;
        StartCoroutine(MoveLegs());
    }

    

    private void SetBodyHeight()
    {
        aligner.CalculateHeightAndNormal();

        var speed = GetOnFlatSurface() ? flatGroundSpeed : unevenMoveSpeed;

        model.position = Vector3.Lerp(model.position, aligner.TargetPos + model.up * heightOffFloor, Time.deltaTime * speed);

        var fromRot = model.rotation;
        var toRot = Quaternion.FromToRotation(model.up, aligner.AverageNormal) * model.rotation;

        model.rotation = Quaternion.Slerp(fromRot, toRot, Time.deltaTime * surfaceAlignmentSpeed);

        rotationAmount = Input.GetAxis("Horizontal") * 10;
        rotationAmount *= Time.deltaTime * turnSpeed;

        var targetRotation = body.transform.rotation * Quaternion.Euler(0, rotationAmount, 0);
        body.transform.rotation = Quaternion.RotateTowards(body.transform.rotation, targetRotation, 100);

        var forwardDirection = body.transform.forward;
        model.position += forwardDirection * Time.deltaTime * speed * Input.GetAxis("Vertical");

        SetBodyModelRotation();
    }

    private IEnumerator MoveLegs()
    {
        yield return null;

        while (true)
        {
            do
            {
                for (int i = 0; i < legGroupOne.Length; i++)
                {
                    legGroupOne[i].TryMove();
                }
                SetBodyHeight();
                yield return null;
            }
            while (GetIsMoving(legGroupOne));

            do
            {
                for (int i = 0; i < legGroupTwo.Length; i++)
                {
                    legGroupTwo[i].TryMove();
                }
                SetBodyHeight();
                yield return null;
            }
            while (GetIsMoving(legGroupTwo));

            do
            {
                for (int i = 0; i < legGroupThree.Length; i++)
                {
                    legGroupThree[i].TryMove();
                }
                SetBodyHeight();
                yield return null;
            }
            while (GetIsMoving(legGroupThree));

            yield return null;
        }
    }

    private bool GetIsMoving(SpiderLegStepper[] group)
    {
        for (int i = 0; i < group.Length; i++)
        {
            if (group[i].Moving) return true;
        }

        return false;
    }

    private bool GetOnFlatSurface()
    {
        for (int i = 0; i < allLegs.Count; i++)
        {
            if (!allLegs[i].ShouldOverstep) return false;
        }
        return true;
    }

    private void SetBodyModelRotation()
    {
        CalculateLegAverageHeight();

        var diff = rightLegHeightFromGround - leftLegHeightFromGround;
        
        float finalAngle;  //Keeping track of final angle to keep code cleaner

        finalAngle = bodyStartAngle.y + rotationOffset * diff * Time.deltaTime;  //Calculate animation angle
        bodyModel.localEulerAngles = Vector3.Lerp(bodyStartAngle, new Vector3(bodyStartAngle.x,  bodyStartAngle.y, finalAngle), Time.deltaTime); //Apply new angle to object

    }
    private void CalculateLegAverageHeight()
    {
        previousLeftLegHeightFromGround = leftLegHeightFromGround;
        previousRightLegHeightFromGround = rightLegHeightFromGround;

        leftLegHeightFromGround = 0;
        rightLegHeightFromGround = 0;
        for (int i = 0; i < allLegs.Count; i++)
        {
            var height = allLegs[i].transform.position.y;
            if (allLegs[i].IsLeftLeg) leftLegHeightFromGround += height;
            else rightLegHeightFromGround += height;
        }
    }

}

