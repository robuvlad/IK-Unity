using Assets.Scripts.DeltaRobot;
using MathNet.Numerics;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeltaRobotFK : MonoBehaviour
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

    [Header("UI Slider")]
    [SerializeField] Slider[] sliders = null;

    private double[] thetas;
    private Vector3[] centresCircles = new Vector3[3];

    private double L;
    private double l;
    private double W_B, U_B, S_B;
    private double W_P, U_P, S_P;

    private int NO_OF_LEGS;
    private double[] coefficients;
    private double[,] a;
    private double[] b;
    private double[] c;
    private Vector3 endEffectorPosition;

    void Start()
    {
        Setup();
        InitTriangles();
        InitThetas();
        InitSliderValues();
    }

    void Update()
    {
        DoForwardKinematics();
    }

    private void Setup()
    {
        L = DeltaRobotFKUtils.L;
        l = DeltaRobotFKUtils.l;
        NO_OF_LEGS = DeltaRobotFKUtils.NO_OF_LEGS;
        coefficients = new double[NO_OF_LEGS];
        a = new double[DeltaRobotFKUtils.FIRST_ROW_NUMBER, DeltaRobotFKUtils.FIRST_COLUMN_NUMBER];
        b = new double[DeltaRobotFKUtils.SECOND_NUMBER];
        c = new double[DeltaRobotFKUtils.THIRD_NUMBER];
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

    private void InitThetas()
    {
        thetas = new double[NO_OF_LEGS];
        thetas[0] = 38;
        thetas[1] = 22;
        thetas[2] = 15;
    }

    private void InitSliderValues()
    {
        for(int i=0; i < NO_OF_LEGS; i++) {
            sliders[i].minValue = DeltaRobotFKUtils.SLIDER_MIN_VALUE;
            sliders[i].maxValue = DeltaRobotFKUtils.SLIDER_MAX_VALUE;
        }    
        sliders[0].value = 38.0f;
        sliders[1].value = 22.0f;
        sliders[2].value = 15.0f;
        //38, 28, 12
    }

    private void DoForwardKinematics()
    {
        UpdateSliderValues();
        UpdateCentresCircles();
        UpdateFirstVariables();
        UpdateSecondVariables();
        UpdateCoefficients();

        FindRootsFromEquation();
        endEffector.localPosition = endEffectorPosition;

        RotateLegs();
        RotateParallelograms();
    }

    private void UpdateSliderValues()
    {
        thetas[0] = sliders[0].value;
        thetas[1] = sliders[1].value;
        thetas[2] = sliders[2].value;
    }

    private void UpdateCentresCircles()
    {
        double radian1 = ConvertDegreesToRadians(thetas[0]);
        double radian2 = ConvertDegreesToRadians(thetas[1]);
        double radian3 = ConvertDegreesToRadians(thetas[2]);

        float x = 0.0f;
        float y = ConvertDoubleToFloat(-W_B - L * Math.Cos(radian1) + U_P);
        float z = ConvertDoubleToFloat(-L * Math.Sin(radian1));
        SetCentreOfSpecificCircle(0, x, y, z);

        x = ConvertDoubleToFloat((Math.Sqrt(3) / 2.0) * (W_B + L * Math.Cos(radian2)) - S_P / 2.0);
        y = ConvertDoubleToFloat((1.0 / 2.0) * (W_B + L * Math.Cos(radian2)) - W_P);
        z = ConvertDoubleToFloat(-L * Math.Sin(radian2));
        SetCentreOfSpecificCircle(1, x, y, z);

        x = ConvertDoubleToFloat(-(Math.Sqrt(3) / 2.0) * (W_B + L * Math.Cos(radian3)) + S_P / 2.0);
        y = ConvertDoubleToFloat((1.0 / 2.0) * (W_B + L * Math.Cos(radian3)) - W_P);
        z = ConvertDoubleToFloat(-L * Math.Sin(radian3));
        SetCentreOfSpecificCircle(2, x, y, z);
    }

    private void SetCentreOfSpecificCircle(int indexCentreOfCircle, float x, float y, float z)
    {
        centresCircles[indexCentreOfCircle] = new Vector3(x, y, z);
    }

    private void UpdateFirstVariables()
    {
        a[0, 0] = 2 * (centresCircles[2].x - centresCircles[0].x);
        a[0, 1] = 2 * (centresCircles[2].y - centresCircles[0].y);
        a[0, 2] = 2 * (centresCircles[2].z - centresCircles[0].z);

        a[1, 0] = 2 * (centresCircles[2].x - centresCircles[1].x);
        a[1, 1] = 2 * (centresCircles[2].y - centresCircles[1].y);
        a[1, 2] = 2 * (centresCircles[2].z - centresCircles[1].z);

        b[0] = Math.Pow(l, 2) - Math.Pow(l, 2) - Math.Pow(centresCircles[0].x, 2) - Math.Pow(centresCircles[0].y, 2) - Math.Pow(centresCircles[0].z, 2) +
            Math.Pow(centresCircles[2].x, 2) + Math.Pow(centresCircles[2].y, 2) + Math.Pow(centresCircles[2].z, 2);
        b[1] = Math.Pow(l, 2) - Math.Pow(l, 2) - Math.Pow(centresCircles[1].x, 2) - Math.Pow(centresCircles[1].y, 2) - Math.Pow(centresCircles[1].z, 2) +
            Math.Pow(centresCircles[2].x, 2) + Math.Pow(centresCircles[2].y, 2) + Math.Pow(centresCircles[2].z, 2);
    }

    private void UpdateSecondVariables()
    {
        c[0] = (a[0, 0] / a[0, 2]) - (a[1, 0] / a[1, 2]);
        c[1] = (a[0, 1] / a[0, 2]) - (a[1, 1] / a[1, 2]);
        c[2] = (b[1] / a[1, 2]) - (b[0] / a[0, 2]);
        c[3] = -(c[1] / c[0]);
        c[4] = -(c[2] / c[0]);
        c[5] = (-a[1, 0] * c[3] - a[1, 1]) / a[1, 2];
        c[6] = (b[1] - a[1, 0] * c[4]) / a[1, 2];
    }

    private void UpdateCoefficients()
    {
        coefficients[0] = Math.Pow(c[3], 2) + 1 + Math.Pow(c[5], 2);
        coefficients[1] = 2 * c[3] * (c[4] - centresCircles[0].x) - 2 * centresCircles[0].y + 2 * c[5] * (c[6] - centresCircles[0].z);
        coefficients[2] = c[4] * (c[4] - 2 * centresCircles[0].x) + c[6] * (c[6] - 2 * centresCircles[0].z) + Math.Pow(centresCircles[0].x, 2) +
            Math.Pow(centresCircles[0].y, 2) + Math.Pow(centresCircles[0].z, 2) - Math.Pow(l, 2);
    }

    private void FindRootsFromEquation()
    {
        double[] coefs = new double[3] { coefficients[2], coefficients[1], coefficients[0] };
        System.Numerics.Complex[] roots = FindRoots.Polynomial(coefs);
        if (float.IsNaN(ConvertDoubleToFloat(roots[0].Real)) || float.IsNaN(ConvertDoubleToFloat(roots[0].Imaginary)))
        {
            Debug.Log("roots " + roots[0]);
            ResolveSingularity();
        }
        else
            for (int i = 0; i < 2; i++)
            {
                if (roots[i].Imaginary == 0)
                {
                    double x = c[3] * roots[i].Real + c[4];
                    double z = c[5] * roots[i].Real + c[6];
                    if (z < 0)
                    {
                        double y = roots[i].Real;
                        endEffectorPosition = new Vector3(ConvertDoubleToFloat(x), ConvertDoubleToFloat(z), ConvertDoubleToFloat(y));
                        Debug.Log(x + "  " + y + "  " + z);
                    }
                }
            }
    }

    private double zn, A, B, C;
    private double x_, y_;
    private double a_, b_, c_, d_, e_, f_;

    private void InitSingularity()
    {
        zn = -L * Math.Sin(ConvertDegreesToRadians(thetas[0]));
        A = 1;
        B = -2 * zn;
        InitX_Y_();
        C = zn * zn - l * l + Math.Pow(x_ - centresCircles[0].x, 2) + Math.Pow(y_ - centresCircles[0].y, 2);
    }

    private void InitX_Y_()
    {
        a_ = 2 * (centresCircles[2].x - centresCircles[0].x);
        b_ = 2 * (centresCircles[2].y - centresCircles[0].y);
        c_ = Math.Pow(l, 2) - Math.Pow(l, 2) - Math.Pow(centresCircles[0].x, 2) - Math.Pow(centresCircles[0].y, 2) + Math.Pow(centresCircles[2].x, 2) +
            Math.Pow(centresCircles[2].y, 2);
        d_ = 2 * (centresCircles[2].x - centresCircles[1].x);
        e_ = 2 * (centresCircles[2].y - centresCircles[1].y);
        f_ = Math.Pow(l, 2) - Math.Pow(l, 2) - Math.Pow(centresCircles[1].x, 2) - Math.Pow(centresCircles[1].y, 2) + Math.Pow(centresCircles[2].x, 2) +
            Math.Pow(centresCircles[2].y, 2);

        x_ = (c_ * e_ - b_ * f_) / (a_ * e_ - b_ * d_);
        y_ = (a_ * f_ - c_ * d_) / (a_ * e_ - b_ * d_);
    }

    private void ResolveSingularity()
    {
        InitSingularity();

        double[] coefs = new double[3] { C, B, A };
        System.Numerics.Complex[] roots = FindRoots.Polynomial(coefs);
        for(int i=0; i < roots.Length; i++)
        {
            if (roots[i].Imaginary == 0 && roots[i].Real < 0)
            {
                endEffectorPosition = new Vector3(ConvertDoubleToFloat(x_), ConvertDoubleToFloat(roots[i].Real), ConvertDoubleToFloat(y_));
                Debug.Log("singularity " + endEffectorPosition);
            }
        }
    }

    private void RotateLegs()
    {
        float angle1 = ConvertDoubleToFloat(thetas[0]);
        float angle2 = ConvertDoubleToFloat(thetas[1]);
        float angle3 = ConvertDoubleToFloat(thetas[2]);

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
        float x = PPoints[2].position.x - parallelograms[4].position.x - 0.25f;
        float y = -(parallelograms[4].position.y - endEffector.position.y);
        float z = -(parallelograms[4].position.z - PPoints[2].position.z + 0.4325f);
        Vector3 target = new Vector3(x, y, z);
        return target;
    }

    private void RotateObjectTowards(Transform currentObject, Vector3 target)
    {
        currentObject.rotation = Quaternion.LookRotation(target);
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

    private double ConvertToTwoDecimalPlaces(double value)
    {
        return Math.Round(value * 100) / 100;
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
