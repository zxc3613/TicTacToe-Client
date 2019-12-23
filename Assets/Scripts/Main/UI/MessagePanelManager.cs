using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class MessagePanelManager : PanelManager
{
    [SerializeField] Text messageText;
    Action callback;

    public void Show(string message, Action callback)        //=null 넣으면 있으면 실행 없으면 null
    {
        messageText.text = message;
        base.Show();
        this.callback = callback;
    }

    public void OnClickOk()
    {
        Hide();
        callback?.Invoke();         //? 넣으면 callback가 있으면 실행 시켜라
    }
}
