using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine.Events;
using System.Collections.Generic;


public class UserAccountManager : MonoBehaviour
{
    public static UserAccountManager Instance;

    public static UnityEvent OnSinInSuccess = new UnityEvent();
    public static UnityEvent<string> OnSinInFailed = new UnityEvent<string>();
    public static UnityEvent<string> OnCreateAccountFailed = new UnityEvent<string>();
    public static UnityEvent<string, string> OnUserDataRetrieved = new UnityEvent<string, string> { };
    public static UnityEvent<string, int> OnStatisticRetrieved = new UnityEvent<string, int> { };
    public static UnityEvent<string, List<PlayerLeaderboardEntry>> OnLeaderboardRetrieved = new UnityEvent<string, List<PlayerLeaderboardEntry>> { };

    string playfabId;


    void Awake()
    {
        Instance = this;
    }


    public void CreationAccount (string username, string emailAddress, string password)
    {
        PlayFabClientAPI.RegisterPlayFabUser(
            new RegisterPlayFabUserRequest()
            {
                Email = emailAddress,
                Password = password,
                Username = username,
                RequireBothUsernameAndEmail = true
            },
            response => 
            {
                Debug.Log($"Successful Account Creation: {username}, {emailAddress}");
                SingIn(username, password);

            },
            error => 
            {
                Debug.Log($"Unsuccessful Account Creation: {username}, {emailAddress} \n {error.ErrorMessage}");
                OnCreateAccountFailed.Invoke(error.ErrorMessage);
            }
            ); 
    }


    public void SingIn(string username, string password)
    {
        PlayFabClientAPI.LoginWithPlayFab(new LoginWithPlayFabRequest()
        {
            Username = username,
            Password = password
        },
        response => { 
            Debug.Log($"Successful Account SingIn: {username}");
            OnSinInSuccess.Invoke();
            playfabId = response.PlayFabId;
        },
        error => { 
            Debug.Log($"Unsuccessful Account SingIn: {username} \n {error.ErrorMessage}");
            OnSinInFailed.Invoke(error.ErrorMessage);
        }
        );
    }

    void GetDeviceID(out string android_id, out string ios_id, out string custom_id)
    {
        android_id = string.Empty;
        ios_id = string.Empty;
        custom_id = string.Empty;

        if(Application.platform == RuntimePlatform.Android) 
        { 
            AndroidJavaClass up = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject currentActivity = up.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaObject contentResolver = currentActivity.Call<AndroidJavaObject>("getContentResolver");
            AndroidJavaClass secure = new AndroidJavaClass("android.provider.Settings$Secure");
            android_id = secure.CallStatic<string>("getString", contentResolver, "android_id");
        }
        else if (Application.platform == RuntimePlatform.IPhonePlayer) 
        {
            ios_id = UnityEngine.iOS.Device.vendorIdentifier;
        }
        else 
        {
            custom_id = SystemInfo.deviceUniqueIdentifier;
        }
    }

    public void SingInDevice()
    {
        GetDeviceID(out string android_id, out string ios_id, out string custom_id);

        if(!string.IsNullOrEmpty(android_id)) 
        {
            Debug.Log($"Logging in with Android Device ID");
            PlayFabClientAPI.LoginWithAndroidDeviceID(new LoginWithAndroidDeviceIDRequest() 
            { 
                AndroidDevice = android_id,
                OS = SystemInfo.operatingSystem,
                AndroidDeviceId = SystemInfo.deviceModel,
                TitleId = PlayFabSettings.TitleId,
                CreateAccount = true
            }, 
            response => 
            {
                Debug.Log($"Success logging in with Android Device ID");
                OnSinInSuccess.Invoke();
                playfabId = response.PlayFabId;
            }, 
            error => 
            {
                Debug.Log($"Unsuccess logging in with Android Device ID:{error.ErrorMessage}");
                OnSinInFailed.Invoke(error.ErrorMessage);
            }
            );
        }
        else if (!string.IsNullOrEmpty(ios_id)) 
        {
            Debug.Log($"Logging in with iOS Device ID");
            PlayFabClientAPI.LoginWithIOSDeviceID(new LoginWithIOSDeviceIDRequest()
            {
                DeviceId = ios_id,
                OS = SystemInfo.operatingSystem,
                DeviceModel = SystemInfo.deviceModel,
                TitleId = PlayFabSettings.TitleId,
                CreateAccount = true
            },
            response =>
            {
                Debug.Log($"Success logging in with iOS Device ID");
                OnSinInSuccess.Invoke();
                playfabId = response.PlayFabId;
            },
            error =>
            {
                Debug.Log($"Unsuccess logging in with iOS Device ID:{error.ErrorMessage}");
                OnSinInFailed.Invoke(error.ErrorMessage);
            }
            );
        }
        else if (!string.IsNullOrEmpty(custom_id))
        {
            Debug.Log($"Logging in with Custom ID");
            PlayFabClientAPI.LoginWithCustomID(new LoginWithCustomIDRequest()
            {
                CustomId = custom_id,
                TitleId = PlayFabSettings.TitleId,
                CreateAccount = true
            },
            response =>
            {
                Debug.Log($"Success logging in with Custom ID");
                OnSinInSuccess.Invoke();
                playfabId = response.PlayFabId;
            },
            error =>
            {
                Debug.Log($"Unsuccess logging in with Custom ID:{error.ErrorMessage}");
                OnSinInFailed.Invoke(error.ErrorMessage);
            }
            );
        }
    }

