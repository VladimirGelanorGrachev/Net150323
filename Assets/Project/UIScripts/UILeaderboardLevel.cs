using PlayFab.ClientModels;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UILeaderboardLevel : MonoBehaviour
{
    [SerializeField] PoolUILeaderboardEntry poolUILeaderboardEntry;

    List<UILeaderboardEntry> existingEntries = new List<UILeaderboardEntry>();

    void OnEnable()
    {
        UserProfile.OnLeaderboardHighscoreUpdated.AddListener(UILeaderboardLevelUpdated);
    }

    void OnDisable()
    {
        UserProfile.OnLeaderboardHighscoreUpdated.RemoveListener(UILeaderboardLevelUpdated);
    }

    void UILeaderboardLevelUpdated (List<PlayerLeaderboardEntry> leaderboardEntries)
    {
        if (existingEntries.Count > 0)
        {
            for (int i = existingEntries.Count - 1; i >= 0; i--) 
            {
                poolUILeaderboardEntry.ReturnToObjectPool(existingEntries[i]);
                
            }
        }
        existingEntries.Clear();

        for (var i = 0; i < leaderboardEntries.Count; i++)
        {
            UILeaderboardEntry entry = poolUILeaderboardEntry.GetFromObjectPool();
            entry.SetLeaderboardEntry(leaderboardEntries[i]);
            existingEntries.Add(entry);
        }
        
    }
}
