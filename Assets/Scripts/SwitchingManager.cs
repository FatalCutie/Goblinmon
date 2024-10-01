using System.Collections.Generic;
using System;
using UnityEngine;
using TMPro;

public class SwitchingManager : MonoBehaviour
{
    public List<SOGoblinmon> goblinmon;
    public GameObject unitButtonHolder;

    void Awake()
    {
        PopulateUnits(); //I'm pretty sure this is redundent
    }

    public void PopulateUnits()
    {
        int i = 0;
        try
        {
            foreach (Transform go in unitButtonHolder.transform)
            {
                TextMeshProUGUI unitNameText = go.GetChild(0).GetComponent<TextMeshProUGUI>();
                UnitButton ub = go.GetComponent<UnitButton>();

                //Init button
                Goblinmon gob = go.gameObject.AddComponent<Goblinmon>();
                gob.goblinData = goblinmon[i];
                gob.currentHP = gob.goblinData.maxHP; //This will need to be changed
                ub.unit = goblinmon[i];
                unitNameText.text = ub.unit.gName;

                i++;
            }
        }
        catch (ArgumentOutOfRangeException) { }

    }
}
