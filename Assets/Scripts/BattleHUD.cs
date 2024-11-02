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
        nameText.text = unit.goblinData.gName;
        levelText.text = "Lvl " + unit.goblinData.gLevel;
        hpSlider.maxValue = unit.goblinData.currentHP;
        hpSlider.value = unit.goblinData.currentHP;
    }

    public void setHP(int hp)
    {
        hpSlider.value = hp;
    }
}
