using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    void Start()
    {
        RotateLegs();
        DoInverseKinematics();

        //FindBaseValues(2.0f, 3.1f, 120.0f);
    }

    void Update()
    {
        RotateLegs();
        DoInverseKinematics();
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
        Vector4 col1 = GetOneColumn(1.0f, 0.0f, 0.0f, 0.0f);
        Vector4 col2 = GetOneColumn(0.0f, 1.0f, 0.0f, 0.0f);
        Vector4 col3 = GetOneColumn(0.0f, 0.0f, 1.0f, 0.0f);
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

        currentChild.localPosition = new Vector3(0.0f, 0.0f + difference / 2.0f, 0.0f);
        //difference / currentFinalLeg.parent.localScale.y

        Debug.Log(sMagnitude);
        Debug.Log("diff " + difference);
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
