using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResearchSimulation
{

    private string organismName;
    private Dictionary<string, ResearchTest> researchIntervals = new Dictionary<string, ResearchTest>();
   

    public ResearchSimulation(string name, Dictionary<string, ResearchTest> intervals)
    {
        organismName = name;
        researchIntervals = intervals;
    }

    public string GetOrganism()
    {
        return organismName;
    }

    public List<string> GetTests()
    {
        List<string> tests = new List<string>();

        foreach (string key in researchIntervals.Keys)
        {
            tests.Add(key);
        }
        return tests;
    }

    public ResearchTest GetTest(string test)
    {
        if (researchIntervals.ContainsKey(test))
        {
            return researchIntervals[test];
        } else
        {
            return null;
        }
    }

}
