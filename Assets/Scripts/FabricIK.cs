﻿using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class FabricIK : MonoBehaviour
{
    [SerializeField] int chainLength = 3;
    [SerializeField] Transform target;
    [SerializeField] int iterations = 10;
    [SerializeField] float epsilon = 0.001f;

    private Transform[] joints;
    private Vector3[] positions;
    private float[] jointsLength;
    private float totalLength;

    void Awake()
    {
        Init();
    }

    void LateUpdate()
    {
        ResolveIK();
    }

    private void Init()
    {
        joints = new Transform[chainLength + 1];
        positions = new Vector3[chainLength + 1];
        jointsLength = new float[chainLength];
        totalLength = 0.0f;

        var current = this.transform;
        for(int i = joints.Length - 1; i >= 0; i--)
        {
            joints[i] = current;

            if (i == joints.Length - 1)
            {

            } else
            {
                jointsLength[i] = (joints[i + 1].position - joints[i].position).magnitude;
                totalLength += jointsLength[i];
            }

            current = current.parent;
        }
    }

    private void ResolveIK()
    {
        if (target == null)
            return;

        if (chainLength != joints.Length)
            Init();

        for(int i = 0; i < joints.Length; i++)
        {
            positions[i] = joints[i].position;
        }

        if ((target.position - joints[0].position).sqrMagnitude >= totalLength * totalLength)
        {
            var direction = (target.position - joints[0].position).normalized;
            for(int i = 1; i < joints.Length; i++)
            {
                positions[i] = direction * jointsLength[i - 1] + positions[i - 1];
            }
        } else
        {
            for(int i = 0; i < iterations; i++)
            {
                //back
                for(int j = positions.Length - 1; j >= 0; j--)
                {
                    if (j == positions.Length - 1)
                        positions[j] = target.position;
                    else
                        positions[j] = (positions[j + 1] - positions[j]).normalized * jointsLength[j] + positions[j + 1];
                }

                //close enough
                if ((target.position - joints[joints.Length - 1].position).sqrMagnitude < epsilon * epsilon)
                    break;
            }
        }

        for(int i = 0; i < positions.Length; i++)
        {
            joints[i].position = positions[i];
        }
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
