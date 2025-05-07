using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class ZoneManager : MonoBehaviour
{
    public enum Area { AREA_TOWN, AREA_CENTER, AREA_SNOW, AREA_MAZE, AREA_WATER, AREA_FIRE, AREA_FINAL, AREA_CAVE };
    public Area area = Area.AREA_TOWN;
    public Area oldArea = Area.AREA_TOWN;
    private bool displaying = false;
    public Animator zoneAnimator;
    public TextMeshProUGUI areaText;

    public IEnumerator DisplayAreaSwap(){
        if(!displaying){ //Avoid stacking if rapidly changing area
            displaying = !displaying; //lock
            zoneAnimator.SetBool("PanelOpen", true);
            yield return new WaitForSeconds(3);
            zoneAnimator.SetBool("PanelOpen", false);
            yield return new WaitForSeconds(0.66f); //roughly forty frames
            displaying = !displaying; //unlock
        }
    }

    public void SwitchArea(Area newArea){
        if(newArea == area) return;
        //Update Area
        oldArea = area;
        area = newArea;
        switch(newArea){
            case Area.AREA_TOWN:
                if(!displaying) areaText.text = "Town";
                area = Area.AREA_TOWN;
                break;
            case Area.AREA_CENTER:
                if(!displaying) areaText.text = "Town Square";
                area = Area.AREA_CENTER;
                break;
            case Area.AREA_SNOW:
                if(!displaying) areaText.text = "Snowpoint";
                area = Area.AREA_SNOW;
                break;
            case Area.AREA_MAZE:
                if(!displaying) areaText.text = "Hedge Maze";
                area = Area.AREA_MAZE;
                break;
            case Area.AREA_WATER:
                if(!displaying) areaText.text = "Tidepool";
                area = Area.AREA_WATER;
                break;
            case Area.AREA_FIRE:
                if(!displaying) areaText.text = "Hotbed";
                area = Area.AREA_FIRE;
                break;
            case Area.AREA_FINAL:
                if(!displaying) areaText.text = "Arena";
                area = Area.AREA_FINAL;
                break;
            case Area.AREA_CAVE:
                if (!displaying) areaText.text = "Fusion Cave";
                area = Area.AREA_CAVE;
                break;
        }
        SwitchAreaMusic();
        StartCoroutine(DisplayAreaSwap());
    }

    public void SwitchAreaMusic(){
        StopAreaMusic();
        switch(area){
            case Area.AREA_TOWN:
                if(oldArea == Area.AREA_CENTER) return;
                FindObjectOfType<AudioManager>().Play("townMusic");
                break;
            case Area.AREA_CENTER:
                if(oldArea == Area.AREA_TOWN) return;
                FindObjectOfType<AudioManager>().Play("townMusic");
                break;
            case Area.AREA_SNOW:
                if (oldArea == Area.AREA_MAZE) return;
                FindObjectOfType<AudioManager>().Play("snowMazeMusic");
                break;
            case Area.AREA_MAZE:
                if (oldArea == Area.AREA_SNOW) return;
                FindObjectOfType<AudioManager>().Play("snowMazeMusic");
                break;
            case Area.AREA_WATER:
                if (oldArea == Area.AREA_FIRE) return;
                FindObjectOfType<AudioManager>().Play("fireWaterMusic");
                break;
            case Area.AREA_FIRE:
                if (oldArea == Area.AREA_WATER) return;
                FindObjectOfType<AudioManager>().Play("fireWaterMusic");
                break;
            case Area.AREA_FINAL:

                break;
            case Area.AREA_CAVE:
                FindObjectOfType<AudioManager>().Play("caveMusic");
                break;
        }
    }

    public void StopAreaMusic(){
        switch(oldArea){
            case Area.AREA_TOWN:
                if (area == Area.AREA_CENTER) return;
                FindObjectOfType<AudioManager>().Stop("townMusic");
                break;
            case Area.AREA_CENTER:
                if (area == Area.AREA_TOWN) return;
                FindObjectOfType<AudioManager>().Stop("townMusic");
                break;
            case Area.AREA_SNOW:
                if (area == Area.AREA_MAZE) return;
                FindObjectOfType<AudioManager>().Stop("snowMazeMusic");
                break;
            case Area.AREA_MAZE:
                if (area == Area.AREA_SNOW) return;
                FindObjectOfType<AudioManager>().Stop("snowMazeMusic");
                break;
            case Area.AREA_WATER:
                if (area == Area.AREA_FIRE) return;
                FindObjectOfType<AudioManager>().Stop("fireWaterMusic");
                break;
            case Area.AREA_FIRE:
                if (area == Area.AREA_WATER) return;
                FindObjectOfType<AudioManager>().Stop("fireWaterMusic");
                break;
            case Area.AREA_FINAL:

                break;
            case Area.AREA_CAVE:
                FindObjectOfType<AudioManager>().Stop("caveMusic");
                break;
        }
    }

    public void StopAllAreaMusic()
    {
        switch (area)
        {
            case Area.AREA_TOWN:
                FindObjectOfType<AudioManager>().Stop("townMusic");
                break;
            case Area.AREA_CENTER:
                FindObjectOfType<AudioManager>().Stop("townMusic");
                break;
            case Area.AREA_SNOW:
                FindObjectOfType<AudioManager>().Stop("snowMazeMusic");
                break;
            case Area.AREA_MAZE:
                FindObjectOfType<AudioManager>().Stop("snowMazeMusic");
                break;
            case Area.AREA_WATER:
                FindObjectOfType<AudioManager>().Stop("fireWaterMusic");
                break;
            case Area.AREA_FIRE:
                FindObjectOfType<AudioManager>().Stop("fireWaterMusic");
                break;
            case Area.AREA_FINAL:

                break;
            case Area.AREA_CAVE:
                FindObjectOfType<AudioManager>().Stop("caveMusic");
                break;
        }
    }
}
