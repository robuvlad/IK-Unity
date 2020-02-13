using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class FabricIK : MonoBehaviour
{
    [SerializeField] int chainLength = 3;
    [SerializeField] Transform target;

    private Transform[] joints;
    private Vector3[] positions;
    private float[] jointsLength;
    private float totalLength;

    void Awake()
    {
        Init();
    }

    public void Init()
    {
        joints = new Transform[chainLength + 1];
        positions = new Vector3[chainLength + 1];
        jointsLength = new float[chainLength];
        totalLength = 0.0f;


    }

    public void OnDrawGizmos()
    {
        var current = this.transform;
        for(int i=0; i < chainLength && current != null && current.parent != null; i++)
        {
            var scale = Vector3.Distance(current.position, current.parent.position) * 0.1f;
            Handles.matrix = Matrix4x4.TRS(current.position, Quaternion.FromToRotation(Vector3.up, current.parent.position - current.position),
                new Vector3(scale, Vector3.Distance(current.parent.position, current.position), scale));
            Handles.color = Color.green;
            Handles.DrawWireCube(Vector3.up * 0.5f, Vector3.one);

            current = current.parent;
        }
    }
}
