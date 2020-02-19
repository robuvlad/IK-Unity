using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SphericalWristFK : MonoBehaviour
{
    [Header("Game Objects")]
    [SerializeField] Transform manBase = null;
    [SerializeField] Transform joint0 = null;
    [SerializeField] Transform joint1 = null;
    [SerializeField] Transform joint2 = null;
    [SerializeField] Transform endEffector = null;
    [SerializeField] Transform prismaticJoint = null;

    [Header("Texts")]
    [SerializeField] Text a1Text = null;
    [SerializeField] Text a2Text = null;
    [SerializeField] Text a3Text = null;
    [SerializeField] Text d3Text = null;
    [SerializeField] Text theta1Text = null;
    [SerializeField] Text theta2Text = null;

    [Header("End Effector")]
    [SerializeField] Text endEffectorXText = null;
    [SerializeField] Text endEffectorYText = null;
    [SerializeField] Text endEffectorZText = null;

    private float a1, a2, a3;
    private float theta1, theta2;
    private float d3;

    private Matrix4x4 H0_1, H1_2, H2_3, H0_3;

    private float[][] table;

    void Start()
    {
        InitParams();
        InitPositions();
        SetTable();
        SetTexts();
        SetDH();
        SetEndEffectorText();
    }

    private void InitParams()
    {
        table = new float[3][];
        table[0] = new float[4];
        table[1] = new float[4];
        table[2] = new float[4];

        a1 = joint1.position.y - manBase.position.y;
        a2 = joint2.position.x - joint1.position.x;
        a3 = endEffector.position.x - joint2.position.x;
        theta1 = 25.0f;
        theta2 = 56.0f;
        d3 = 2.0f;
    }

    private void InitPositions()
    {
        prismaticJoint.position = new Vector3(prismaticJoint.position.x + d3, prismaticJoint.position.y, prismaticJoint.position.z);
        joint1.rotation = Quaternion.AngleAxis(90.0f - theta2, -Vector3.forward);
        joint0.rotation = Quaternion.AngleAxis(theta1, -Vector3.up);
    }

    private void SetTable()
    {
        table[0][0] = theta1;
        table[1][0] = theta2 + 90.0f;
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

    private void SetTexts()
    {
        a1Text.text = "A1: " + a1.ToString();
        a2Text.text = "A2: " + a2.ToString();
        a3Text.text = "A3: " + a3.ToString();
        d3Text.text = "D3: " + d3.ToString();
        theta1Text.text = "Theta1: " + ((int)(theta1)).ToString();
        theta2Text.text = "Theta2: " + ((int)(theta2)).ToString();
    }

    private void SetDH()
    {
        H0_1 = GetMatrix(table[0][0], table[0][1], table[0][2], table[0][3]);
        H1_2 = GetMatrix(table[1][0], table[1][1], table[1][2], table[1][3]);
        H2_3 = GetMatrix(table[2][0], table[2][1], table[2][2], table[2][3]);

        H0_3 = H0_1 * H1_2 * H2_3;
    }

    private void SetEndEffectorText()
    {
        endEffectorXText.text = H0_3.GetColumn(3).x > 0.0001 ? "X " + H0_3.GetColumn(3).x.ToString() : "X " + 0;
        endEffectorYText.text = H0_3.GetColumn(3).y > 0.0001 ? "Y " + H0_3.GetColumn(3).y.ToString() : "Y " + 0;
        endEffectorZText.text = H0_3.GetColumn(3).z > 0.0001 ? "Z " + H0_3.GetColumn(3).z.ToString() : "Z " + 0;
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

    
}
