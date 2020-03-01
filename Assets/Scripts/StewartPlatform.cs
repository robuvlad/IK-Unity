using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StewartPlatform : MonoBehaviour
{
    Vector3 A1, A2, P;
    Vector3 B1, B2;
    Vector3 S1;
    Matrix4x4 R;

    [Header("Up points")]
    [SerializeField] Transform up1GO = null;
    [SerializeField] Transform up2GO = null;
    [SerializeField] Transform up3GO = null;

    [Header("Bottom Legs")]
    [SerializeField] Transform leg1GO = null;
    [SerializeField] Transform leg2GO = null;
    [SerializeField] Transform leg3GO = null;
    [SerializeField] Transform leg4GO = null;
    [SerializeField] Transform leg5GO = null;
    [SerializeField] Transform leg6GO = null;

    void Start()
    {
        A1 = new Vector3(-10.0f, -1.0f, 0.0f);
        P = new Vector3(0.0f, 0.0f, 20.0f);
        B1 = new Vector3(-5.0f, -5.0f, 20.0f);

        Vector4 col1 = new Vector4(1.0f, 0.0f, 0.0f, 0.0f);
        Vector4 col2 = new Vector4(0.0f, 1.0f, 0.0f, 0.0f);
        Vector4 col3 = new Vector4(0.0f, 0.0f, 1.0f, 0.0f);
        Vector4 col4 = new Vector4(0.0f, 0.0f, 0.0f, 1.0f);
        R = new Matrix4x4(col1, col2, col3, col4);
        Vector4 newB = new Vector4(B1.x, B1.y, B1.z, 0.0f);
        Vector4 result = R * newB;
        Vector3 finalB = new Vector3(result.x, result.y, result.z);

        S1 = P + finalB - A1;

        Debug.Log("vector " + S1);
        Debug.Log("lungime " + S1.magnitude);

        RotateLegs();

        FindBaseValues(2.0f, 3.1f, 120.0f);
    }

    private void RotateLegs()
    {
        Vector3 direction1 = up1GO.position - leg1GO.position;
        Vector3 direction2 = up3GO.position - leg2GO.position;
        Vector3 direction3 = up3GO.position - leg3GO.position;
        Vector3 direction4 = up2GO.position - leg4GO.position;
        Vector3 direction5 = up2GO.position - leg5GO.position;
        Vector3 direction6 = up1GO.position - leg6GO.position;

        leg1GO.rotation = Quaternion.LookRotation(direction1);
        leg2GO.rotation = Quaternion.LookRotation(direction2);
        leg3GO.rotation = Quaternion.LookRotation(direction3);
        leg4GO.rotation = Quaternion.LookRotation(direction4);
        leg5GO.rotation = Quaternion.LookRotation(direction5);
        leg6GO.rotation = Quaternion.LookRotation(direction6);
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
