using System;
using UnityEngine;
using UnityEngine.Events;



public class UserProfile : MonoBehaviour
{
    public static UserProfile instance;

    public static UnityEvent<ProfileData> OnProfileData = new UnityEvent<ProfileData>();

    [SerializeField] ProfileData _profileData;
    public float _xpThreshold = 1000;

    void Awake()
    {
        instance = this; 
    }


    private void OnEnable()
    {
        UserAccountManager.OnSinInSuccess.AddListener(SingIn);
        UserAccountManager.OnUserDataRetrived.AddListener(UsedDataRetrived);
    }
    
    private void OnDisable()
    {
        UserAccountManager.OnSinInSuccess.RemoveListener(SingIn);
        UserAccountManager.OnUserDataRetrived.RemoveListener(UsedDataRetrived);
    }

    void SingIn()
    {
        GetUserData();
    }

    [ContextMenu("Get Profile Data")]
    void GetUserData()
    {
        UserAccountManager.Instance.GetUserData("ProfileData");
    }

    void UsedDataRetrived(string key, string value)
    {
        if(key == "ProfileData")
        {
            _profileData = JsonUtility.FromJson<ProfileData>(value);
            OnProfileData.Invoke(_profileData);
        }
    }

    [ContextMenu("Set Profile Data")]
    void SetUserData(UnityAction OnSuccess = null)
    {
        UserAccountManager.Instance.SetUserData("ProfileData", JsonUtility.ToJson(_profileData));
    }

    public void AddXp() 
    {
        _profileData._level += UnityEngine.Random.Range(0f, 1f);
        SetUserData(GetUserData);
    }
    

}

[System.Serializable]
public class ProfileData
{
    public string _playerName;
    public float _level;    
}