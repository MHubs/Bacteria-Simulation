using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResearchTest
{

    private List<ResearchInterval> intervals = new List<ResearchInterval>();
    private string testName;
    private string comment;
    private string testType;
    private float control;
    
    public ResearchTest(string name, string comment, string type, float control, List<ResearchInterval> intervals) 
    {
        testName = name;
        this.comment = comment;
        this.testType = type;
        this.intervals = intervals;
        this.control = control;
    }

    public float GetControl()
    {
        return control;
    }

    public List<ResearchInterval> GetIntervals()
    {
        return intervals;
    }

    public string GetTestType()
    {
        return testType;
    }

    public string GetTestName()
    {
        return testName;
    }

    public string GetComment()
    {
        return comment;
    }

}
