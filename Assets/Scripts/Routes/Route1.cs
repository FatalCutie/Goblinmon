using UnityEngine;

public class Route1 : RandomEncounter
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            GenerateRandomEncounter();
        }
    }

    public override void GenerateRandomEncounter()
    {
        int i = rnd.Next(0, 100);
        if(i <= 30)
            InitializeBattle(unitsOnRoute[0]); //Fire
        else if(i <= 90)
            InitializeBattle(unitsOnRoute[1]); //Water
        else
            InitializeBattle(unitsOnRoute[2]); //Grass
    }
}
