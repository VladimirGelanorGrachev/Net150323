using UnityEngine;
using UnityEngine.UI;

public class UISingIn : MonoBehaviour
{
    [SerializeField] Text errorText;
    [SerializeField] Canvas canvas;

    string username, password;

    void OnEnable()
    {
        UserAccountManager.OnSinInFailed.AddListener(OnSingInFailed);
        UserAccountManager.OnSinInSuccess.AddListener(OnSingInSuccess);
    }

    void OnDisable()
    {
        UserAccountManager.OnSinInFailed.RemoveListener(OnSingInFailed);
        UserAccountManager.OnSinInSuccess.RemoveListener(OnSingInSuccess);
    }

    void OnSingInFailed(string error)
    { 
        errorText.gameObject.SetActive(true);
        errorText.text = error; 
    }

    void OnSingInSuccess()
    {
        canvas.enabled = false;
    }

    public void UpdateUsername(string _username)
    {
        username = _username;
    }

    public void UpdatePassword(string _password)
    {
        password = _password;
    }

    public void SingIn()
    {
        UserAccountManager.Instance.SingIn(username, password);
    }
}
