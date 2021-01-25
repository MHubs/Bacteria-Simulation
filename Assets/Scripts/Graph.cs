﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Graph : MonoBehaviour
{

    public float graphWidth;
    public float graphHeight;
    public bool useGradient = false;
    public Color gradientColor;

    private float minX;
    private float minY;
    private float maxX;
    private float maxY;

    LineRenderer newLineRenderer;
    int vertexAmount = 60;
    float xInterval;

    GameObject parentCanvas;
    private List<double> bestFitCoeffs;

    // Use this for initialization
    void Start()
    {
        parentCanvas = GameObject.Find("Canvas");
        graphWidth = this.gameObject.GetComponent<RectTransform>().rect.width;
        graphHeight = this.gameObject.GetComponent<RectTransform>().rect.height;
        newLineRenderer = this.gameObject.GetComponent<LineRenderer>();
        newLineRenderer.positionCount = vertexAmount;
        newLineRenderer.sortingOrder = 4;
        newLineRenderer.sortingLayerName = "UI";

        minX = -graphWidth / 2;
        maxX = graphWidth / 2;
        minY = -graphHeight / 2;
        maxY = graphHeight / 2;

        xInterval = graphWidth / vertexAmount;
        Debug.Log(newLineRenderer + " : " + xInterval);
    }

    //Display 1 minute of data or as much as there is.
    public void Draw(List<ResearchInterval> researchIntervals, List<double> coeffs, float start, float end, float max, float min)
    {
        if (researchIntervals.Count == 0)
            return;

        if (useGradient)
        {
            newLineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            Gradient gradient = new Gradient();
            GradientColorKey[] keys = new GradientColorKey[6];

            
            if (researchIntervals[0].GetTimeSinceSimStart() == start)
            {
                keys[0] = new GradientColorKey(gradientColor, 0.0f);
                keys[1] = new GradientColorKey(gradientColor, 0.0f);
            } else
            {
                keys[0] = new GradientColorKey(Color.red + gradientColor, 0.0f);
                keys[1] = new GradientColorKey(Color.red + gradientColor, (researchIntervals[0].GetTimeSinceSimStart() - 0.2f - start) / (end - start));
            }

            keys[2] = new GradientColorKey(gradientColor, (researchIntervals[0].GetTimeSinceSimStart() - start) / (end - start));
            keys[3] = new GradientColorKey(gradientColor, (researchIntervals[researchIntervals.Count - 1].GetTimeSinceSimStart() - start) / (end - start));

            if (researchIntervals[researchIntervals.Count - 1].GetTimeSinceSimStart() == end)
            {
                keys[4] = new GradientColorKey(gradientColor, 1f);
                keys[5] = new GradientColorKey(gradientColor, 1f);
            } else
            {
                keys[4] = new GradientColorKey(Color.red + gradientColor, (researchIntervals[researchIntervals.Count - 1].GetTimeSinceSimStart() + 0.2f - start) / (end - start));
                keys[5] = keys[0] = new GradientColorKey(Color.red + gradientColor, 1);
            } 
            

        
            gradient.SetKeys(
                keys,
                new GradientAlphaKey[] { new GradientAlphaKey(1, 0.0f), new GradientAlphaKey(1, 1.0f) }
            );
            newLineRenderer.colorGradient = gradient;
        }

        float x;
        int iter = 0;
        for (float i = start; i < end; i += (end-start) / vertexAmount)
        {
            if (iter >= vertexAmount)
            {
                break;
            }

            float percentX = (i - start) / (end - start);
            

            float y = (float)F(coeffs, i);

            float percentY = (y - min) / (max - min);



            //y *= (graphHeight / ((float)Mathf.Abs((float)absMax) - add));
            //x = iter * xInterval;



            newLineRenderer.SetPosition(iter, new Vector3((graphWidth * percentX) + minX, (graphHeight * percentY) + minY, -10));
            iter++;

        }
    }

    

    public double F(List<double> coeffs, double x)
    {
        double total = 0;
        double x_factor = 1;
        for (int i = 0; i < coeffs.Count; i++)
        {
            total += x_factor * coeffs[i];
            x_factor *= x;
        }
        return total;
    }
}