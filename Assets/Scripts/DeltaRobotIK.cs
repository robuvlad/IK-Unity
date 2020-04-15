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

    [SerializeField] Transform endEffector = null;

    [SerializeField] Transform sth = null;
    [SerializeField] Transform sth2 = null;

    private double W_B, U_B, S_B;
    private double W_P, U_P, S_P;
    private double[] thetas = new double[3];
    private double L = 2.0;
    private double l = 4.0;

    private double[] Ei = new double[3];
    private double[] Fi = new double[3];
    private double[] Gi = new double[3];

    void Start()
    {
        InitTriangles();
        InitThetas();
        CalculateLi();
        Calculateli();
        CalculateAi();


        DoInverseKinematics();

        //Debug.Log("dist " + (sth2.position - sth.position).magnitude);

        /*
        S_B = 0.567;
        U_B = 0.327;
        W_B = 0.164;

        S_P = 0.076;
        U_P = 0.044;
        W_P = 0.022;
        L = 0.524;
        l = 1.244;

        double a = Get_a();
        double b = Get_b();
        double c = Get_c();
        Vector3 P = new Vector3(0.0f, 0.0f, -0.9f);

        GenerateEi(P);
        GenerateFi(P);
        GenerateGi(P);

        for (int i = 0; i < 3; i++)
        {
            double currentFi = Fi[i];
            double currentEi = Ei[i];
            double currentGi = Gi[i];
            double[] roots = GetRootsTi(currentFi, currentEi, currentGi);
            Debug.Log(i + "  " + roots[0]);
            Debug.Log(i + "  " + roots[1]);
        }*/

    }

    private void InitTriangles()
    {
        S_B = ConvertToTwoDecimalPlaces((topVerticesPoints[0].position - topVerticesPoints[1].position).magnitude);
        U_B = ConvertToTwoDecimalPlaces((Mathf.Sqrt(3) / 3.0) * S_B);
        W_B = ConvertToTwoDecimalPlaces((Mathf.Sqrt(3) / 6.0) * S_B);

        S_P = ConvertToTwoDecimalPlaces((PPoints[0].position - PPoints[1].position).magnitude);
        U_P = ConvertToTwoDecimalPlaces((Mathf.Sqrt(3) / 3.0) * S_P);
        W_P = ConvertToTwoDecimalPlaces((Mathf.Sqrt(3) / 6.0) * S_P);

        Debug.Log(W_B + " " + U_B + " " + S_B + " " + W_P + " " + U_P + " " + S_P);
    }

    private void InitThetas()
    {
        thetas[0] = 45.0;
        thetas[1] = 45.0;
        thetas[2] = 45.0;
    }

    private void CalculateLi()
    {
        double angle_1 = ConvertDegreesToRadians(thetas[0]);
        double angle_2 = ConvertDegreesToRadians(thetas[1]);
        double angle_3 = ConvertDegreesToRadians(thetas[2]);

        float x_1 = 0.0f;
        float y_1 = ConvertDoubleToFloat(-L * Math.Cos(angle_1));
        float z_1 = ConvertDoubleToFloat(-L * Math.Sin(angle_1));

        float x_2 = ConvertDoubleToFloat((Math.Sqrt(3) / 2.0) * L * Math.Cos(angle_2));
        float y_2 = ConvertDoubleToFloat((1.0 / 2.0) * L * Math.Cos(angle_2));
        float z_2 = ConvertDoubleToFloat(-L * Math.Sin(angle_2));

        float x_3 = ConvertDoubleToFloat((-Math.Sqrt(3) / 2.0) * L * Math.Cos(angle_3));
        float y_3 = ConvertDoubleToFloat((1.0 / 2.0) * L * Math.Cos(angle_3));
        float z_3 = ConvertDoubleToFloat(-L * Math.Sin(angle_3));

        Vector3 leg1 = new Vector3(-x_1, -z_1, -y_1);
        Vector3 leg2 = new Vector3(-x_2, -z_2, -y_2);
        Vector3 leg3 = new Vector3(-x_3, -z_3, -y_3);

        RotateObjectTowards(topLPivots[0], leg1);
        RotateObjectTowards(topLPivots[1], leg2);
        RotateObjectTowards(topLPivots[2], leg3);
    }

    private void Calculateli()
    {
        double angle_1 = ConvertDegreesToRadians(thetas[0]);
        double angle_2 = ConvertDegreesToRadians(thetas[1]);
        double angle_3 = ConvertDegreesToRadians(thetas[2]);

        float x_1 = ConvertDoubleToFloat(endEffector.localPosition.x);
        float y_1 = ConvertDoubleToFloat(endEffector.localPosition.z + L * Math.Cos(angle_1) + Get_a());
        float z_1 = ConvertDoubleToFloat(endEffector.localPosition.y + L * Math.Sin(angle_1));

        Vector3 leg1 = new Vector3(x_1, y_1, z_1);
        Debug.Log("l1  " + x_1 + "  " + y_1 + "  " + z_1);
    }

    private void CalculateAi()
    {
        double angle_1 = ConvertDegreesToRadians(thetas[0]);
        double angle_2 = ConvertDegreesToRadians(thetas[1]);
        double angle_3 = ConvertDegreesToRadians(thetas[2]);

        float x_1 = 0.0f;
        float y_1 = ConvertDoubleToFloat(-W_B - L * Math.Cos(angle_1));
        float z_1 = ConvertDoubleToFloat(-L * Math.Sin(angle_1));

        Debug.Log("A1  " + x_1 + "  " + y_1 + "  " + z_1);
        Debug.Log("sth " + sth.localPosition);
    }

    private void DoInverseKinematics()
    {
        Vector3 P = new Vector3(endEffector.localPosition.x, endEffector.localPosition.z, endEffector.localPosition.y);
        GenerateEi(P);
        GenerateFi(P);
        GenerateGi(P);
        for(int i = 0; i < 3; i++)
        {
            double currentFi = ConvertToTwoDecimalPlaces(Fi[i]);
            double currentEi = ConvertToTwoDecimalPlaces(Ei[i]);
            double currentGi = ConvertToTwoDecimalPlaces(Gi[i]);
            double[] roots = GetRootsTi(currentFi, currentEi, currentGi);
            Debug.Log("Fi " + currentFi);
            Debug.Log("Ei " + currentEi);
            Debug.Log("Gi " + currentGi);
            Debug.Log(i + "  " + roots[0]);
            Debug.Log(i + "  " + roots[1]);
        }
    }

    private void GenerateEi(Vector3 vec)
    {
        Ei[0] = 2 * L * (vec.y + Get_a());
        Ei[1] = -L * (Math.Sqrt(3) * (vec.x + Get_b()) + vec.y + Get_c());
        Ei[2] = L * (Math.Sqrt(3) * (vec.x - Get_b()) - vec.y - Get_c());
    }

    private void GenerateFi(Vector3 vec)
    {
        for(int i = 0; i < 3; i++)
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
        Gi[1] = Math.Pow(x, 2) + Math.Pow(y, 2) + Math.Pow(z, 2) + Math.Pow(Get_b(), 2) + Math.Pow(Get_c(),2) + Math.Pow(L, 2) + 
            2 * (x * Get_b() + y * Get_c()) - Math.Pow(l, 2);
        Gi[2] = Math.Pow(x, 2) + Math.Pow(y, 2) + Math.Pow(z, 2) + Math.Pow(Get_b(), 2) + Math.Pow(Get_c(), 2) + Math.Pow(L, 2) +
            2 * (-x * Get_b() + y * Get_c()) - Math.Pow(l, 2);
    }

    private double[] GetRootsTi(double Fi, double Ei, double Gi)
    {
        double ti1 = (-Fi + Math.Sqrt(Math.Pow(Ei, 2) + Math.Pow(Fi, 2) - Math.Pow(Gi, 2))) / (Gi - Ei);
        double ti2 = (-Fi - Math.Sqrt(Math.Pow(Ei, 2) + Math.Pow(Fi, 2) - Math.Pow(Gi, 2))) / (Gi - Ei);
        double[] roots = new double[] { ti1, ti2 };
        return roots;
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
