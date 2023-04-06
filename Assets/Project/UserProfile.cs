using PlayFab.ClientModels;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SocialPlatforms;
using UnityEngine.SocialPlatforms.Impl;

public class UserProfile : MonoBehaviour
{
    public static UserProfile Instance;

    public static UnityEvent<ProfileData> OnProfileDataUpdated = new UnityEvent<ProfileData>();
    public static UnityEvent<CurrencyData> OnCurrencyDataUpdated = new UnityEvent<CurrencyData>();
    public static UnityEvent<List<PlayerLeaderboardEntry>> OnLeaderboardHighscoreUpdated = new UnityEvent<List<PlayerLeaderboardEntry>>();

    [Header("Data")]
    [SerializeField] ProfileData _profileData;
    [SerializeField] List<PlayerLeaderboardEntry> leaderboardHighscore = new List<PlayerLeaderboardEntry>();
    [SerializeField] CurrencyData _currencyData;
    public int highscore = 0;

    [Header("Settings")]
    public float levelCap = 1000;    

    void Awake()
    {
        Instance = this; 
    }


    private void OnEnable()
    {
        UserAccountManager.OnSinInSuccess.AddListener(SingIn);
        UserAccountManager.OnUserDataRetrieved.AddListener(UsedDataRetrieved);
        UserAccountManager.OnLeaderboardRetrieved.AddListener(LeaderboardRetrieved);
        UserAccountManager.OnStatisticRetrieved.AddListener(StatisticRetrieved);
    }
    
    private void OnDisable()
    {
        UserAccountManager.OnSinInSuccess.RemoveListener(SingIn);
        UserAccountManager.OnUserDataRetrieved.RemoveListener(UsedDataRetrieved);
        UserAccountManager.OnLeaderboardRetrieved.RemoveListener(LeaderboardRetrieved);
        UserAccountManager.OnStatisticRetrieved.RemoveListener(StatisticRetrieved);
    }
    

    void SingIn()
    {
        GetUserData();
        GetHighscoreStatistic();
        GetLeaderboardHighscore();
    }

    void UserDataRetrieved(string key, string value)
    {
        if (key == "ProfileData")
        {
            if (value != null)
            {
                _profileData = JsonUtility.FromJson<ProfileData>(value);
            }
            else
            {
                _profileData = new ProfileData();
            }
            _profileData._playerName = UserAccountManager.userAccountInfo.TitleInfo.DisplayName;

            OnProfileDataUpdated.Invoke(_profileData);
        }
    }

    [ContextMenu("Get UserData")]
    void GetUserData()
    {
        UserAccountManager.Instance.GetUserData("ProfileData");
    }

    [ContextMenu("Set UserData")]
    void SetUserData(UnityAction OnSuccess = null)
    {
        UserAccountManager.Instance.SetUserData("ProfileData", JsonUtility.ToJson(_profileData));
        OnProfileDataUpdated.Invoke(_profileData);
    }

    void GetHighscoreStatistic()
    {
        UserAccountManager.Instance.GetStatistic("Highscore");
    }

    void StatisticRetrieved(string statistic, int Value)
    {
        if (statistic == "Highscore")
        {
            highscore = Value;
        }
    }

    void UsedDataRetrieved(string key, string value)
    {
        if (key == "ProfileData")
        {
            _profileData = JsonUtility.FromJson<ProfileData>(value);
            OnProfileDataUpdated.Invoke(_profileData);
        }
    }

    [ContextMenu("Update Display Name")]
    public void UpdateDisplayName()
    {
        UserAccountManager.Instance.SetDisplayName(_profileData._playerName);
    }


    public void SetPlayerName(string playerName)
    {
        _profileData._playerName = playerName;
        
    }

    [ContextMenu("Add Random XP")]
    public void AddRandomXp() 
    {
        _profileData._level += UnityEngine.Random.Range(0f, 1f);
        _profileData._xp = (int)((_profileData._level - (MathF.Floor(_profileData._level))) * levelCap);

        SetUserData();
    }


    [ContextMenu("Get Highscore Leaderboard")]
    void GetLeaderboardHighscore()
    {
        UserAccountManager.Instance.GetLeaderboard("Highscore");
    }


    [ContextMenu("Increase Highscore")]
    void IncreaseHighscore()
    {
        highscore += 1;
        UserAccountManager.Instance.SetStatistic("Highscore", highscore);
    }

    void LeaderboardRetrieved(string statistic, List<PlayerLeaderboardEntry> leaderboard)
    {
        if (statistic == "Highscore")
        {
            leaderboardHighscore = leaderboard;
            OnLeaderboardHighscoreUpdated.Invoke(leaderboard);
        };
    }

}

[System.Serializable]
public class ProfileData
{
    public string _playerName;
    public float _level;
    public int _xp;
    public int _health;
}

public class CurrencyData
{
    public int Diamonds;
    public int Gold;
    public int Silver;

}