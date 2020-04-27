using Assets.Scripts.DeltaRobot;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeltaRobotIK : MonoBehaviour
{
    [Header("Triangle Points")]
    [SerializeField] Transform[] topVerticesPoints = null;
    [SerializeField] Transform[] BPoints = null;
    [SerializeField] Transform[] PPoints = null;

    [Header("Legs")]
    [SerializeField] Transform[] topLPivots = null;
    [SerializeField] Transform[] parallelograms = null;

    [Header("End Effector")]
    [SerializeField] Transform endEffector = null;

    [Header("UI Sliders")]
    [SerializeField] Slider[] sliders = null;

    private double W_B, U_B, S_B;
    private double W_P, U_P, S_P;
    private double[] thetas;
    private double L;
    private double l;

    private double[] Ei;
    private double[] Fi;
    private double[] Gi;

    private void Start()
    {
        SetupVariables();
        InitTriangles();
        InitSliderValues(); 
    }

    private void Update()
    {
        DoInverseKinematics();
        UpdateSliderValues();

        for (int i = 0; i < DeltaRobotIKUtils.NO_OF_LEGS; i++)
            Debug.Log(i + "  " + ConvertRadiansToDegrees(thetas[i]));

        Debug.Log("pos " + endEffector.localPosition);
    }

    private void SetupVariables()
    {
        thetas = new double[3] { 0.0, 0.0, 0.0 };
        Ei = new double[3];
        Fi = new double[3];
        Gi = new double[3];
        L = DeltaRobotIKUtils.L;
        l = DeltaRobotIKUtils.l;
    }

    private void InitTriangles()
    {
        S_B = ConvertToTwoDecimalPlaces((topVerticesPoints[0].position - topVerticesPoints[1].position).magnitude);
        U_B = ConvertToTwoDecimalPlaces((Mathf.Sqrt(3) / 3.0) * S_B);
        W_B = ConvertToTwoDecimalPlaces((Mathf.Sqrt(3) / 6.0) * S_B);

        S_P = ConvertToTwoDecimalPlaces((PPoints[0].position - PPoints[1].position).magnitude);
        U_P = ConvertToTwoDecimalPlaces((Mathf.Sqrt(3) / 3.0) * S_P);
        W_P = ConvertToTwoDecimalPlaces((Mathf.Sqrt(3) / 6.0) * S_P);
    }

    private void InitSliderValues()
    {
        sliders[0].minValue = DeltaRobotIKUtils.X_MIN_VALUE;
        sliders[0].maxValue = DeltaRobotIKUtils.X_MAX_VALUE;
        sliders[0].value = DeltaRobotIKUtils.X_INITIAL_VALUE;

        sliders[1].minValue = DeltaRobotIKUtils.Y_MIN_VALUE;
        sliders[1].maxValue = DeltaRobotIKUtils.Y_MAX_VALUE;
        sliders[1].value = DeltaRobotIKUtils.Y_INITIAL_VALUE;

        sliders[2].minValue = DeltaRobotIKUtils.Z_MIN_VALUE;
        sliders[2].maxValue = DeltaRobotIKUtils.Z_MAX_VALUE;
        sliders[2].value = DeltaRobotIKUtils.Z_INITIAL_VALUE;
    }

    private void UpdateSliderValues()
    {
        endEffector.localPosition = new Vector3(sliders[0].value, sliders[2].value, sliders[1].value);
    }

    private void DoInverseKinematics()
    {
        Vector3 P = new Vector3(endEffector.localPosition.x, endEffector.localPosition.z, endEffector.localPosition.y);
        GenerateEi(P);
        GenerateFi(P);
        GenerateGi(P);
        for(int i = 0; i < DeltaRobotIKUtils.NO_OF_LEGS; i++)
        {
            double currentFi = ConvertToTwoDecimalPlaces(Fi[i]);
            double currentEi = ConvertToTwoDecimalPlaces(Ei[i]);
            double currentGi = ConvertToTwoDecimalPlaces(Gi[i]);
            double roots = GetRootsTi(currentFi, currentEi, currentGi);
            thetas[i] = 2 * Math.Atan(roots);
        }
        RotateLegs();
        RotateParallelograms();
    }

    private void GenerateEi(Vector3 vec)
    {
        Ei[0] = 2 * L * (vec.y + Get_a());
        Ei[1] = -L * (Math.Sqrt(3) * (vec.x + Get_b()) + vec.y + Get_c());
        Ei[2] = L * (Math.Sqrt(3) * (vec.x - Get_b()) - vec.y - Get_c());
    }

    private void GenerateFi(Vector3 vec)
    {
        for (int i = 0; i < DeltaRobotIKUtils.NO_OF_LEGS; i++)
        {
            Fi[i] = 2 * vec.z * L;
        }
    }

    private void GenerateGi(Vector3 vec)
    {
        double x = vec.x;
        double y = vec.y;
        double z = vec.z;

        Gi[0] = Math.Pow(x, 2) + Math.Pow(y, 2) + Math.Pow(z, 2) + Math.Pow(Get_a(), 2) + Math.Pow(L, 2) + 2 * y * Get_a() - Math.Pow(l, 2);
        Gi[1] = Math.Pow(x, 2) + Math.Pow(y, 2) + Math.Pow(z, 2) + Math.Pow(Get_b(), 2) + Math.Pow(Get_c(), 2) + Math.Pow(L, 2) +
            2 * (x * Get_b() + y * Get_c()) - Math.Pow(l, 2);
        Gi[2] = Math.Pow(x, 2) + Math.Pow(y, 2) + Math.Pow(z, 2) + Math.Pow(Get_b(), 2) + Math.Pow(Get_c(), 2) + Math.Pow(L, 2) +
            2 * (-x * Get_b() + y * Get_c()) - Math.Pow(l, 2);
    }

    private double GetRootsTi(double Fi, double Ei, double Gi)
    {
        double ti1 = (-Fi + Math.Sqrt(Math.Pow(Ei, 2) + Math.Pow(Fi, 2) - Math.Pow(Gi, 2))) / (Gi - Ei);
        double ti2 = (-Fi - Math.Sqrt(Math.Pow(Ei, 2) + Math.Pow(Fi, 2) - Math.Pow(Gi, 2))) / (Gi - Ei);
        if (ti1 <= 1.5 && ti1 >= -1.5)
            return ti1;
        return ti2;
    }

    private void RotateLegs()
    {
        float angle1 = ConvertDoubleToFloat(ConvertRadiansToDegrees(thetas[0]));
        float angle2 = ConvertDoubleToFloat(ConvertRadiansToDegrees(thetas[1]));
        float angle3 = ConvertDoubleToFloat(ConvertRadiansToDegrees(thetas[2]));

        topLPivots[0].rotation = Quaternion.Euler(new Vector3(-angle1, topLPivots[0].eulerAngles.y, topLPivots[0].eulerAngles.z));
        topLPivots[1].rotation = Quaternion.Euler(new Vector3(-angle2, topLPivots[1].eulerAngles.y, topLPivots[1].eulerAngles.z));
        topLPivots[2].rotation = Quaternion.Euler(new Vector3(-angle3, topLPivots[2].eulerAngles.y, topLPivots[2].eulerAngles.z));
    }

    private void RotateParallelograms()
    {
        Vector3 firstTarget = GetFirstParallelogram();
        Vector3 secondTarget = GetSecondParallelogram();
        Vector3 thirdTarget = GetThirdParallelogram();
        RotateObjectTowards(parallelograms[0], firstTarget);
        RotateObjectTowards(parallelograms[1], firstTarget);
        RotateObjectTowards(parallelograms[2], secondTarget);
        RotateObjectTowards(parallelograms[3], secondTarget);
        RotateObjectTowards(parallelograms[4], thirdTarget);
        RotateObjectTowards(parallelograms[5], thirdTarget);
    }

    private Vector3 GetFirstParallelogram()
    {
        float x = -(parallelograms[0].position.x - PPoints[0].position.x - 0.5f);
        float y = -(parallelograms[0].position.y - endEffector.position.y);
        float z = PPoints[0].position.z - parallelograms[0].position.z;
        Vector3 target = new Vector3(x, y, z);
        return target;
    }

    private Vector3 GetSecondParallelogram()
    {
        float x = -(parallelograms[2].position.x - PPoints[1].position.x + 0.25f);
        float y = -(parallelograms[2].position.y - endEffector.position.y);
        float z = -(parallelograms[2].position.z - PPoints[1].position.z - 0.4325f);
        Vector3 target = new Vector3(x, y, z);
        return target;
    }

    private Vector3 GetThirdParallelogram()
    {
        float  x = PPoints[2].position.x - parallelograms[4].position.x - 0.25f;
        float y = -(parallelograms[4].position.y - endEffector.position.y);
        float z = -(parallelograms[4].position.z - PPoints[2].position.z + 0.4325f);
        Vector3 target = new Vector3(x, y, z);
        return target;
    }

    private void RotateObjectTowards(Transform currentObject, Vector3 target)
    {
        currentObject.rotation = Quaternion.LookRotation(target);
    }

    private double Get_a()
    {
        return W_B - U_P;
    }

    private double Get_b()
    {
        return ((S_P / 2.0) - (Math.Sqrt(3) / 2.0) * W_B);
    }

    private double Get_c()
    {
        return (W_P - (1.0 / 2.0) * W_B);
    }

    private double ConvertToTwoDecimalPlaces(double value)
    {
        return Math.Round(value * 100) / 100;
    }

    private double ConvertDegreesToRadians(double angle)
    {
        double formula = angle * Math.PI / 180;
        return formula;
    }

    private double ConvertRadiansToDegrees(double angle)
    {
        double formula = (angle * 180.0) / Math.PI;
        return formula;
    }

    private float ConvertDoubleToFloat(double value)
    {
        return Convert.ToSingle(ConvertToTwoDecimalPlaces(value));
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        DrawLine(BPoints[0].position, BPoints[1].position, BPoints[2].position);
        Gizmos.color = Color.black;
        DrawLine(topVerticesPoints[0].position, topVerticesPoints[1].position, topVerticesPoints[2].position);
        Gizmos.color = Color.red;
        DrawLine(PPoints[0].position, PPoints[1].position, PPoints[2].position);        
    }

    private void DrawLine(Vector3 vec1, Vector3 vec2, Vector3 vec3)
    {
        Gizmos.DrawLine(vec1, vec2);
        Gizmos.DrawLine(vec2, vec3);
        Gizmos.DrawLine(vec3, vec1);
    }
}
