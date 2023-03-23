using PlayFab;
using PlayFab.ClientModels;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICreateAccount : MonoBehaviour
{
    [SerializeField] Text errorText;
    [SerializeField] Canvas canvas;

    private string _username, _password, _emailAddress;

    void OnEnable()
    {
        UserAccountManager.OnCreateAccountFailed.AddListener(OnCreateAccountFailed);
        UserAccountManager.OnSinInSuccess.AddListener(OnCreateAccountSuccess);
    }

    void OnDisable()
    {
        UserAccountManager.OnCreateAccountFailed.RemoveListener(OnCreateAccountFailed);
        UserAccountManager.OnSinInSuccess.RemoveListener(OnCreateAccountSuccess);
    }

    void OnCreateAccountSuccess()
    {
        canvas.enabled = false;
    }

    void OnCreateAccountFailed(string error)
    {
        errorText.gameObject.SetActive(true);
        errorText.text = error;
    }

    public void UpdateUsername(string username)
    {
        _username = username;
    }

    public void UpdatePassword(string password) 
    {
        _password = password;
    }

    public void UpdtateEmailAddress(string emailAddress)
    {
        _emailAddress = emailAddress;
    }

    public void CreateAccount()
    {
        UserAccountManager.Instance.CreationAccount(_username, _emailAddress, _password);
    }
}
