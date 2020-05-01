using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class FabricIK : MonoBehaviour
{
    [SerializeField] int chainLength = 3;
    [SerializeField] Transform target = null;
    [SerializeField] int iterations = 10;
    [SerializeField] float epsilon = 0.001f;

    private Transform[] joints;
    private Vector3[] positions;
    private float[] jointsLength;
    private float totalLength;

    private Vector3[] directions;
    private Quaternion[] rotations;
    private Quaternion rotationTarget;

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

        directions = new Vector3[chainLength + 1];
        rotations = new Quaternion[chainLength + 1];
        rotationTarget = target.rotation;

        var current = this.transform;
        for(int i = joints.Length - 1; i >= 0; i--)
        {
            joints[i] = current;
            rotations[i] = current.rotation;

            if (i == joints.Length - 1)
            {
                directions[i] = target.position - current.position;
            } else
            {
                directions[i] = joints[i + 1].position - current.position;
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

        SetPositions();

        if ((target.position - joints[0].position).sqrMagnitude >= totalLength * totalLength)
        {
            SetPositionsIfGreater();
        } else
        {
            ResolveBackwardsForwards();
        }

        SetJoints();
    }


    private void SetPositionsIfGreater()
    {
        var direction = (target.position - positions[0]).normalized;
        for (int i = 1; i < joints.Length; i++)
        {
            positions[i] = direction * jointsLength[i - 1] + positions[i - 1];
        }
    }

    private void ResolveBackwardsForwards()
    {
        for (int i = 0; i < iterations; i++)
        {
            //back
            for (int j = positions.Length - 1; j > 0; j--)
            {
                if (j == positions.Length - 1)
                    positions[j] = target.position;
                else
                    positions[j] = (positions[j] - positions[j + 1]).normalized * jointsLength[j] + positions[j + 1];
            }

            //forward
            for (int j = 1; j < positions.Length; j++)
                positions[j] = (positions[j] - positions[j - 1]).normalized * jointsLength[j - 1] + positions[j - 1];

            //close enough
            if ((positions[positions.Length - 1] - target.position).sqrMagnitude < epsilon * epsilon)
                break;
        }
    }

    private void SetPositions()
    {
        for (int i = 0; i < joints.Length; i++)
        {
            positions[i] = joints[i].position;
        }
    }

    private void SetJoints()
    {
        for (int i = 0; i < positions.Length; i++)
        {
            if (i == positions.Length - 1)
                joints[i].rotation = target.rotation * Quaternion.Inverse(rotationTarget) * rotations[i];
            else
                joints[i].rotation = Quaternion.FromToRotation(directions[i], positions[i + 1] - positions[i]) * rotations[i];
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
