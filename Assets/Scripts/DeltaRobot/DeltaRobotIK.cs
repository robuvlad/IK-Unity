using Assets.Scripts.DeltaRobot;
using Assets.Scripts.DeltaRobot.ik;
using Assets.Scripts.DeltaRobot.init;
using Assets.Scripts.DeltaRobot.pick_and_place;
using Assets.Scripts.DeltaRobot.utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeltaRobotIK : MonoBehaviour
{
    [Header("Triangle Points")]
    [SerializeField] Transform[] topVerticesPoints = null;      // vertices points from the top triangle / platform
    [SerializeField] Transform[] BPoints = null;                // B points from the top triangle / platform = middle side points
    [SerializeField] Transform[] PPoints = null;                // P points = vertices points from the bottom triangle / platform

    [Header("Legs")]
    [SerializeField] Transform[] topLPivots = null;             // top pivots used for rotation
    [SerializeField] Transform[] parallelograms = null;         // 6 legs to form 3 parallelograms

    [Header("End Effector")]
    [SerializeField] Transform endEffector = null;              // end Effector = mobile platform = bottom platform

    [Header("UI Sliders")]
    [SerializeField] Slider[] sliders = null;                   // GUI sliders for changing the end effector position

    [Header ("Pick and Place Simulation")]
    [SerializeField] GameObject trajectory = null;

    private double W_B, U_B, S_B;                               // configuration values for the top triangle / platform
    private double W_P, U_P, S_P;                               // configuration values for the bottom triangle / platform
    private double[] thetas;                                    // final angles to be calculated
    private double L;                                           // length of the upper part of the legs
    private double l;                                           // length of the bottom part of the legs

    private double[] Ei;                                        // variable equation
    private double[] Fi;                                        // variable equation
    private double[] Gi;                                        // variable equation

    private ServiceConverter serviceConverter;                  // contains converters: radians, degrees, double, float
    private ServiceInitializer serviceInitializer;              // initialize the base values, siders
    private LegsGenerator legsGenerator;                        // generates the roots for the last equation
    private LegsRotation legsRotation;                          // rotate the whole legs
    private PickAndPlaceSimulation simulation;                  // pick and place simulation for IK method


    private const int TOTAL_NO_JOINTS = 3;

    private void Start()
    {
        SetupVariables();
        serviceInitializer.InitTriangles();
        SetupRobotStructure();
        serviceInitializer.InitSliderValuesIK();

        simulation.Init(sliders, trajectory, endEffector);   
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

        simulation = gameObject.AddComponent<PickAndPlaceSimulation>();

        thetas = new double[TOTAL_NO_JOINTS] { 0.0, 0.0, 0.0 };
        Ei = new double[TOTAL_NO_JOINTS];
        Fi = new double[TOTAL_NO_JOINTS];
        Gi = new double[TOTAL_NO_JOINTS];
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
        legsRotation.RotateLegs(thetas, false);
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
