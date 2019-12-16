using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SignInPanelManager : PanelManager
{
    [SerializeField] SignUpPanelManager signUpPanelManager;
    [SerializeField] InputField usernameInputField;
    [SerializeField] InputField passwordInputField;

    public void OnClickSignUp()
    {
        signUpPanelManager.Show();
    }
    public void OnClickSignIn()
    {
        //로그인
        HTTPNetworkManager.Instance.SignIn(usernameInputField.text, passwordInputField.text, (response) =>
        {
            if (response.Headers.ContainsKey("Set-Cookie"))
            {
                string cookie = response.Headers["Set-Cookie"];
                int firstIndex = cookie.IndexOf('=') + 1;
                int lastIndex = cookie.IndexOf(';');

                string cookieValue = cookie.Substring(firstIndex, lastIndex - firstIndex);

                PlayerPrefs.SetString("sid", cookieValue);
            }
        }, () =>
        { 
            //로그인창 흔들리기.
        });

        Hide();
    }
}
