using System;
using System.Collections;
using System.Collections.Generic;
using Engine.Inputs;
using Engine.Support;
using UnityEngine;

public class GameInputerGetter : IInputsGetter
{
    public float GetAxis(string axisName)
    {
        return ETCInput.GetAxis(axisName);
    }

    public bool GetButtonDown(string buttonName)
    {
        return ETCInput.GetButtonDown(buttonName);
    }
}
