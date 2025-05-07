using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BiomeManager : MonoBehaviour
{
    public SpriteRenderer background;
    public List<Sprite> backgroundSprites;
    public SpriteRenderer playerBattleStation;
    public SpriteRenderer enemyBattleStation;
    public List<Sprite> battleStations;
    //Cave, Fire, Grass, Ice, Water

    public void SetUpBiome(){
        int i = CheckBiome();
        background.sprite = backgroundSprites[i];
        playerBattleStation.sprite = battleStations[i];
        enemyBattleStation.sprite = battleStations[i];
    }

    public int CheckBiome(){
        ZoneManager.Area place = FindObjectOfType<PlayerPositionManager>().area;
        switch(place){
            case ZoneManager.Area.AREA_SNOW:
                return 3;
            case ZoneManager.Area.AREA_MAZE:
                return 2;
            case ZoneManager.Area.AREA_WATER:
                return 4;
            case ZoneManager.Area.AREA_CAVE:
                return 0;
            case ZoneManager.Area.AREA_FIRE:
                return 1;
            case ZoneManager.Area.AREA_FINAL:
                return 0;
        }
        return 0;
    }
}
