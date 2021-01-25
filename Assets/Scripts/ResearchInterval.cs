using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResearchInterval
{

    private float amount;
    private float timeSinceStart;
    private string units;
    private float currentControl;
    private bool hasCurrentControl;


    public ResearchInterval(float amt, float currTime, bool cc, float curr, string units)
    {
        amount = amt;
        timeSinceStart = currTime;
        this.units = units;
        hasCurrentControl = cc;
        currentControl = curr;
    }

    public float GetAmount()
    {
        return amount;
    }

    public float GetTimeSinceSimStart()
    {
        return timeSinceStart;
    }

    public string GetUnits()
    {
        return units;
    }

    public bool HasCurrentControl()
    {
        return hasCurrentControl;
    }

    public float GetCurrentControl()
    {
        return currentControl;
    }

}
