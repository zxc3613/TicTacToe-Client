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
                nameText.text = info.name;
                scoreText.text = info.score.ToString();
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
        }, () =>
        {
            addScoreButton.interactable = true;
        });
    }
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
}
