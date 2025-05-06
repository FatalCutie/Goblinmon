using TMPro;
using UnityEngine;

public class EncyclopediaButton : MonoBehaviour
{
    public EncyclopediaSO so;
    public TextMeshProUGUI entryText;
    public TextMeshProUGUI buttonText;
    public GameObject typeChart;

    void Awake()
    {
        UpdateEntryTitle();
        if(typeChart.activeSelf) typeChart.SetActive(false);
    }

    public void UpdateEntry(){
        if(typeChart.activeSelf) typeChart.SetActive(false);
        entryText.text = so.entryText;
        FindObjectOfType<AudioManager>().Play("press");
    }

    public void TypeEntry(){
        typeChart.SetActive(true);
        entryText.text = so.entryText;
        FindObjectOfType<AudioManager>().Play("press");
    }

    public void UpdateEntryTitle(){
        buttonText.text = so.entryTitle;
    }
}
