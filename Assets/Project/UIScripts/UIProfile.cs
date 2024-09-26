using System;
using System.Collections;
using System.Collections.Generic;
using PlayFab.ClientModels;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

public class UIProfile : MonoBehaviour
{
    [SerializeField] CanvasGroup canvasGroup;

    [Header("Profile")]
    [SerializeField] InputField nameText;
    [SerializeField] Text levelText;
    [SerializeField] Text xpText;
    [SerializeField] Image xpFill;
    [SerializeField] Text playerHealth;

    [Header("currency")]
    [SerializeField] Text diamondsText;
    [SerializeField] Text silverText;
    [SerializeField] Text goldText;

    [Header("Highscore Leaderboard")]
    [SerializeField] PoolUILeaderboardEntry poolUILeaderboardEntry;
    List<UILeaderboardEntry> entries = new List<UILeaderboardEntry>();

    void OnEnable()
    {
        UserAccountManager.OnSinInSuccess.AddListener(SingIn);

        UserProfile.OnProfileDataUpdated.AddListener(ProfileDataUpdated);        
        UserProfile.OnLeaderboardHighscoreUpdated.AddListener(LeaderboardHighscoreUpdated);
        //CurrencyData.OnCurrencyDataUpdated.AddListner(СurrencyDataUpdated);
    }

    void OnDisable()
    {
        UserAccountManager.OnSinInSuccess.RemoveListener(SingIn);
        UserProfile.OnProfileDataUpdated.RemoveListener(ProfileDataUpdated);
        UserProfile.OnLeaderboardHighscoreUpdated.RemoveListener(LeaderboardHighscoreUpdated);
        //CurrencyData.OnCurrencyDataUpdated.RemoveListner(СurrencyDataUpdated);
    }

    void SingIn()
    {
        StartCoroutine(FadeIn());
    }

    IEnumerator FadeIn()
    {
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;
        canvasGroup.alpha = 0;
        while (canvasGroup.alpha < 1)
        {
            canvasGroup.alpha += Time.deltaTime;
            yield return null;
        }
        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;
    }

    void LeaderboardHighscoreUpdated(List<PlayerLeaderboardEntry> leaderboard)
    {
        if (entries.Count > 0)
        {
            for (var i = entries.Count - 1; i >= 0; i--)
            {
                poolUILeaderboardEntry.ReturnToObjectPool(entries[i]);
            }
        }
        entries.Clear();
        for (var i = 0; i < leaderboard.Count; i++)
        {
            UILeaderboardEntry entry = poolUILeaderboardEntry.GetFromObjectPool();
            entry.SetLeaderboardEntry(leaderboard[i]);
            entries.Add(entry);
            entry.transform.SetAsLastSibling();
            
            if (leaderboard[i].PlayFabId == UserAccountManager.playfabID)
            {
                if (leaderboard[i].StatValue != UserProfile.Instance.highscore) UserAccountManager.Instance.GetLeaderboardDelayed("Highscore");
            }
        }
    }
  

    void ProfileDataUpdated(ProfileData profileData)
    {
        nameText.text = profileData._playerName;
        levelText.text = (Mathf.Floor(profileData._level)).ToString();
        xpText.text = $"{profileData._xp} / {UserProfile.Instance.levelCap}";
        xpFill.fillAmount = ((float)profileData._xp) / (UserProfile.Instance.levelCap);
        playerHealth.text = (Mathf.Floor(profileData._health)).ToString();
    }

    void СurrencyDataUpdated(CurrencyData currensyData)
    {
        diamondsText.text = (Mathf.Floor(currensyData.Diamonds)).ToString();
        silverText.text = (Mathf.Floor(currensyData.Diamonds)).ToString();
        goldText.text = (Mathf.Floor(currensyData.Diamonds)).ToString();
    }

}
