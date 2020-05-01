using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Position : MonoBehaviour
{
    void Start()
    {
        FindBaseValues(0.0f, 0.0f, 1.77f, 1.77f, 30.0f);
        FindBaseValues2(0.0f, 0.0f, 2.5f, 435.0f);
    }

    private void FindBaseValues(float x_c, float y_c, float x, float y, float angle)
    {
        float angleRadians = FromDegreesToRadians(angle);
        float x2 = x - x_c;
        float y2 = y - y_c;

        Vector2 vec1 = new Vector2(x, y);
        Vector2 vec2 = new Vector2(Mathf.Cos(angleRadians) * x2 - Mathf.Sin(angleRadians) * y2 + x_c,
            Mathf.Cos(angleRadians) * y2 + Mathf.Sin(angleRadians) * x2 + y_c);
        Debug.Log("Vec1: " + vec1.ToString("F2"));
        Debug.Log("Vec2: " + vec2.ToString("F2"));
    }

    private void FindBaseValues2(float x_c, float y_c, float radius, float angle)
    {
        float angleRadians = FromDegreesToRadians(angle);

        Vector2 vec = new Vector2(radius * Mathf.Cos(angleRadians) + x_c, radius * Mathf.Sin(angleRadians) + y_c);
        Debug.Log("Vec1 " + vec.ToString("F2"));
    }

    private float FromDegreesToRadians(float theta)
    {
        return theta * Mathf.PI / 180.0f;
    }
}
