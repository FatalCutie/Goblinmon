using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneSwitcher : MonoBehaviour
{
    public ZoneManager zm;
    public ZoneManager.Area areaToSwitch = ZoneManager.Area.AREA_TOWN;

    void Start()
    {
        zm = FindObjectOfType<ZoneManager>();
    }

    public void SwitchToSelectedZone(){
        FindObjectOfType<ZoneManager>().SwitchArea(areaToSwitch);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player")){
            SwitchToSelectedZone();
        }
    }
}
