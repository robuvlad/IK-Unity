﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SphericalWristFK : MonoBehaviour
{
    [SerializeField] Transform manBase = null;
    [SerializeField] Transform joint0 = null;
    [SerializeField] Transform joint1 = null;
    [SerializeField] Transform joint2 = null;
    [SerializeField] Transform endEffector = null;
    [SerializeField] Transform prismaticJoint = null;

    [SerializeField] Text a1Text = null;
    [SerializeField] Text a2Text = null;
    [SerializeField] Text a3Text = null;
    [SerializeField] Text d3Text = null;
    [SerializeField] Text theta1Text = null;
    [SerializeField] Text theta2Text = null;

    private float a1, a2, a3;
    private float theta1, theta2;
    private float d3;

    private Matrix4x4 H0_1, H1_2, H2_3, H0_3;

    private float[][] table;

    void Start()
    {
        Init();
        SetTable();
        SetTexts();

        H0_1 = GetMatrix(theta1, 90.0f, 0.0f, a1);
        H1_2 = GetMatrix(theta2 + 90.0f, 90.0f, 0.0f, 0.0f);
        H2_3 = GetMatrix(0.0f, 0.0f, 0.0f, a2 + a3 + d3);

        H0_3 = H0_1 * H1_2 * H2_3;

        Debug.Log("Col 0 " + H0_3.GetColumn(0));
        Debug.Log("Col 1 " + H0_3.GetColumn(1));
        Debug.Log("Col 2 " + H0_3.GetColumn(2));
        Debug.Log("Col 3 " + H0_3.GetColumn(3));
    }

    private void Init()
    {
        a1 = joint1.position.y - manBase.position.y;
        a2 = joint2.position.x - joint1.position.x;
        a3 = endEffector.position.x - joint2.position.x;
        theta1 = 0.0f;
        theta2 = 70.0f;
        d3 = 2.0f;

        Debug.Log(a1);
        Debug.Log(a2);
        Debug.Log(a3);

        prismaticJoint.position = new Vector3(prismaticJoint.position.x + d3, prismaticJoint.position.y, prismaticJoint.position.z);
        joint1.rotation = Quaternion.AngleAxis(90.0f - theta2, -Vector3.forward);
        joint0.rotation = Quaternion.AngleAxis(theta1, -Vector3.up);
    }

    private void SetTable()
    {
        table = new float[3][];
        table[0] = new float[4];
        table[1] = new float[4];
        table[2] = new float[4];

        table[0][0] = theta1;
        table[1][0] = theta2;
        table[2][0] = 0.0f;

        table[0][1] = 90.0f;
        table[1][1] = 90.0f;
        table[2][1] = 0.0f;

        table[0][2] = 0.0f;
        table[1][2] = 0.0f;
        table[2][2] = 0.0f;

        table[0][3] = a1;
        table[1][3] = 0.0f;
        table[2][3] = a2 + a3 + d3;
    }

    private Matrix4x4 GetMatrix(float theta, float alpha, float r, float d)
    {
        float thetaR = FromDtoR(theta);
        float alphaR = FromDtoR(alpha);

        Vector4 col0 = new Vector4(Mathf.Cos(thetaR), Mathf.Sin(thetaR), 0.0f, 0.0f);
        Vector4 col1 = new Vector4(-Mathf.Sin(thetaR) * Mathf.Cos(alphaR), Mathf.Cos(thetaR) * Mathf.Cos(alphaR), Mathf.Sin(alphaR), 0.0f);
        Vector4 col2 = new Vector4(Mathf.Sin(thetaR) * Mathf.Sin(alphaR), -Mathf.Cos(thetaR) * Mathf.Sin(alphaR), Mathf.Cos(alphaR), 0.0f);
        Vector4 col3 = new Vector4(r * Mathf.Cos(thetaR), r * Mathf.Sin(thetaR), d, 1.0f);

        Matrix4x4 matrix = new Matrix4x4(col0, col1, col2, col3);
        return matrix;
    }

    private float FromDtoR(float angle)
    {
        return angle * Mathf.PI / 180.0f;
    }

    private void SetTexts()
    {
        a1Text.text = "A1: " + a1.ToString();
        a2Text.text = "A2: " + a2.ToString();
        a3Text.text = "A3: " + a3.ToString();
        d3Text.text = "D3: " + d3.ToString();
        theta1Text.text = "Theta1: " + theta1.ToString();
        theta2Text.text = "Theta2: " + theta2.ToString();
    }
}
