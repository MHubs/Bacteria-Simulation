using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetUp
{

    public static SetUp instance = new SetUp();

    public Dictionary<string, ResearchSimulation> simulations = new Dictionary<string, ResearchSimulation>();

    public List<string> microorgansims = new List<string>();

    public void StartSetUp(TextAsset asset)
    {

        string json = asset.text;

        JSONObject obj = new JSONObject(json);

        if (obj.type == JSONObject.Type.OBJECT)
        {

            JSONObject dataArray = obj["Data"];

            if (dataArray.type == JSONObject.Type.ARRAY)
            {
                foreach (JSONObject organismSimulation in dataArray.list)
                {

                    if (organismSimulation.type == JSONObject.Type.OBJECT)
                    {

                        string orgName = organismSimulation["name"].str;

                        Debug.Log("Starting " + orgName);

                        JSONObject orgTests = organismSimulation["tests"];

                        Dictionary<string, ResearchTest> testDictionary = new Dictionary<string, ResearchTest>();

                        if (orgTests.type == JSONObject.Type.ARRAY)
                        {
                            foreach (JSONObject orgTest in orgTests.list)
                            {



                                string testName = orgTest["name"].str;

                                string comment = orgTest["comment"].str;
                                string testType = orgTest["type"].str;
                                float testControl = orgTest["control"].f;

                                JSONObject intervals = orgTest["intervals"];

                                List<ResearchInterval> researchIntervals = new List<ResearchInterval>();

                                if (intervals.type == JSONObject.Type.ARRAY)
                                {

                                    foreach (JSONObject interval in intervals.list)
                                    {

                                        float time = interval["time"].f;
                                        float amt = interval["amount"].f;
                                        string unit = interval["units"].str;

                                        bool hasCC = false;
                                        float cc = 0;

                                        if (interval["control"] != null && interval["control"].type != JSONObject.Type.NULL)
                                        {
                                            hasCC = true;
                                            cc = interval["control"].f;
                                        }

                                        // Create Research Interval
                                        ResearchInterval intvl = new ResearchInterval(amt, time, hasCC, cc, unit);
                                        researchIntervals.Add(intvl);
                                    }

                                }
                                // Create Research Test
                                ResearchTest researchTest = new ResearchTest(testName, comment, testType, testControl, researchIntervals);

                                testDictionary.Add(testName, researchTest);
                            }
                        }

                        // Create Research Simulation
                        ResearchSimulation researchSimulation = new ResearchSimulation(orgName, testDictionary);
                        simulations.Add(orgName, researchSimulation);
                        microorgansims.Add(orgName);
                    }

                }
            }


        }

        Debug.Log("Loaded " + simulations.Count + " Simulations");


    }
}
