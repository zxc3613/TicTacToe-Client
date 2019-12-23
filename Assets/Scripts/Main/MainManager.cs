using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;

public class MainManager : MonoBehaviour
{
    [SerializeField] SignInPanelManager signInPanelManager;
    [SerializeField] MessagePanelManager messagePanelManager;
    [SerializeField] Button startGameButton;
    [SerializeField] Button logoutButton;
    [SerializeField] Text nameText;
    [SerializeField] Text scoreText;

    static MainManager instance;
    public static MainManager Instance
    {
        get
        {
            if (!instance)
            {
                instance = GameObject.FindObjectOfType(typeof(MainManager)) as MainManager;
                if (!instance)
                {
                    GameObject container = new GameObject();
                    container.name = "MainManager";
                    instance = container.AddComponent(typeof(MainManager)) as MainManager;
                }
            }
            return instance;
        }
    }
    void Start()
    {
        EnableLoginButton(false);
        GetInfo();
    }

    void EnableLoginButton(bool value)
    {
        startGameButton.interactable = value;
        logoutButton.interactable = value;
    }

    void GetInfo()
    {
        HTTPNetworkManager.Instance.Info((response) =>
        {
            Debug.Log(response);

            string resultStr = response.Message;
            HTTPResponseInfo info = response.GetDataFromMessage<HTTPResponseInfo>();
            SetInfo(info.name, info.score);
        }, () =>
        {
            nameText.text = "";
            scoreText.text = "";
        });
        //string sid = PlayerPrefs.GetString("sid", "");
        //if (sid.Equals(""))
        //{
        //    signInPanelManager.Show();
        //}
        //else
        //{
        //    HTTPNetworkManager.Instance.Info((response) =>
        //    {
        //        Debug.Log(response);

        //        string resultStr = response.Message;
        //        //HTTPResponseMessage infoStr = JsonUtility.FromJson<HTTPResponseMessage>(resultStr);
        //        //HTTPResponseMessage info = JsonUtility.FromJson<HTTPResponseMessage>(infoStr.message);
        //        HTTPResponseInfo info = response.GetDataFromMessage<HTTPResponseInfo>();
        //        SetInfo(info.name, info.score);
        //    }, () =>
        //    {
        //        nameText.text = "";
        //        scoreText.text = "";
        //    });
        //}
    }
    //스코어 증가
    public void AddScore()
    {
        startGameButton.interactable = false;
        HTTPNetworkManager.Instance.AddScore(5, (response) =>
        {
            startGameButton.interactable = true;

            HTTPResponseInfo info = response.GetDataFromMessage<HTTPResponseInfo>();
            SetInfo(info.name, info.score);
        }, () =>
        {
            startGameButton.interactable = true;
        });
    }
    #region UI Button events
    //게임 스타트
    public void OnClickStartGame()
    {
        SceneManager.LoadScene("Game");
    }
    //로그아웃
    public void OnClickLogout()
    {
        logoutButton.interactable = false;
        HTTPNetworkManager.Instance.Logout((response) =>
        {
            PlayerPrefs.SetString("sid", "");
            startGameButton.interactable = false;
            nameText.text = "";
            scoreText.text = "";
        }, () =>
        {
            logoutButton.interactable = false;
        });
    }
    #endregion

    //텍스트에 문자열 넣기
    public void SetInfo(string name, int score)
    {
        nameText.text = name;
        scoreText.text = score.ToString();

        EnableLoginButton(true);
    }

    #region Panel 관련 메소드
    public void ShowSignInPanel()
    {
        signInPanelManager.Show();
    }
    public void ShowMessagePanel(string message, Action callback = null)
    {
        messagePanelManager.Show(message, callback);
    }
    #endregion
}
