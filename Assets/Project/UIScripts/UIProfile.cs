using System;
using System.Collections;
using System.Collections.Generic;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.UI;

public class UIProfile : MonoBehaviour
{
    [SerializeField] CanvasGroup canvasGroup;

    [SerializeField] InputField playerNameText;
    [SerializeField] Text playerLevelText;
    [SerializeField] Text playerXPtext;
    [SerializeField] Text playerXPFill;

    void OnEnable()
    {
        UserAccountManager.OnSinInSuccess.AddListener(SingIn);

        UserProfile.OnProfileData.AddListener(ProfileDataUpdate);
    }

    void OnDisable()
    {
        UserAccountManager.OnSinInSuccess.RemoveListener(SingIn);
        UserProfile.OnProfileData.RemoveListener(ProfileDataUpdate);
    }

    void SingIn()
    {
        //SceneManager.LoadScene(1);
        canvasGroup.alpha = 1;
    }



    private void ProfileDataUpdate(ProfileData profileData)
    {
        float level = (Mathf.Floor(profileData._level));
        float xp = profileData._level - level;

        playerNameText.text = profileData._playerName;
        playerLevelText.text = level.ToString();
        playerXPtext.text = (xp * UserProfile.instance.levelCap).ToString();
        //playerXPFill.fillAmount = ((float)profileData.xp) / (UserProfile.Instance.levelCap);
    }
}
