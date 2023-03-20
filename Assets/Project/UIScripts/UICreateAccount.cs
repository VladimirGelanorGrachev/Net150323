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

    string username, password, emailAddress;

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

    public void UpdateUsername(string _username)
    {
        username = _username;
    }

    public void UpdatePassword(string _password) 
    {
        password = _password;
    }

    public void UpdtateEmailAddress(string _emailAddress)
    {
        emailAddress = _emailAddress;
    }

    public void CreateAccount()
    {
        UserAccountManager.Instance.CreationAccount(username, emailAddress, password);
    }
}
