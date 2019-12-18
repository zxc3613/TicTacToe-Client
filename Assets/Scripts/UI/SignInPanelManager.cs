using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class SignInPanelManager : PanelManager
{
    [SerializeField] SignUpPanelManager signUpPanelManager;
    [SerializeField] InputField usernameInputField;
    [SerializeField] InputField passwordInputField;

    [SerializeField] Button signInButton;

    byte validationFlag = 0;

    public override void Show()
    {
        base.Show();
        signInButton.interactable = false;
    }
    public void OnClickSignUp()
    {
        this.Hide();
        signUpPanelManager.Show();
    }
    public void OnClickSignIn()
    {
        //Validation
        //if (PanelValidation() == false) return;
        //로그인
        HTTPNetworkManager.Instance.SignIn(usernameInputField.text, passwordInputField.text, (response) =>
        {
            //세션 ID 저장
            if (response.Headers.ContainsKey("Set-Cookie"))
            {
                string cookie = response.Headers["Set-Cookie"];
                int firstIndex = cookie.IndexOf('=') + 1;
                int lastIndex = cookie.IndexOf(';');

                string cookieValue = cookie.Substring(firstIndex, lastIndex - firstIndex);

                PlayerPrefs.SetString("sid", cookieValue);
            }
            Debug.Log(response.Message);
            //유저의 점수 표시
            HTTPResponseInfo info = response.GetDataFromMessage<HTTPResponseInfo>();
            GameManager.Instance.SetInfo(info.name, info.score);
            //로그인창 닫기
            Hide();
        }, () =>
        { 
            //로그인창 흔들리기.
        });
    }
    void OnValueChangedFinalCheck()
    {
        if (validationFlag == 3)
        {
            signInButton.interactable = true;
        }
        else
        {
            signInButton.interactable = false;
        }
    }
    public void OnValueChangedUsername(InputField inputField)
    {
        string pattern = @"^[a-zA-Z0-9]{4,12}$";
        if (Regex.IsMatch(inputField.text, pattern))
        {
            validationFlag = (byte)(validationFlag | 1);
        }
        else
        {
            validationFlag = (byte)(validationFlag & ~1);       //~ 넣으면 비트 반전
        }
        Debug.Log(validationFlag);
        OnValueChangedFinalCheck();
    }
    public void OnValueChangedPassword(InputField inputField)
    {
        string pattern = @"^[a-zA-Z0-9]{4,12}$";
        if (Regex.IsMatch(inputField.text, pattern))
        {
            validationFlag = (byte)(validationFlag | 2);
        }
        else
        {
            validationFlag = (byte)(validationFlag & 13);
        }
        Debug.Log(validationFlag);
        OnValueChangedFinalCheck();
    }
    //bool PanelValidation()
    //{
    //    if (usernameInputField.text.Length < 4 || usernameInputField.text.Length > 20) return false;
    //    if (passwordInputField.text.Length < 4 || passwordInputField.text.Length > 20) return false;
    //    return true;
    //}
}
