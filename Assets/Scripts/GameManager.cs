using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] SignInPanelManager signInPanelManager;
    [SerializeField] Button addScoreButton;

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
        ChecksignIn();
    }

    void ChecksignIn()
    {
        string sid = PlayerPrefs.GetString("sid", "");
        if (sid.Equals(""))
        {
            signInPanelManager.Show();
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
    public void ShowSignInPanel()
    {
        signInPanelManager.Show();
    }
}
