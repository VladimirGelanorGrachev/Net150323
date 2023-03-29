using PlayFab.ClientModels;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SocialPlatforms.Impl;

public class UserProfile : MonoBehaviour
{
    public static UserProfile instance;

    public static UnityEvent<ProfileData> OnProfileData = new UnityEvent<ProfileData>();
    public static UnityEvent<List<PlayerLeaderboardEntry>> OnLeaderboardLevelUpdate = new UnityEvent<List<PlayerLeaderboardEntry>>();

    [Header("Data")]
    [SerializeField] ProfileData _profileData;
    [SerializeField] List<PlayerLeaderboardEntry> leaderboardHighscore = new List<PlayerLeaderboardEntry>();
    public int highscore = 0;

    [Header("Settings")]
    public float levelCap = 1000;    

    void Awake()
    {
        instance = this; 
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
        GetLeaderboardLevel();
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
            //_profileData._playerName = UserAccountManager.userAccountInfo.TitleInfo.DisplayName;

            //OnProfileDataUpdated.Invoke(_profileData);
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
        //OnProfileDataUpdated.Invoke(_profileData);
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
            OnProfileData.Invoke(_profileData);
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
        SetUserData(GetUserData);
        UserAccountManager.Instance.SetDisplayName(playerName);
    }

    public void AddXp() 
    {
        _profileData._level += UnityEngine.Random.Range(0f, 1f);
        SetUserData(GetUserData);
        UserAccountManager.Instance.SetStatistic("Level",(int)(Mathf.FloorToInt(_profileData._level)));
    }
    
    void GetLeaderboardLevel()
    {
        UserAccountManager.Instance.GetLeaderboard("Level");
    }

    void LeaderboardRetrieved(string key, List<PlayerLeaderboardEntry> leaderboardEntries)
    {
        if (key == "Level")
        {
            OnLeaderboardLevelUpdate.Invoke(leaderboardEntries);
        };
    }

}

[System.Serializable]
public class ProfileData
{
    public string _playerName;
    public float _level;
    public int _xp;
}