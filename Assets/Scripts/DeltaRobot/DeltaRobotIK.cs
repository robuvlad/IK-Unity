using Assets.Scripts.DeltaRobot;
using Assets.Scripts.DeltaRobot.ik;
using Assets.Scripts.DeltaRobot.init;
using Assets.Scripts.DeltaRobot.utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeltaRobotIK : MonoBehaviour
{
    [Header("Triangle Points")]
    [SerializeField] Transform[] topVerticesPoints = null;
    [SerializeField] Transform[] BPoints = null;
    [SerializeField] Transform[] PPoints = null;

    [Header("Legs")]
    [SerializeField] Transform[] topLPivots = null;
    [SerializeField] Transform[] parallelograms = null;

    [Header("End Effector")]
    [SerializeField] Transform endEffector = null;

    [Header("UI Sliders")]
    [SerializeField] Slider[] sliders = null;

    private double W_B, U_B, S_B;
    private double W_P, U_P, S_P;
    private double[] thetas;
    private double L;
    private double l;

    private double[] Ei;
    private double[] Fi;
    private double[] Gi;

    private ServiceConverter serviceConverter;
    private ServiceInitializer serviceInitializer;
    private LegsGenerator legsGenerator;
    private LegsRotation legsRotation;

    private void Start()
    {
        SetupVariables();
        serviceInitializer.InitTriangles();
        SetupRobotStructure();
        serviceInitializer.InitSliderValuesIK();
    }

    private void Update()
    {
        DoInverseKinematics();
        UpdateEndEffectorPosition();
    }

    private void SetupVariables()
    {
        serviceConverter = new ServiceConverter();
        serviceInitializer = ScriptableObject.CreateInstance<ServiceInitializer>();
        serviceInitializer.Init(serviceConverter, topVerticesPoints, PPoints, sliders);

        thetas = new double[3] { 0.0, 0.0, 0.0 };
        Ei = new double[3];
        Fi = new double[3];
        Gi = new double[3];
        L = DeltaRobotIKUtils.L;
        l = DeltaRobotIKUtils.l;
    }

    private void SetupRobotStructure()
    {
        W_B = serviceInitializer.W_B;
        U_B = serviceInitializer.U_B;
        S_B = serviceInitializer.S_B;
        W_P = serviceInitializer.W_P;
        U_P = serviceInitializer.U_P;
        S_P = serviceInitializer.S_P;

        legsGenerator = ScriptableObject.CreateInstance<LegsGenerator>();
        legsGenerator.Init(L, l, Get_a(), Get_b(), Get_c());

        legsRotation = ScriptableObject.CreateInstance<LegsRotation>();
        legsRotation.Init(serviceConverter, topLPivots, parallelograms, PPoints, endEffector);
    }

    private void DoInverseKinematics()
    {
        Vector3 P = new Vector3(endEffector.localPosition.x, endEffector.localPosition.z, endEffector.localPosition.y);
        legsGenerator.GenerateEi(P);
        legsGenerator.GenerateFi(P);
        legsGenerator.GenerateGi(P);
        SetLegsGenerator();
        for (int i = 0; i < DeltaRobotIKUtils.NO_OF_LEGS; i++)
        {
            double currentFi = serviceConverter.ConvertToTwoDecimalPlaces(Fi[i]);
            double currentEi = serviceConverter.ConvertToTwoDecimalPlaces(Ei[i]);
            double currentGi = serviceConverter.ConvertToTwoDecimalPlaces(Gi[i]);
            double roots = legsGenerator.GetRootsTi(currentFi, currentEi, currentGi);
            thetas[i] = 2 * Math.Atan(roots);
        }
        legsRotation.RotateLegs(thetas);
        legsRotation.RotateParallelograms();
    }

    private void SetLegsGenerator()
    {
        Ei = legsGenerator.Ei;
        Fi = legsGenerator.Fi;
        Gi = legsGenerator.Gi;
    }

    private void UpdateEndEffectorPosition()
    {
        endEffector.localPosition = new Vector3(sliders[0].value, sliders[2].value, sliders[1].value);
    }

    private double Get_a()
    {
        return W_B - U_P;
    }

    private double Get_b()
    {
        return ((S_P / 2.0) - (Math.Sqrt(3) / 2.0) * W_B);
    }

    private double Get_c()
    {
        return (W_P - (1.0 / 2.0) * W_B);
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        DrawLine(BPoints[0].position, BPoints[1].position, BPoints[2].position);
        Gizmos.color = Color.black;
        DrawLine(topVerticesPoints[0].position, topVerticesPoints[1].position, topVerticesPoints[2].position);
        Gizmos.color = Color.red;
        DrawLine(PPoints[0].position, PPoints[1].position, PPoints[2].position);        
    }

    private void DrawLine(Vector3 vec1, Vector3 vec2, Vector3 vec3)
    {
        Gizmos.DrawLine(vec1, vec2);
        Gizmos.DrawLine(vec2, vec3);
        Gizmos.DrawLine(vec3, vec1);
    }
}
