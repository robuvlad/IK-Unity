using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeRotation : MonoBehaviour
{
    private float speed = 25.0f;

    void Update()
    {
        float speedRotation = speed * Time.deltaTime;
        this.transform.Rotate(new Vector3(1.0f, 1.0f, 1.0f) * speedRotation);
    }
}
