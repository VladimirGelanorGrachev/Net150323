using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine.Events;


public class UserAccountManager : MonoBehaviour
{
    public static UserAccountManager Instance;

    public static UnityEvent OnSinInSuccess = new UnityEvent();
    public static UnityEvent<string> OnSinInFailed = new UnityEvent<string>();
    public static UnityEvent<string> OnCreateAccountFailed = new UnityEvent<string>();

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
            Debug.Log($"Successful Account Creation: {username}");
            OnSinInSuccess.Invoke();
        },
        error => { 
            Debug.Log($"Unsuccessful Account Creation: {username} \n {error.ErrorMessage}");
            OnSinInFailed.Invoke(error.ErrorMessage);
        }
        );
    }
}
