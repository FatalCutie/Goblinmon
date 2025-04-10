using System;
using TMPro;
using UnityEngine;

//This can probably be merged into a different script but I don't wanna, so fuck off
public class ItemUIManager : MonoBehaviour
{
    public TextMeshProUGUI fusionItems;
    public TextMeshProUGUI captureItems;
    public PlayerPositionManager ppm;
    void Update()
    {
        if(!ppm) ppm = FindObjectOfType<PlayerPositionManager>();
        if(Int32.Parse(fusionItems.text) != ppm.fusionItems) fusionItems.text = $"{ppm.fusionItems}";
        if(Int32.Parse(captureItems.text) != ppm.captureItems) captureItems.text = $"{ppm.captureItems}";
    }
}
