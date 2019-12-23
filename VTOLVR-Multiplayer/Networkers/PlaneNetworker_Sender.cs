﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class PlaneNetworker_Sender : MonoBehaviour
{
    public ulong networkUID;

    //Classes we use to find the information out
    private FlightInfo flightInfo;
    private WheelsController wheelsController;
    private AeroController aeroController;
    private TiltController tiltController;
    private VRThrottle vRThrottle;

    private Message_PlaneUpdate lastMessage;

    private void Awake()
    {
        lastMessage = new Message_PlaneUpdate(false, 0, 0, 0, 0, 0, 0, false, false, networkUID);

        flightInfo = GetComponent<FlightInfo>();
        wheelsController = GetComponent<WheelsController>();
        aeroController = GetComponent<AeroController>();
        tiltController = GetComponent<TiltController>();
        vRThrottle = gameObject.GetComponentInChildren<VRThrottle>();
        if (vRThrottle == null)
            Debug.Log("Cound't find throttle");
        else
            vRThrottle.OnSetThrottle.AddListener(SetThrottle);
    }

    private void LateUpdate()
    {
        lastMessage.flaps = aeroController.flaps;
        lastMessage.pitch = aeroController.input.x;
        lastMessage.yaw = aeroController.input.y;
        lastMessage.roll = aeroController.input.z;
        lastMessage.breaks = aeroController.brake;
        lastMessage.landingGear = LandingGearState();
        lastMessage.networkUID = networkUID;

        if (Networker.isHost)
            Networker.SendGlobalP2P(lastMessage, Steamworks.EP2PSend.k_EP2PSendUnreliable);
        else
            Networker.SendP2P(Networker.hostID, lastMessage, Steamworks.EP2PSend.k_EP2PSendUnreliable);
    }

    private bool LandingGearState()
    {
        return wheelsController.gearAnimator.GetCurrentState() == GearAnimator.GearStates.Extended;
    }

    public void SetThrottle(float t)
    {
        lastMessage.throttle = t;
    }
}