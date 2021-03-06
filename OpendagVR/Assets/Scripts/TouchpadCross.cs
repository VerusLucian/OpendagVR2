﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
[RequireComponent(typeof(SteamVR_TrackedObject))]

public class TouchpadCross : MonoBehaviour {

    SteamVR_TrackedObject trackedObj;
    SteamVR_Controller.Device device;
    bool pressed;
    public GameObject bowPrefab;

    public bool teleportEnabled = false;
    public bool grabEnabled = true;
    public bool buyEnabled = true;

    GameObject cameraRig;
    GameObject bow;

    GameObject controllerLeft;
    GameObject controllerRight;

    SteamVR_TrackedController buttonsLeft;
    SteamVR_TrackedController buttonsRight;
    

    void Awake()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
        SetupGameObjects();
        ChangeToTeleporting();
    }

    int j = 0;
    // Use this for initialization
    void FixedUpdate()
    {
        SetupGameObjects();
        if(controllerRight != null)
        {
            if (j == 0)
            {
                ChangeToTeleporting();
                j++;
            }
        }
        device = SteamVR_Controller.Input((int)trackedObj.index);
    }

    // Update is called once per frame
    void Update()
    {
        if(pressed == false)
        {
            if (device.GetPressDown(SteamVR_Controller.ButtonMask.Touchpad))
            {
                pressed = true;
                Vector2 touchpad = (device.GetAxis(EVRButtonId.k_EButton_Axis0));

                if (touchpad.y > 0.7f)
                {
                    Debug.Log("Moving Up");

                    try
                    {
                        if (GameObject.Find("IntroWave").activeSelf || GameObject.Find("IntroWave") != null)
                            GameObject.Find("IntroWave").GetComponent<introWave>().ExternalInput("TeleporterMode");
                    }
                    catch (Exception e)
                    {
                        Debug.Log(e);
                    }



                    ChangeToBuy();
                }

                else if (touchpad.y < -0.7f)
                {
                    Debug.Log("Moving Down");
                }

                if (touchpad.x > 0.7f)
                {
                    if (!controllerRight.GetComponent<Teleportation>().enabled)
                    {
                        ChangeToTeleporting();
                    }
                }

                else if (touchpad.x < -0.7f)
                {
                    if (!controllerRight.GetComponent<RWVR_InteractionController>().enabled)
                    {
                        ChangeToGrab();
                    }
                }

            }
        }
        else
        {
            if(device.GetPressUp(SteamVR_Controller.ButtonMask.Touchpad))
                pressed = false;
        }
    }

    public void ChangeToTeleporting()
    {

        if (grabEnabled)
        {
            DisableGrab();
        }

        if (buyEnabled)
        {
            DisableBuy();
        }

        teleportEnabled = true;
        controllerRight.GetComponent<SteamVR_LaserPointer>().enabled = true;
        if (controllerRight.transform.Find("New Game Object") != null)
            controllerRight.transform.Find("New Game Object").gameObject.SetActive(true);
        controllerRight.GetComponent<Teleportation>().enabled = true;

        try
        {
            if (GameObject.Find("IntroWave").activeSelf || GameObject.Find("IntroWave") != null)
                GameObject.Find("IntroWave").GetComponent<introWave>().ExternalInput("TeleportMode");
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }

 
    }

    void ChangeToGrab()
    {
        if(teleportEnabled)
        {
            DisableTeleport();
        }

        if (buyEnabled)
        {
            DisableBuy();
        }

        grabEnabled = true;
        controllerRight.transform.Find("Origin").gameObject.SetActive(true);
        controllerLeft.transform.Find("Origin").gameObject.SetActive(true);
        controllerRight.GetComponent<RWVR_InteractionController>().enabled = true;
        controllerLeft.GetComponent<RWVR_InteractionController>().enabled = true;
        SpawnBow();

    }

    void ChangeToBuy()
    {
        if (teleportEnabled)
        {
            DisableTeleport();
        }

        if (grabEnabled)
        {
            DisableGrab();
        }

        buyEnabled = true;
        controllerRight.GetComponent<SteamVR_LaserPointer>().enabled = true;
        controllerRight.transform.Find("New Game Object").gameObject.SetActive(true);
        cameraRig.GetComponent<BuildTower>().enabled = true;
        cameraRig.GetComponent<UpgradeTower>().enabled = true;
    }

    void DisableTeleport()
    {
        teleportEnabled = false;
        controllerRight.GetComponent<SteamVR_LaserPointer>().enabled = false;
        if (controllerRight.transform.Find("New Game Object") != null)
            controllerRight.transform.Find("New Game Object").gameObject.SetActive(false);
        controllerRight.GetComponent<Teleportation>().enabled = false;
    }

    void DisableGrab()
    {
        grabEnabled = false;
        controllerRight.GetComponent<RWVR_InteractionController>().enabled = false;
        controllerLeft.GetComponent<RWVR_InteractionController>().enabled = false;
        controllerRight.transform.Find("Origin").gameObject.SetActive(false);
        controllerLeft.transform.Find("Origin").gameObject.SetActive(false);
        RemoveBow();
    }

    void DisableBuy()
    {
        buyEnabled = false;
        controllerRight.GetComponent<SteamVR_LaserPointer>().enabled = false;
        if(controllerRight.transform.Find("New Game Object") != null)
            controllerRight.transform.Find("New Game Object").gameObject.SetActive(false);
        cameraRig.GetComponent<BuildTower>().enabled = false;
        cameraRig.GetComponent<UpgradeTower>().enabled = false;
    }

    void RemoveBow()
    {
        if (bow != null)
            GameObject.DestroyObject(bow);
    }

    void SpawnBow()
    {
        RemoveBow();
        bow = bowPrefab;
        bow = GameObject.Instantiate(bow, new Vector3(cameraRig.transform.position.x, cameraRig.transform.position.y + 1f, cameraRig.transform.position.z), Quaternion.identity);
        bow.GetComponent<Rigidbody>().isKinematic = true;

        // say to the introwave script that the bow has been spawned (only 1st time)
        try
        {
            if (GameObject.Find("IntroWave").activeSelf || GameObject.Find("IntroWave") != null)
                GameObject.Find("IntroWave").GetComponent<introWave>().ExternalInput("BowHasBeenSpawned");
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }


    }

    void SetupGameObjects()
    {
        if (cameraRig == null)
            cameraRig = GameObject.Find("[CameraRig]");

        if (controllerLeft == null)
            controllerLeft = cameraRig.transform.Find("Controller (left)").gameObject;

        if (controllerRight == null)
            controllerRight = cameraRig.transform.Find("Controller (right)").gameObject;
    }
}