    public void GetUserData(string key)
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest()
        {
            PlayFabId = playfabId,
            Keys = new List<string>() 
            {
                key
            }
        },
        response =>
        {
            Debug.Log("Seccessful GetUserData");
            if (response.Data.ContainsKey(key)) OnUserDataRetrieved.Invoke(key, response.Data[key].Value);
            else OnUserDataRetrieved.Invoke(key, null);
        },
        error =>
        {
            Debug.Log($"Unseccessful GetUserData: {error.ErrorMessage}");
        }
        );
    }

    public void SetUserData(string key, string value, UnityAction OnSuccess = null)
    {
        PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest()
        {
            Data = new Dictionary<string, string>()
            {
                { key, value}
            }
        },
        response =>
        {
            Debug.Log("Seccessful SetUserData");
            OnSuccess.Invoke();
        },
        error =>
        {
            Debug.Log($"Unseccessful SetUserData: {error.ErrorMessage}");
        }
        );
    }

    public void GetStatistic (string key) 
    {
        PlayFabClientAPI.GetPlayerStatistics(new GetPlayerStatisticsRequest()
        {
            StatisticNames = new List<string> {key}
        },
        response =>
        {
            Debug.Log($"Successful GetStatistic");
           if(response.Statistics != null && response.Statistics.Count >0) 
                OnStatisticRetrieved.Invoke(key, response.Statistics[0].Value);
        },
        error =>
        {
            Debug.Log($"Unsuccessful GetStatistic: {error.ErrorMessage}");
        }
        );
    
    }

    public void SetStatistic (string key, int value)
    {
        PlayFabClientAPI.UpdatePlayerStatistics(new UpdatePlayerStatisticsRequest()
        {
            Statistics = new List<StatisticUpdate>() 
            {
                new StatisticUpdate()
                {
                StatisticName = key,
                Value = value
                }
            }
        },
         response =>
         {
             Debug.Log($"Successful SetStatistic");             
         },
        error =>
        {
            Debug.Log($"Unsuccessful SetStatistic: {error.ErrorMessage}");
        }
        );
    }

    public void GetLeaderboard (string key) 
    {
        PlayFabClientAPI.GetLeaderboard(new GetLeaderboardRequest()
        {
            StatisticName= key,
            MaxResultsCount= 5
        },
         response =>
         {
             Debug.Log($"Successful Getleaderboard");
            if(response.Leaderboard != null)
                 OnLeaderboardRetrieved.Invoke(key,response.Leaderboard);
         },
        error =>
        {
            Debug.Log($"Unsuccessful GetLeaderboard: {error.ErrorMessage}");
        }
        );
    }

    public void SetDisplayName(string displayName) 
    {
        PlayFabClientAPI.UpdateUserTitleDisplayName(new UpdateUserTitleDisplayNameRequest() 
        { 

        },
        response => 
        {
            Debug.Log($"Successful SetDisplayName");
        }, 
        error => 
        {
            Debug.Log($"Unsuccessful SetDisplayName: {error.ErrorMessage}");
        }
        );
    }
}
