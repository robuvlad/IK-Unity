using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawingCubes : MonoBehaviour
{
    private GameObject cube;
    private Transform pen;
    private Vector3 penAdding;

    private float timeToWait = 3.0f;
    private float timeToLive = 10.0f;

    public void Init(GameObject cube, Transform pen)
    {
        this.cube = cube;
        this.pen = pen;
        penAdding = new Vector3(0.0f, -2.0f, 0.0f);
    }

    public void DrawCubes()
    {
        StartCoroutine(DrawOneCube());
    }

    private IEnumerator DrawOneCube()
    {
        while (true)
        {
            yield return new WaitForSeconds(timeToWait);
            var prefab = Instantiate(cube, pen.transform.position + penAdding, Quaternion.identity);
            Destroy(prefab, timeToLive);
        }
    }
}
