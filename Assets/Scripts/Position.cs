using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Position : MonoBehaviour
{
    void Start()
    {
        FindBaseValues(1.15f, -2.22f, 90.0f);
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
