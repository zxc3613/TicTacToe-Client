using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public class SignUpPanelManager : PanelManager
{
    [SerializeField] SignInPanelManager signInPanelManager;
    [SerializeField] InputField usernameInputField;
    [SerializeField] InputField firstPasswordInputField;
    [SerializeField] InputField secondPasswordInputField;
    [SerializeField] InputField nameInputField;

    [SerializeField] Button signUpButton;

    byte validationFlag = 0;

    public override void Show()
    {
        base.Show();
        signUpButton.interactable = false;
    }

    void SetInputFieldInteractable(bool value)
    {
        usernameInputField.interactable = value;
        firstPasswordInputField.interactable = value;
        secondPasswordInputField.interactable = value;
        nameInputField.interactable = value;
    }
    public void OnClickOK()
    {
        //if (PanelValidation() == false) return;

        string username = usernameInputField.text;
        string password = firstPasswordInputField.text;
        string name = nameInputField.text;

        SetInputFieldInteractable(false);

        HTTPNetworkManager.Instance.SignUp(username, password, name, (response) =>
        {
            SetInputFieldInteractable(true);


            if (response.Headers.ContainsKey("Set-Cookie"))
            {
                string cookie = response.Headers["Set-Cookie"];
                int firstIndex = cookie.IndexOf('=') + 1;
                int lastIndex = cookie.IndexOf(';');

                string cookieValue = cookie.Substring(firstIndex, lastIndex - firstIndex);

                PlayerPrefs.SetString("sid", cookieValue);
            }

            //유저의 점수 표시
            HTTPResponseInfo info = response.GetDataFromMessage<HTTPResponseInfo>();
            MainManager.Instance.SetInfo(info.name, info.score);
            //회원가입창 닫기
            Hide();
        }, () =>
        {
            SetInputFieldInteractable(true);
        });
    }

    public void OnClickCancel()
    {
        signInPanelManager.Show();
        Hide();
    }

    void OnValueChangedFinalCheck()
    {
        string firstPassword = firstPasswordInputField.text;
        string secondPassword = secondPasswordInputField.text;
        if (validationFlag == 15 && (firstPassword == secondPassword))
        {
            signUpButton.interactable = true;
        }
        else
        {
            signUpButton.interactable = false;
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
    public void OnValueChangedFirstPassword(InputField inputField)
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
    public void OnValueChangedSecondPassword(InputField inputField)
    {
        string pattern = @"^[a-zA-Z0-9]{4,12}$";
        if (Regex.IsMatch(inputField.text, pattern))
        {
            validationFlag = (byte)(validationFlag | 1 << 2);
        }
        else
        {
            validationFlag = (byte)(validationFlag & ~(1 << 2));
        }
        Debug.Log(validationFlag);
        OnValueChangedFinalCheck();
    }
    public void OnValueChangedName(InputField inputField)
    {
        string pattern = @"^[a-zA-Z가-힣_]{2,12}$";
        if (Regex.IsMatch(inputField.text, pattern))
        {
            validationFlag = (byte)(validationFlag | 1 << 3);
        }
        else
        {
            validationFlag = (byte)(validationFlag & ~(1 << 3));
        }
        Debug.Log(validationFlag);
        OnValueChangedFinalCheck();
    }

    //bool PanelValidation()
    //{
    //    if (usernameInputField.text.Length < 4 || usernameInputField.text.Length > 20) return false;
    //    if (firstPasswordInputField.text.Length < 4 || firstPasswordInputField.text.Length > 20) return false;
    //    if (firstPasswordInputField.text != secondPasswordInputField.text) return false;
    //    if (nameInputField.text.Length < 2 || nameInputField.text.Length > 15) return false;
    //    return true;
    //}
}
