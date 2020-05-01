using Assets.Scripts.StewartPlatform.init;
using Assets.Scripts.StewartPlatform.logic;
using Assets.Scripts.StewartPlatform.rotation;
using Assets.Scripts.StewartPlatform.utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StewartPlatformIK : MonoBehaviour
{
    [Header("End effector")]
    [SerializeField] Transform endEffector = null;                                  // end effector game object = top platform = mobile platform

    [Header("Base")]
    [SerializeField] Transform baseEffector = null;                                 // base effector game object = bottom platform = fixed platform

    [Header("Bottom Legs GO")]
    [SerializeField] Transform[] bottomLegs = null;                                 // lower part of each leg -> game objects

    [Header("Top Legs GO")]
    [SerializeField] Transform[] topLegs = null;                                    // fixed game objects on the mobile platform

    [Header("Final legs GO")]
    [SerializeField] Transform[] finalLegs = null;                                  // upper part of each leg -> game objects

    [Header("Children GO")]
    [SerializeField] Transform[] children = null;                                   // transition parts for each leg

    [Header("UI")]
    [SerializeField] Slider[] sliders = null;                                       // slider for each dof -> 6 sliders

    [Header("Canvas")]
    [SerializeField] GameObject menu = null;                                        // canvas menu for activation / deactivation which controls the sliders

    private float phi_roll = 0.0f, theta_pitch = 0.0f, psi_yaw = 0.0f;              // initial values for rotation

    private ServiceInitializerIK serviceInitializer;                                // initialize the system variables, e.g. sliders
    private ServiceRotationIK serviceRotation;                                      // rotate each leg towards target
    private InverseKinematics inverseKinematics;                                    // do the logic for inverse kinematics algorithm

    void Start()
    {
        ShowMenu();
        Init();
    }

    void Update()
    {
        serviceRotation.RotateLegs();

        inverseKinematics.Init(topLegs, bottomLegs, finalLegs, children, endEffector, baseEffector);
        inverseKinematics.DoInverseKinematics(phi_roll, theta_pitch, psi_yaw);

        SetEndEffectorPosition(sliders[2].value, sliders[0].value, sliders[1].value, sliders[4].value, sliders[5].value, sliders[3].value);
    }

    private void ShowMenu()
    {
        menu.SetActive(true);
    }

    private void Init()
    {
        serviceInitializer = new ServiceInitializerIK(sliders);
        serviceInitializer.InitializeSliders();

        serviceRotation = ScriptableObject.CreateInstance<ServiceRotationIK>();
        serviceRotation.Init(topLegs, bottomLegs);

        inverseKinematics = ScriptableObject.CreateInstance<InverseKinematics>();
    }

    private void SetEndEffectorPosition(float xPos, float yPos, float zPos, float xRot, float yRot, float zRot)
    {
        endEffector.position = new Vector3(xPos, yPos, zPos);
        endEffector.rotation = Quaternion.Euler(xRot, yRot, zRot);
    }
}
