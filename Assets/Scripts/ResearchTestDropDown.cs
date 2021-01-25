using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResearchTestDropDown : MonoBehaviour
{
    public Dropdown dropdown;

    private static string selectedTest = "";
    private static ResearchSimulation currentSimulation;

    public void SetUpDD(ResearchSimulation simulation)
    {
        currentSimulation = simulation;
        selectedTest = "";

        // Generate list of available Locales
        var options = new List<Dropdown.OptionData>();
        int selected = 0;
        for (int i = 0; i < simulation.GetTests().Count; ++i)
        {
            var test = simulation.GetTests()[i];
            if (selectedTest == test || selectedTest == "")
            {
                selected = i;
                selectedTest = test;
                MOSelected(i);
            }
                
            Dropdown.OptionData data = new Dropdown.OptionData(test);
            options.Add(data);
        }
        dropdown.options = options;

        dropdown.value = selected;
        dropdown.onValueChanged.AddListener(MOSelected);
    }

    static void MOSelected(int index)
    {
        selectedTest = currentSimulation.GetTests()[index];

        // Start Simulation
        FindObjectOfType<Simulation>().SetUpSimulation(currentSimulation, selectedTest);
    }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}


