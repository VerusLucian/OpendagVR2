﻿//This Script is created by Patrick

using System;
using System.Collections;
using UnityEngine;

public class Teleportation : MonoBehaviour {
    [Header("Setup:")]
    public GameObject cameraRig;
    public GameObject animationCanvas;
    public GameObject StartingPosition;

    [Header("Automaticaly Setup:")]
    [SerializeField]
    private SteamVR_TrackedController buttons;
    [SerializeField]
    private SteamVR_LaserPointer pointer;
    [SerializeField]
    private GameObject CurrentTeleportPos;




    private void Awake()
    {
        CurrentTeleportPos = StartingPosition.transform.parent.gameObject;
        TeleportObject teleport = CurrentTeleportPos.GetComponentInChildren<TeleportObject>();
        teleport.Teleport(cameraRig.transform, null);
    }

    private void FixedUpdate()
    {
        while (buttons == null || pointer == null)
        {
            SetButtons();
            SetPointer();
        }

        Raycast();
        
    }

    void Raycast()
    {
        if(pointer != null)
        {
            RaycastHit hit;
            Physics.Raycast(pointer.pointer.transform.position, pointer.pointer.transform.forward, out hit);

            if (hit.collider != null)
            {
                if (hit.collider.tag == "TeleportZone")
                {
                    TeleportObject teleport = hit.collider.GetComponentInChildren<TeleportObject>();
                    if (buttons.triggerPressed)
                    {
                        try
                        {
                            if (GameObject.Find("IntroWave").activeSelf || GameObject.Find("IntroWave") != null)
                                GameObject.Find("IntroWave").GetComponent<introWave>().ExternalInput("Teleported");
                        }
                        catch (Exception e)
                        {
                            Debug.Log(e);
                        }
              
                        StartCoroutine(Teleport(hit, teleport));
                    }
                }
            }
        }
    }


    void SetButtons()
    {
        if (buttons == null)
        {
            buttons = GetComponent<SteamVR_TrackedController>();
        }
    }

    void SetPointer()
    {
        if (pointer == null)
        {
            pointer = GetComponent<SteamVR_LaserPointer>();
        }
    }

    IEnumerator Teleport(RaycastHit hit, TeleportObject teleport)
    {
        GameObject prev = CurrentTeleportPos;
        CurrentTeleportPos = hit.collider.gameObject;
        animationCanvas.GetComponentInChildren<Animator>().SetBool("Teleport", true);
        yield return new WaitForSeconds(0.3f);
        teleport.Teleport(cameraRig.transform, prev);
        animationCanvas.GetComponentInChildren<Animator>().SetBool("Teleport", false);
    }
}
