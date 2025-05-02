using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Move", order = 1)]
public static class NPCTracker
{
    private static HashSet<string> defeatedNPCIds = new HashSet<string>();

    public static void MarkDefeated(string npcId)
    {
        defeatedNPCIds.Add(npcId);
    }

    public static bool IsDefeated(string npcId)
    {
        return defeatedNPCIds.Contains(npcId);
    }
}
