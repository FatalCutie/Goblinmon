using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor.SearchService;
using UnityEngine.SceneManagement;
public class PartyStorage : MonoBehaviour
{
    public PartyStorage instance;
    [SerializeField] public List<SOGoblinmon> goblinmonSO;
    public List<Goblinmon> goblinmon;
    [SerializeField] private bool hasGottenGoblinmon = true;
    void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            //Destroy(gameObject);
            //Override previous instance, because older session should have more up to date info
            Destroy(instance);
            instance = this;
            return;
        }

        DontDestroyOnLoad(gameObject);
        InitializeGoblinmonParty();
    }

    //If goblinmon list is not populated then populate it with info from goblinmonSO
    void InitializeGoblinmonParty()
    {
        //Check if list is empty before running
        if (goblinmon.Count == 0 && hasGottenGoblinmon) //Goblinmon
        {
            try
            {
                for (int i = 0; i < 6; i++) //party maximum is 6 units
                {
                    if (goblinmonSO[i] != null) //if a SO exists
                    {
                        //Create a Goblinmon script on the Unit Button to hold data
                        //TODO instantiate goblinmon on different object then clear it later like switching manager
                        Goblinmon gob = gameObject.AddComponent<Goblinmon>();
                        goblinmon.Add(gob);
                        gob.goblinData = goblinmonSO[i];
                        gob.currentHP = goblinmonSO[i].maxHP; //set Goblinmon to max health at init. I don't think this will cause problems later down the line :clueless:
                        gob.friendOrFoe = Goblinmon.FriendOrFoe.FRIEND; //So unit faces the correct direction in battle
                        //goblinmon[i] = gob; //is this redundant?
                        //Destroy(gameObject.GetComponent<Goblinmon>());
                    }
                }
                //TODO: Destroy list
            }
            catch (ArgumentOutOfRangeException) { }


        }

    }

    //Update goblinmon party after a battle
    void UpdateGoblinmonParty()
    {
        //TODO: Take Goblinmon from switching menu and override them in goblinmon

    }
}
