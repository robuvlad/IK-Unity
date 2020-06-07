using Assets.Scripts.DeltaRobot;
using Assets.Scripts.DeltaRobot.drawing;
using Assets.Scripts.DeltaRobot.ik;
using Assets.Scripts.DeltaRobot.init;
using Assets.Scripts.DeltaRobot.logic;
using Assets.Scripts.DeltaRobot.utils;
using MathNet.Numerics;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeltaRobotFK : MonoBehaviour
{
    [Header("Triangle Points")]
    [SerializeField] Transform[] topVerticesPoints = null;                  // vertices points from the top triangle / platform
    [SerializeField] Transform[] BPoints = null;                            // B points from the top triangle / platform = middle side points
    [SerializeField] Transform[] PPoints = null;                            // P points = vertices points from the bottom triangle / platform

    [Header("Legs")]
    [SerializeField] Transform[] topLPivots = null;                         // top pivots used for rotation
    [SerializeField] Transform[] parallelograms = null;                     // 6 legs to form 3 parallelograms

    [Header("End Effector")]
    [SerializeField] Transform endEffector = null;                          // end Effector = mobile platform = bottom platform = OUTPUT

    [Header("UI Slider")]
    [SerializeField] Slider[] sliders = null;                               // GUI sliders for changing the theta angles
    [SerializeField] Text[] texts = null;                                   // GUI texts for theta values

    [Header("Cubes Simulation")]
    [SerializeField] GameObject cube = null;                                // create a set of cubes to simulate FK
    [SerializeField] Transform pen = null;                                  // pen game object

    private double[] thetas;                                                // represent the angle for the 3 joints in degrees = INPUT
    private Vector3[] centresCircles;                                       // the centres of the invisibile circles

    private double L;                                                       // length of the upper part of the legs
    private double l;                                                       // length of the bottom part of the legs
    private double W_B, U_B, S_B;
    private double W_P, U_P, S_P;

    private int NO_OF_LEGS;                                                 // the total number of legs = 3
    private double[] coefficients;                                          // the coefficients for the quadratic equation
    private double[,] a;                                                    // first variables for equation
    private double[] b;                                                     // second variables for equation
    private double[] c;                                                     // third variables for equation
    private Vector3 endEffectorPosition;                                    // the end effector position

    private ServiceConverter serviceConverter;                              // contains converters: radians, degrees, double, float
    private ServiceInitializer serviceInitializer;                          // initialize the base values, siders, angles
    private LegsRotation legsRotation;                                      // rotate the whole legs
    private SpheresCreation circlesCreation;                                // creates the invisible spheres and resolve the final equation

    //private LineRenderer line;                                            // line renderer component used for drawing / painting simulation
    //private DrawingSimulation simulation;                                 // simulation of the FK algorithm
    private DrawingCubes simulation;

    void Start()
    {
        Setup();
        serviceInitializer.InitTriangles();
        SetupRobotStructure();
        serviceInitializer.InitSliderValuesFK();
        setupSimulation();
        //SetupLineRenderer();
    }

    void Update()
    {
        DoForwardKinematics();
        SetupUIValues();
        //simulation.DrawLines();
    }

    private void Setup()
    {
        L = DeltaRobotFKUtils.L;
        l = DeltaRobotFKUtils.l;
        NO_OF_LEGS = DeltaRobotFKUtils.NO_OF_LEGS;
        coefficients = new double[NO_OF_LEGS];
        a = new double[DeltaRobotFKUtils.FIRST_ROW_NUMBER, DeltaRobotFKUtils.FIRST_COLUMN_NUMBER];
        b = new double[DeltaRobotFKUtils.SECOND_NUMBER];
        c = new double[DeltaRobotFKUtils.THIRD_NUMBER];
        thetas = new double[NO_OF_LEGS];
        centresCircles = new Vector3[NO_OF_LEGS];

        serviceConverter = new ServiceConverter();
        serviceInitializer = ScriptableObject.CreateInstance<ServiceInitializer>();
        serviceInitializer.Init(serviceConverter, topVerticesPoints, PPoints, sliders);

        legsRotation = ScriptableObject.CreateInstance<LegsRotation>();
        legsRotation.Init(serviceConverter, topLPivots, parallelograms, PPoints, endEffector);
    }

    private void SetupRobotStructure()
    {
        W_B = serviceInitializer.W_B;
        U_B = serviceInitializer.U_B;
        S_B = serviceInitializer.S_B;
        W_P = serviceInitializer.W_P;
        U_P = serviceInitializer.U_P;
        S_P = serviceInitializer.S_P;

        thetas[0] = DeltaRobotFKUtils.VALUE_THETA_1;
        thetas[1] = DeltaRobotFKUtils.VALUE_THETA_2;
        thetas[2] = DeltaRobotFKUtils.VALUE_THETA_3;

        circlesCreation = ScriptableObject.CreateInstance<SpheresCreation>();
        circlesCreation.Init(serviceConverter, centresCircles, a, b, c, coefficients);
        circlesCreation.InitStructureRobot(W_B, U_B, S_B, W_P, U_P, S_P);
    }

    private void setupSimulation()
    {
        //simulation = ScriptableObject.CreateInstance<DrawingCubes>();
        simulation = gameObject.AddComponent<DrawingCubes>();
        simulation.Init(cube, pen);
        simulation.DrawCubes();
    }

    private void DoForwardKinematics()
    {
        UpdateThetaValues();
        circlesCreation.UpdateInvisibleCircles(thetas);
        
        endEffector.localPosition = circlesCreation.endEffectorPosition;

        legsRotation.RotateLegs(thetas, true);
        legsRotation.RotateParallelograms();
    }

    private void UpdateThetaValues()
    {
        thetas[0] = sliders[0].value;
        thetas[1] = sliders[1].value;
        thetas[2] = sliders[2].value;
    }

    private void SetupUIValues()
    {
        for(int i=0; i<NO_OF_LEGS; i++)
            this.texts[i].text = (Math.Round(thetas[i] * 100.0f) / 100.0f).ToString();
    }

    /*
    private void SetupLineRenderer()
    {
        line = GetComponent<LineRenderer>();
        line.startWidth = 0.05f;
        line.endWidth = 0.05f;

        simulation = new DrawingSimulation(sliders, line, endEffector);
    }*/

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
