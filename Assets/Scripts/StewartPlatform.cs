using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StewartPlatform : MonoBehaviour
{
    [Header("End effector")]
    [SerializeField] Transform endEffector = null;

    [Header("Base")]
    [SerializeField] Transform baseEffector = null;

    [Header("Bottom Legs GO")]
    [SerializeField] Transform[] bottomLegs = null;

    [Header("Top Legs GO")]
    [SerializeField] Transform[] topLegs = null;

    [Header("Final legs GO")]
    [SerializeField] Transform[] finalLegs = null;

    [Header("Children GO")]
    [SerializeField] Transform[] children = null;

    [Header("UI")]
    [SerializeField] Slider[] sliders = null;

    private float phi_roll = 0.0f, theta_pitch = 0.0f, psi_yaw = 0.0f;

    void Start()
    {
        RotateLegs();
        DoInverseKinematics();

        InitializeSlider();

        //FindBaseValues(5.0f, 0.0f, 120.0f);
    }

    void Update()
    {
        RotateLegs();
        DoInverseKinematics();

        SetEndEffectorPosition(sliders[2].value, sliders[0].value, sliders[1].value, sliders[4].value, sliders[5].value, sliders[3].value);
    }

    private void RotateLegs()
    {
        Vector3 direction1 = topLegs[0].position - bottomLegs[0].position;
        Vector3 direction2 = topLegs[1].position - bottomLegs[1].position;
        Vector3 direction3 = topLegs[1].position - bottomLegs[2].position;
        Vector3 direction4 = topLegs[3].position - bottomLegs[3].position;
        Vector3 direction5 = topLegs[3].position - bottomLegs[4].position;
        Vector3 direction6 = topLegs[0].position - bottomLegs[5].position;

        bottomLegs[0].rotation = Quaternion.LookRotation(direction1);
        bottomLegs[1].rotation = Quaternion.LookRotation(direction2);
        bottomLegs[2].rotation = Quaternion.LookRotation(direction3);
        bottomLegs[3].rotation = Quaternion.LookRotation(direction4);
        bottomLegs[4].rotation = Quaternion.LookRotation(direction5);
        bottomLegs[5].rotation = Quaternion.LookRotation(direction6);
    }

    private void DoInverseKinematics()
    {
        float phi = FromDegreesToRadians(phi_roll);
        float theta = FromDegreesToRadians(theta_pitch);
        float psi = FromDegreesToRadians(psi_yaw);

        Vector4 col1 = GetOneColumn(Mathf.Cos(psi) * Mathf.Cos(theta), Mathf.Sin(psi) * Mathf.Sin(theta), -Mathf.Sin(theta), 0.0f);
        Vector4 col2 = GetOneColumn(-Mathf.Sin(psi) * Mathf.Cos(phi) + Mathf.Cos(psi) * Mathf.Sin(theta) * Mathf.Sin(phi),
            Mathf.Cos(psi) * Mathf.Cos(phi) + Mathf.Sin(psi) * Mathf.Sin(theta) * Mathf.Sin(phi), Mathf.Cos(theta) * Mathf.Sin(phi), 0.0f);
        Vector4 col3 = GetOneColumn(Mathf.Sin(psi) * Mathf.Sin(phi) + Mathf.Cos(psi) * Mathf.Sin(theta) * Mathf.Cos(phi),
            -Mathf.Cos(psi) * Mathf.Sin(phi) + Mathf.Sin(psi) * Mathf.Sin(theta) * Mathf.Cos(phi), Mathf.Cos(theta) * Mathf.Cos(phi), 0.0f);
        Vector4 col4 = GetOneColumn(0.0f, 0.0f, 0.0f, 1.0f);
        Matrix4x4 matrix = GetMatrix4x4(col1, col2, col3, col4);

        Vector3 p = GetP();

        for(int i=0; i< bottomLegs.Length; i++)
        {
            Vector3 a = GetA(bottomLegs[i]);
            Vector3 b = GetB(topLegs[i]);
            Vector3 bFrame0 = GetBFrame0(b, matrix);
            Vector3 s = GetS(p, a, bFrame0);
            DrawIK(s, finalLegs[i], bottomLegs[i], children[i]);
        }
    }

    private Vector4 GetOneColumn(float xValue, float yValue, float zValue, float wValue)
    {
        Vector4 column = new Vector4(xValue, yValue, zValue, wValue);
        return column;
    }

    private Matrix4x4 GetMatrix4x4(Vector4 col1, Vector4 col2, Vector4 col3, Vector4 col4)
    {
        Matrix4x4 matrix = new Matrix4x4(col1, col2, col3, col4);
        return matrix;
    }

    private void DrawIK(Vector3 s, Transform currentFinalLeg, Transform currentLeg, Transform currentChild)
    {
        float sMagnitude = s.magnitude;
        float distance = (currentFinalLeg.position - currentLeg.position).magnitude;
        float difference = sMagnitude - distance;

        currentChild.localPosition = new Vector3(0.0f, 0.0f + difference / currentFinalLeg.parent.localScale.y, 0.0f);
    }

    private Vector3 GetP()
    {
        return endEffector.position - baseEffector.position;
    }

    private Vector3 GetA(Transform legGO)
    {
        return legGO.position - baseEffector.position;
    }

    private Vector3 GetB(Transform upGO)
    {
        return upGO.position - endEffector.position;
    }

    private Vector3 GetBFrame0(Vector3 b, Matrix4x4 matrix)
    {
        Vector4 bFrame1 = new Vector4(b.x, b.y, b.z, 0.0f);
        Vector4 matrixResult = matrix * bFrame1;
        Vector3 bFrame0 = new Vector3(matrixResult.x, matrixResult.y, matrixResult.z);
        return bFrame0;
    }

    private Vector3 GetS(Vector3 p, Vector3 a, Vector3 bFrame0)
    {
        return p - a + bFrame0;
    }


    private void InitializeSlider()
    {
        //heaven
        sliders[0].minValue = 3.0f;
        sliders[0].maxValue = 6.0f;
        sliders[0].value = 4.5f;

        //surge
        sliders[1].minValue = -2.0f;
        sliders[1].maxValue = 2.0f;
        sliders[1].value = 0.0f;

        //sway
        sliders[2].minValue = -2.0f;
        sliders[2].maxValue = 2.0f;
        sliders[2].value = 0.0f;

        //roll
        sliders[3].minValue = -30.0f;
        sliders[3].maxValue = 30.0f;
        sliders[3].value = 0.0f;

        //pitch
        sliders[4].minValue = -30.0f;
        sliders[4].maxValue = 30.0f;
        sliders[4].value = 0.0f;

        //yaw
        sliders[5].minValue = -30.0f;
        sliders[5].maxValue = 30.0f;
        sliders[5].value = 0.0f;
    }

    private void SetEndEffectorPosition(float xPos, float yPos, float zPos, float xRot, float yRot, float zRot)
    {
        endEffector.position = new Vector3(xPos, yPos, zPos);
        endEffector.rotation = Quaternion.Euler(xRot, yRot, zRot);
    }

    private void FindBaseValues(float x, float y, float angle)
    {
        float angleRadians = FromDegreesToRadians(angle);

        Vector2 vec1 = new Vector2(x, y);
        Vector2 vec2 = new Vector2(Mathf.Cos(angleRadians) * vec1.x - Mathf.Sin(angleRadians) * vec1.y,
            Mathf.Cos(angleRadians) * vec1.y + Mathf.Sin(angleRadians) * vec1.x);
        Vector2 vec3 = new Vector2(Mathf.Cos(angleRadians) * vec2.x - Mathf.Sin(angleRadians) * vec2.y,
            Mathf.Cos(angleRadians) * vec2.y + Mathf.Sin(angleRadians) * vec2.x);
        Debug.Log("Vec1: " + vec1.ToString("F2"));
        Debug.Log("Vec2: " + vec2.ToString("F2"));
        Debug.Log("Vec3: " + vec3.ToString("F2"));
    }

    private float FromDegreesToRadians(float theta)
    {
        return theta * Mathf.PI / 180.0f;
    }
    
}
