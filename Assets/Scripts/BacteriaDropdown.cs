using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BacteriaDropdown : MonoBehaviour
{
    public Dropdown dropdown;
    public TextAsset dataFile;
    public ResearchTestDropDown testDD;

    private static string selectedMO = "";

    void Start()
    {

        SetUp.instance.StartSetUp(dataFile);


        // Generate list of available Locales
        var options = new List<Dropdown.OptionData>();
        int selected = 0;
        for (int i = 0; i < SetUp.instance.microorgansims.Count; ++i)
        {
            var mo = SetUp.instance.microorgansims[i];
            if (selectedMO == mo || selectedMO == "")
            {
                selected = i;
                selectedMO = mo;
                MOSelected(i);
            }
                
            Dropdown.OptionData data = new Dropdown.OptionData(mo);
            options.Add(data);
        }
        dropdown.options = options;

        dropdown.value = selected;
        dropdown.onValueChanged.AddListener(MOSelected);

        testDD.SetUpDD(SetUp.instance.simulations[selectedMO]);
    }

     void MOSelected(int index)
    {
        selectedMO = SetUp.instance.microorgansims[index];

        testDD.SetUpDD(SetUp.instance.simulations[selectedMO]);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}


