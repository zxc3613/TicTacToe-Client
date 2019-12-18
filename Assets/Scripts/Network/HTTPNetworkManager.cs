using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;

public class HTTPNetworkManager : MonoBehaviour
{
    static HTTPNetworkManager instance;
    public static HTTPNetworkManager Instance
    {
        get 
        {
            if (!instance)
            {
                instance = GameObject.FindObjectOfType(typeof(HTTPNetworkManager)) as HTTPNetworkManager;
                if (!instance)
                {
                    GameObject container = new GameObject();
                    container.name = "HTTPNetworkManager";
                    instance = container.AddComponent(typeof(HTTPNetworkManager)) as HTTPNetworkManager;
                }
            }
            return instance;
        }
    }

    public void SignIn(string username, string password, Action<HTTPResponse> success, Action fail)
    {
        //로그인
        HTTPRequestSignIn signInData = new HTTPRequestSignIn(username, password);
        var postData = signInData.GetJSON();

        StartCoroutine(SendPostRequest(postData, HTTPNetworkConstant.logInRequestURL, success, fail));
    }

    public void SignUp(string username, string password, string name, Action<HTTPResponse> success, Action fail)
    {
        //회원가입
        HTTPRequestSignUp signUpData = new HTTPRequestSignUp(username, password, name);
        var postData = signUpData.GetJSON();

        StartCoroutine(SendPostRequest(postData, HTTPNetworkConstant.signUpRequestURL, success, fail));
    }

    public void AddScore(int score, Action<HTTPResponse> success, Action fail)
    {
        //스코어 저장
        HTTPRequestAddScore addScoreData = new HTTPRequestAddScore(score);
        addScoreData.score = score;

        var postData = addScoreData.GetJSON();

        StartCoroutine(SendPostRequest(postData, HTTPNetworkConstant.addScoreRequestURL, success, fail));
    }

    public void Info(Action<HTTPResponse> success, Action fail)
    {
        StartCoroutine(SendGetRequest(HTTPNetworkConstant.infoRequestURL, success, fail));
    }

    public void Logout(Action<HTTPResponse> success, Action fail)
    {
        StartCoroutine(SendGetRequest(HTTPNetworkConstant.logoutURL, success, fail));
    }
    IEnumerator SendGetRequest(string requestURL, Action<HTTPResponse> success, Action fail)
    {
        using (UnityWebRequest www = UnityWebRequest.Get(HTTPNetworkConstant.serverURL + requestURL))
        {
            yield return www.SendWebRequest();
            if (www.isNetworkError)
            {
                Debug.Log(www.error);
                fail();
            }
            else if (www.isHttpError)
            {
                long code = www.responseCode;
                switch (code)
                {
                    case 401:
                        PlayerPrefs.SetString("sid", "");
                        GameManager.Instance.ShowSignInPanel();
                        break;
                }
                fail();
            }
            else
            {
                Dictionary<string, string> headers = www.GetResponseHeaders();

                long code = www.responseCode;
                HTTPResponseMessage message = JsonUtility.FromJson<HTTPResponseMessage>(www.downloadHandler.text);
                HTTPResponse response = new HTTPResponse(code, message.message, headers);

                //string message = www.downloadHandler.text;
                //HTTPResponse response = new HTTPResponse(code, message, headers);
                success(response);
            }
        }
    }

    IEnumerator SendPostRequest(string data, string requestURL, Action<HTTPResponse> success, Action fail)
    {
        using (UnityWebRequest www = UnityWebRequest.Put(HTTPNetworkConstant.serverURL + requestURL, data))
        {
            www.method = "Post";
            www.SetRequestHeader("Content-Type", "application/json");
            string sid = PlayerPrefs.GetString("sid", "");
            if (sid != "")
            {
                www.SetRequestHeader("Set-Cookie", sid);
            }
            yield return www.SendWebRequest();


            if (www.isNetworkError)
            {
                Debug.Log(www.error);
                fail();
            }
            else if (www.isHttpError)
            {
                long code = www.responseCode;
                switch (code)
                {
                    case 401:
                        PlayerPrefs.SetString("sid", "");
                        GameManager.Instance.ShowSignInPanel();
                        break;
                }
                fail();
            }
            else
            {
                //서버 > 클라이언트로 응답(Response) 메세지 도착
                Dictionary<string, string> headers = www.GetResponseHeaders();

                long code = www.responseCode;
                //string message = www.downloadHandler.text;

                HTTPResponseMessage message = JsonUtility.FromJson<HTTPResponseMessage>(www.downloadHandler.text);
                HTTPResponse response = new HTTPResponse(code, message.message, headers);
                success(response);
            }

        }
    }
}
