using UnityEngine;

public class Cave : RandomEncounter
{
    public override void GenerateRandomEncounter()
    {
        int i = rnd.Next(0, 46);
        int rangeIndex = (i - 1) / 5; //This gives values from 0 to 8
        InitializeBattle(unitsOnRoute[rangeIndex]);
    }
}
