using Assets.Scripts.StewartPlatform.init;
using Assets.Scripts.StewartPlatform.logic;
using Assets.Scripts.StewartPlatform.rotation;
using Assets.Scripts.StewartPlatform.utils;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StewartPlatformFK : MonoBehaviour
{
    [Header("End effector")]
    [SerializeField] Transform endEffector = null;                          // end effector game object = top platform = mobile platform

    [Header("Bottom Legs GO")]
    [SerializeField] Transform[] bottomLegs = null;                         // lower part of each leg -> game objects

    [Header("Top Legs GO")]
    [SerializeField] Transform[] topLegs = null;                            // fixed game objects on the mobile platform

    [Header("Children GO")]
    [SerializeField] Transform[] children = null;                           // transition parts for each leg

    [Header("Final legs GO")]
    [SerializeField] Transform[] finalLegs = null;                          // upper part of each leg -> game objects

    [Header("Final Legs for Children")]
    [SerializeField] Transform[] childrenFinal = null;                      // upper part for each transition leg

    [Header("UI")]
    [SerializeField] Slider[] sliders = null;                               // slider for each dof -> 6 sliders

    [Header("Canvas")]
    [SerializeField] GameObject menu = null;                                // canvas menu for activation / deactivation which controls the sliders

    private ServiceInitializerFK serviceInitializer;                        // initialize the system variables, e.g. sliders
    private StewartPlatformFKConverter serviceConverter;                    // service converter, e.g. radians to degrees
    private ServiceRotationFK serviceRotation;                              // rotate each leg towards target
    private ForwardKinematics forwardKinematics;                            // do the logic for forward kinematics algorithm

    void Start()
    {
        SetupMenu();
        Initialize();   
    }

    void Update()
    {
        forwardKinematics.DoForwardKinematics();
    }

    private void SetupMenu()
    {
        menu.SetActive(true);
    }

    private void Initialize()
    {
        serviceConverter = new StewartPlatformFKConverter();

        serviceInitializer = new ServiceInitializerFK(sliders);
        serviceInitializer.InitializeSliders();

        serviceRotation = ScriptableObject.CreateInstance<ServiceRotationFK>();
        serviceRotation.Init(topLegs, bottomLegs, finalLegs, children, sliders);
        serviceRotation.RotateLegs();

        forwardKinematics = ScriptableObject.CreateInstance<ForwardKinematics>();
        forwardKinematics.Init(serviceConverter, children, childrenFinal, bottomLegs, endEffector, sliders);
        forwardKinematics.SetupVariables();
    }
}
