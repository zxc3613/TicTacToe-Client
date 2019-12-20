using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatPanelManager : MonoBehaviour
{
    [SerializeField] GameObject chatCellPrefab;
    [SerializeField] Transform content;

    [SerializeField] ScrollRect scrollRect;
    [SerializeField] InputField messageInputField;
    [SerializeField] Button sendButton;

    int lastSeq = 0;

    void Start()
    {
        //매초마다 한번씩 새로운 메세지 받아오기
        StartCoroutine(GetNewMessage());
    }

    public void OnClickSendMessage()
    {
        if (messageInputField.text != "")
        {
            HTTPNetworkManager.Instance.AddMessage(messageInputField.text, (response) =>
            {
                Debug.Log(response);
                messageInputField.text = "";
            }, () =>
            {

            });
        }
    }
    IEnumerator GetNewMessage()
    {
        //서버로부터 메세지 받아오기
        while(true)
        {
            HTTPNetworkManager.Instance.LoadChat((response) =>
            {
                if (response.Message == "") return;

                HTTPResponseChat chat = response.GetDataFromMessage<HTTPResponseChat>();
                foreach (HTTPResponseChat.Chat message in chat.objects)
                {
                    if (lastSeq != 0)
                    {
                        ChatCell chatCell = Instantiate(chatCellPrefab, content).GetComponent<ChatCell>();
                        chatCell.CachedText.text = string.Format("[{0}] {1}", message.name, message.message);
                        chatCell.transform.SetAsLastSibling();
                    }
                    lastSeq = int.Parse(message._id);
                }
            }, () =>
            {

            }, lastSeq);
            yield return new WaitForSeconds(1);
        }
    }
}
