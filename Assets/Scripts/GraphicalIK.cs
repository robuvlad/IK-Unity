using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GraphicalIK : MonoBehaviour
{
    [SerializeField] Transform joint1 = null;
    [SerializeField] Transform joint2 = null;

    [SerializeField] Slider sliderX = null;
    [SerializeField] Slider sliderY = null;

    float theta1, theta2;
    float a2 = 4.0f;
    float a4 = 6.0f;
    float x02 = 8.0f;
    float y02 = 6.0f;

    void Start()
    {
        sliderX.minValue = 0.0f;
        sliderY.minValue = 0.0f;

        sliderX.maxValue = 7.0f;
        sliderY.maxValue = 7.0f;

        sliderX.value = x02;
        sliderY.value = y02;
    }

    void Update()
    {
        float r1;
        float phi1, phi2, phi3;

        x02 = sliderX.value;
        y02 = sliderY.value;

        r1 = Mathf.Sqrt(Mathf.Pow(x02, 2) + Mathf.Pow(y02, 2));
        phi1 = Mathf.Acos(((Mathf.Pow(a4, 2) - Mathf.Pow(a2, 2) - Mathf.Pow(r1, 2)) / (-2 * a2 * r1)));
        phi2 = Mathf.Atan((y02 / x02));
        theta1 = phi2 - phi1;
        phi3 = Mathf.Acos(((Mathf.Pow(r1, 2) - Mathf.Pow(a2, 2) - Mathf.Pow(a4, 2)) / (-2 * a2 * a4)));
        theta2 = Mathf.PI - phi3;
        theta2 += theta1;

        Debug.Log("theta1: " + theta1 / Mathf.PI * 180.0f);
        Debug.Log("theta2: " + theta2 / Mathf.PI * 180.0f);

        joint1.rotation = Quaternion.AngleAxis(-theta1 / Mathf.PI * 180.0f, Vector3.up);
        joint2.rotation = Quaternion.AngleAxis(-theta2 / Mathf.PI * 180.0f, Vector3.up);
    }
}
