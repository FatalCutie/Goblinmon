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
    public GameObject fusionIcon;

    public void SetHUD(Goblinmon unit)
    {
        nameText.text = unit.goblinData.gName;
        levelText.text = "Lvl " + unit.goblinData.gLevel;
        hpSlider.maxValue = unit.goblinData.maxHP;
        hpSlider.value = unit.currentHP;
        healthText.text = $"{unit.currentHP}/{unit.goblinData.maxHP}";
        if (unit.goblinData.isFusion) fusionIcon.SetActive(true);
        else fusionIcon.SetActive(false);
    }

    public void setHP(int hp, Goblinmon victim)
    {
        // healthText.text = $"{victim.currentHP}/{victim.goblinData.maxHP}";
        // hpSlider.value = hp;
        StartCoroutine(AnimateHealthBar(hp, victim));
    }

    private IEnumerator AnimateHealthBar(int targetHP, Goblinmon victim)
    {
        float duration = 0.5f; // How long the animation should take
        float elapsed = 0f;
        float startHP = hpSlider.value;

        healthText.text = $"{victim.currentHP}/{victim.goblinData.maxHP}";

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            hpSlider.value = Mathf.Lerp(startHP, targetHP, t);
            yield return null;
        }

        hpSlider.value = targetHP; // Snap to final value just in case
    }
}
