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
    public TextMeshProUGUI healthText;

    public void SetHUD(Goblinmon unit)
    {
        nameText.text = unit.goblinData.gName;
        levelText.text = "Lvl " + unit.goblinData.gLevel;
        hpSlider.maxValue = unit.goblinData.maxHP;
        hpSlider.value = unit.goblinData.currentHP;
        healthText.text = $"{unit.goblinData.currentHP}/{unit.goblinData.maxHP}";
    }

    public void setHP(int hp, Goblinmon victim)
    {
        healthText.text = $"{victim.goblinData.currentHP}/{victim.goblinData.maxHP}";
        hpSlider.value = hp;
    }
}
