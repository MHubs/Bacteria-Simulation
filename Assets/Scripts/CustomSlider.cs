using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomSlider : MonoBehaviour
{

    public Slider slider;
    public GameObject backgroundPrefab;
    public GameObject fillArea;
    private List<GameObject> fillRects = new List<GameObject>();

    public void CreateFill(ResearchTest researchTest, float min, float max)
    {
        foreach (GameObject obj in fillRects)
        {
            Destroy(obj);
        }

        fillRects.Clear();

        // Main Background
        GameObject predictionBackground = Instantiate(backgroundPrefab);
        RectTransform predTransform = predictionBackground.GetComponent<RectTransform>();
        Image predImage = predictionBackground.GetComponent<Image>();
        predictionBackground.transform.SetParent(fillArea.transform, false);

        predImage.color = new Color(255/255,0,0);

        predTransform.Left(-5);
        predTransform.Right(-15);
        predTransform.Top(0);
        predTransform.Bottom(0);

        // Max = 90
        // Min = -200
        // (max - min) = 290

        // t = -100

        // (t - min) = 100
        // (t-min)/(max-min) = 100/290 = 0.34 = 34%

        foreach(ResearchInterval interval in researchTest.GetIntervals())
        {
            GameObject intervalBackground = Instantiate(backgroundPrefab);
            RectTransform intervalTransform = intervalBackground.GetComponent<RectTransform>();
            Image intervalImage = intervalBackground.GetComponent<Image>();
            intervalBackground.transform.SetParent(fillArea.transform, false);

            intervalImage.color = new Color(255/255, 255/255, 255/255);

            float percent = (interval.GetTimeSinceSimStart() - min) / (max - min);

            intervalTransform.Left(-5);
            intervalTransform.Right(-15);
            intervalTransform.Top(0);
            intervalTransform.Bottom(0);

            intervalTransform.anchorMin = new Vector2(percent, 0);
            intervalTransform.anchorMax = new Vector2(percent, 1);

            intervalTransform.SetAsFirstSibling();

            fillRects.Add(intervalBackground);
        }

        GameObject rangeBackground = Instantiate(backgroundPrefab);
        RectTransform rangeTransform = rangeBackground.GetComponent<RectTransform>();
        Image rangeImage = rangeBackground.GetComponent<Image>();
        rangeBackground.transform.SetParent(fillArea.transform, false);

        float minPercent = (researchTest.GetIntervals()[0].GetTimeSinceSimStart() - min) / (max - min);
        float maxPercent = (researchTest.GetIntervals()[researchTest.GetIntervals().Count - 1].GetTimeSinceSimStart() - min) / (max - min);

        rangeTransform.Left(-5);
        rangeTransform.Right(-15);
        rangeTransform.Top(0);
        rangeTransform.Bottom(0);

        rangeTransform.anchorMin = new Vector2(minPercent, 0);
        rangeTransform.anchorMax = new Vector2(maxPercent, 1);

        rangeImage.color = new Color(0, 255/255, 0);

        rangeBackground.transform.SetAsFirstSibling();

        predictionBackground.transform.SetAsFirstSibling();

        fillRects.Add(rangeBackground);
        fillRects.Add(predictionBackground);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

public static class RectTransformExtensions
{
    public static RectTransform Left(this RectTransform rt, float x)
    {
        rt.offsetMin = new Vector2(x, rt.offsetMin.y);
        return rt;
    }

    public static RectTransform Right(this RectTransform rt, float x)
    {
        rt.offsetMax = new Vector2(-x, rt.offsetMax.y);
        return rt;
    }

    public static RectTransform Bottom(this RectTransform rt, float y)
    {
        rt.offsetMin = new Vector2(rt.offsetMin.x, y);
        return rt;
    }

    public static RectTransform Top(this RectTransform rt, float y)
    {
        rt.offsetMax = new Vector2(rt.offsetMax.x, -y);
        return rt;
    }
}
