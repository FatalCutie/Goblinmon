using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BattleHUD : MonoBehaviour
{

    public TextMeshProUGUI nameText;
    public TextMeshProUGUI levelText;
    public GameObject attackButtonStorage;
    public Slider hpSlider;

    public void SetHUD(Goblinmon unit)
    {
        Debug.Log(unit.gName);

        nameText.text = unit.gName;
        levelText.text = "Lvl " + unit.gLevel;
        hpSlider.maxValue = unit.maxHP;
        hpSlider.value = unit.currentHP;
    }

    public void SetPlayerMoves(Goblinmon unit)
    {
        int i = 0;
        foreach (Transform go in attackButtonStorage.transform) //this is also pretty dumb
        {
            TextMeshProUGUI moveNameText = go.GetChild(0).GetComponent<TextMeshProUGUI>();
            moveNameText.text = unit.moveset[i].moveName;
            i++;
        }
    }

    public void setHP(int hp)
    {
        hpSlider.value = hp;
    }
}
