using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/HelpEntry", order = 4)]
public class EncyclopediaSO : ScriptableObject{
    public string entryTitle;
    public string entryText;
    //Maybe add icon later
}
