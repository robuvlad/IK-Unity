using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeltaRobotIK : MonoBehaviour
{
    [SerializeField] Transform[] topVerticesPoints = null;
    [SerializeField] Transform[] BPoints = null;
    [SerializeField] Transform[] PPoints = null;

    [SerializeField] Transform[] topLPivots = null;
    

    [SerializeField] Transform sth = null;
    [SerializeField] Transform sth2 = null;

    private double W_B, U_B, S_B;
    private double W_P, U_P, S_P;
    private double[] thetas = new double[3];
    private double L = 1.0;
    private Vector3[] Li = new Vector3[3];

    void Start()
    {
        InitTriangles();
        InitThetas();
        CalculateLi();

        Debug.Log(sth2.position - sth.position);
    }

    private void InitTriangles()
    {
        S_B = ConvertToTwoDecimalPlaces((topVerticesPoints[0].position - topVerticesPoints[1].position).magnitude);
        U_B = ConvertToTwoDecimalPlaces(Mathf.Sqrt(3) / 3 * S_B);
        W_B = ConvertToTwoDecimalPlaces(Mathf.Sqrt(3) / 6 * S_B);

        S_P = ConvertToTwoDecimalPlaces((PPoints[0].position - PPoints[1].position).magnitude);
        U_P = ConvertToTwoDecimalPlaces(Mathf.Sqrt(3) / 3 * S_P);
        W_P = ConvertToTwoDecimalPlaces(Mathf.Sqrt(3) / 6 * S_P);

        Debug.Log(W_B + " " + U_B + " " + S_B + " " + W_P + " " + U_P + " " + S_P);
    }

    private void InitThetas()
    {
        thetas[0] = 45.0;
        thetas[1] = 60.0;
        thetas[2] = 30.0;
    }

    private void CalculateLi()
    {
        double angle_1 = ConvertDegreesToRadians(thetas[0]);
        double angle_2 = ConvertDegreesToRadians(thetas[1]);
        double angle_3 = ConvertDegreesToRadians(thetas[2]);

        float x_1 = 0.0f;
        float y_1 = ConvertDoubleToFloat(-L * Math.Cos(angle_1));
        float z_1 = ConvertDoubleToFloat(-L * Math.Sin(angle_1));
        Debug.Log("1  " + x_1 + "  " + y_1 + "  " + z_1);

        float x_2 = ConvertDoubleToFloat((Math.Sqrt(3) / 2.0) * L * Math.Cos(angle_2));
        float y_2 = ConvertDoubleToFloat((1.0 / 2.0) * L * Math.Cos(angle_2));
        float z_2 = ConvertDoubleToFloat(-L * Math.Sin(angle_2));
        Debug.Log("2  " + x_2 + "  " + y_2 + "  " + z_2);

        float x_3 = ConvertDoubleToFloat((-Math.Sqrt(3) / 2.0) * L * Math.Cos(angle_3));
        float y_3 = ConvertDoubleToFloat((1.0 / 2.0) * L * Math.Cos(angle_3));
        float z_3 = ConvertDoubleToFloat(-L * Math.Sin(angle_3));
        Debug.Log("3  " + x_3 + "  " + y_3 + "  " + z_3);

        topLPivots[0].rotation = Quaternion.LookRotation(new Vector3(-x_1, -z_1, -y_1));
        topLPivots[1].rotation = Quaternion.LookRotation(new Vector3(-x_2, -z_2, -y_2));
        topLPivots[2].rotation = Quaternion.LookRotation(new Vector3(-x_3, -z_3, -y_3));
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
