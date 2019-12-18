using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] SignInPanelManager signInPanelManager;
    [SerializeField] Button addScoreButton;
    [SerializeField] Button logoutButton;
    [SerializeField] Text nameText;
    [SerializeField] Text scoreText;
    static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            if (!instance)
            {
                instance = GameObject.FindObjectOfType(typeof(GameManager)) as GameManager;
                if (!instance)
                {
                    GameObject container = new GameObject();
                    container.name = "GameManager";
                    instance = container.AddComponent(typeof(GameManager)) as GameManager;
                }
            }
            return instance;
        }
    }
    void Start()
    {
        GetInfo();
    }

    void GetInfo()
    {
        string sid = PlayerPrefs.GetString("sid", "");
        if (sid.Equals(""))
        {
            signInPanelManager.Show();
        }
        else
        {
            HTTPNetworkManager.Instance.Info((response) =>
            {
                Debug.Log(response);

                string resultStr = response.Message;
                //HTTPResponseMessage infoStr = JsonUtility.FromJson<HTTPResponseMessage>(resultStr);
                //HTTPResponseMessage info = JsonUtility.FromJson<HTTPResponseMessage>(infoStr.message);
                HTTPResponseInfo info = response.GetDataFromMessage<HTTPResponseInfo>();
                SetInfo(info.name, info.score);
            }, () =>
            {
                nameText.text = "";
                scoreText.text = "";
            });
        }
    }
    public void AddScore()
    {
        addScoreButton.interactable = false;
        HTTPNetworkManager.Instance.AddScore(5, (response) =>
        {
            addScoreButton.interactable = true;

            HTTPResponseInfo info = response.GetDataFromMessage<HTTPResponseInfo>();
            SetInfo(info.name, info.score);
        }, () =>
        {
            addScoreButton.interactable = true;
        });
    }
    //로그아웃
    public void Logout()
    {
        logoutButton.interactable = false;
        HTTPNetworkManager.Instance.Logout((response) =>
        {
            PlayerPrefs.SetString("sid", "");
            addScoreButton.interactable = false;
            nameText.text = "";
            scoreText.text = "";
        }, () =>
        {
            logoutButton.interactable = false;
        });
    }
    public void ShowSignInPanel()
    {
        signInPanelManager.Show();
    }
    //텍스트에 문자열 넣기
    public void SetInfo(string name, int score)
    {
        nameText.text = name;
        scoreText.text = score.ToString();
    }
}
