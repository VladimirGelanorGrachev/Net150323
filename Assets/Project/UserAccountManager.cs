using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine.Events;
using System.Collections.Generic;
using System.Collections;
using TMPro;
using UnityEngine.UI;

public class UserAccountManager : MonoBehaviour
{
    public static UserAccountManager Instance;

    public static UnityEvent OnSinInSuccess = new UnityEvent();
    public static UnityEvent<string> OnSinInFailed = new UnityEvent<string>();
    public static UnityEvent<string> OnCreateAccountFailed = new UnityEvent<string>();
    public static UnityEvent<string, string> OnUserDataRetrieved = new UnityEvent<string, string> { };
    public static UnityEvent<string, int> OnStatisticRetrieved = new UnityEvent<string, int> { };
    public static UnityEvent<string, List<PlayerLeaderboardEntry>> OnLeaderboardRetrieved = new UnityEvent<string, List<PlayerLeaderboardEntry>> { };

    public static string playfabID;
    public static UserAccountInfo userAccountInfo;




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
            playfabID = response.PlayFabId;
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
                playfabID = response.PlayFabId;
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
                playfabID = response.PlayFabId;
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
                playfabID = response.PlayFabId;
            },
            error =>
            {
                Debug.Log($"Unsuccess logging in with Custom ID:{error.ErrorMessage}");
                OnSinInFailed.Invoke(error.ErrorMessage);
            }
            );
        }
    }


#region Statistics and Leaderboard
    public void GetStatistic (string statistic) 
    {
        PlayFabClientAPI.GetPlayerStatistics(new GetPlayerStatisticsRequest()
        {
            StatisticNames = new List<string> {statistic}
        },
        result =>
        {
            if (result.Statistics.Count > 0)
            {
                Debug.Log($"Successfuly got {statistic} | {result.Statistics[0]}");
                if (result.Statistics != null)
                {
                    OnStatisticRetrieved.Invoke(statistic, result.Statistics[0].Value);
                }
            }
            else
            {
                Debug.Log($"No existing statistic [{statistic}] for user");
            }
        },
        error =>
        {
            Debug.Log($"Could not retrieve {statistic} | {error.ErrorMessage}");
        }
        );
    
    }

    public void SetStatistic (string statistic, int value)
    {
        PlayFabClientAPI.UpdatePlayerStatistics(new UpdatePlayerStatisticsRequest()
        {
            Statistics = new List<StatisticUpdate>() 
            {
                new StatisticUpdate()
                {
                StatisticName = statistic,
                Value = value
                }
            }
        },
         result =>
         {
             Debug.Log($"{statistic} successfully updated");
             GetLeaderboard(statistic);
         },
        error =>
        {
            Debug.Log($"{statistic} update unsuccessful | {error.ErrorMessage}");
        }
        );
    }

    public void GetLeaderboard (string statistic) 
    {
        PlayFabClientAPI.GetLeaderboard(new GetLeaderboardRequest()
        {
            StatisticName = statistic            
        },
         result =>
         {
             Debug.Log($"Successfully got {statistic} leaderboard | 0.{result.Leaderboard[0].DisplayName} {result.Leaderboard[0].StatValue}");
             OnLeaderboardRetrieved.Invoke(statistic, result.Leaderboard);
         },
        error =>
        {
            Debug.Log($"Could not retrieve {statistic} leaderboard | {error.ErrorMessage}");
        }
        );
    }
    public void GetLeaderboardDelayed(string statistic)
    {
        StartCoroutine(CheckLeaderboardDelay(statistic));
    }

    IEnumerator CheckLeaderboardDelay(string statistic)
    {
        yield return new WaitForSeconds(3);
        GetLeaderboard(statistic);
    }

    #endregion

#region DisplayName
    void CheckDisplayName(string username, UnityAction completeAction)
    {
        PlayFabClientAPI.GetAccountInfo(new GetAccountInfoRequest()
        {
            PlayFabId = playfabID
        }, result => {
            userAccountInfo = result.AccountInfo;

            if (result.AccountInfo.TitleInfo.DisplayName == null || result.AccountInfo.TitleInfo.DisplayName.Length == 0)
            {
                PlayFabClientAPI.UpdateUserTitleDisplayName(new UpdateUserTitleDisplayNameRequest()
                {
                    DisplayName = username
                }, result => {
                    Debug.Log($"Display name set to username");
                    completeAction.Invoke();
                }, error => {
                    Debug.Log($"Display name could not be set to username | {error.ErrorMessage}");
                });
            }
            else
            {
                completeAction.Invoke();
            }
        }, error => {
            Debug.Log($"Could not retrieve AccountInfo | {error.ErrorMessage}");
        });
    }

    public void SetDisplayName(string displayName)
    {
        PlayFabClientAPI.UpdateUserTitleDisplayName(new UpdateUserTitleDisplayNameRequest()
        {
            DisplayName = displayName
        }, result => 
        {
            Debug.Log($"Display name set to username");
        }, error => 
        {
            Debug.Log($"Display name could not be set to username | {error.ErrorMessage}");
        });
    }
#endregion

#region UserData
    public void GetUserData(string key)
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest()
        {
            PlayFabId = playfabID,
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

    public void SetUserData(string key, string userData)
    {
        PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest()
        {
            Data = new Dictionary<string, string>()
            {
                { key, userData}
            }
        },
        response =>
        {
            Debug.Log($"{key} Seccessful SetUserData");

        },
        error =>
        {
            Debug.Log($"{key} Unseccessful SetUserData: {error.ErrorMessage}");
        }
        );
    }
#endregion

}
