using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SocketIO;

public class TicTacToeManager : MonoBehaviour
{
    [SerializeField] Button connectButton;
    [SerializeField] Button readyButton;
    [SerializeField] Text logText;
    [SerializeField] GameObject cellsObject;
    [SerializeField] GameOverPanelManager gameOverPanelManager;

    //플레이어의 마커타입
    private MarkerType playerMarkerType;
    //현제 게임의 상태
    private enum GameState { None, playerTurn, OpponentTurn, GameOver}
    private GameState currentState;
    private GameState CurrentState
    {
        get { return currentState; }
        set
        {
            switch (value)
            {
                case GameState.None:
                case GameState.OpponentTurn:
                case GameState.GameOver:
                    SetActveTouchCells(false);
                    break;
                case GameState.playerTurn:
                    SetActveTouchCells(true);
                    break;
                
            }
            currentState = value;
        }
    }
    //화면에 있는 셀의 정보
    //public Cell[] cells;
    public List<Cell> cells;

    //승리 판정
    private enum Winner { None, Player, Opponent, Tie }
    //Grid의 행과 열의 수
    private const int rowColNum = 3;
    //SocketIO
    private SocketIOComponent socket;

    //Roon ID와 Client ID 저장할 변수
    ClientInfo clientInfo;

    private void Start()
    { 
        //임시 코드
        playerMarkerType = MarkerType.Circle;
        CurrentState = GameState.playerTurn;

        //소켓 초기화
        InitSocket();

        //Cells 오브젝트 숨기기
        cellsObject.SetActive(false);

        //Ready 버튼 숨기기
        readyButton.gameObject.SetActive(false);
    }
    private void Update()
    {
        if (CurrentState == GameState.playerTurn)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector2 pos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
                RaycastHit2D hitInfo = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(pos), Vector2.zero);
                if (hitInfo && hitInfo.transform.tag == "Cell")
                {
                    Cell cell = hitInfo.transform.GetComponent<Cell>();
                    cell.MarkerType = playerMarkerType;

                    Winner winner = Checkwinner();

                    //선택된 셀의 정보 전달
                    int index = cells.IndexOf(cell);

                    JSONObject data = new JSONObject();
                    data.AddField("index", index);
                    data.AddField("roomId", clientInfo.roomId);

                    switch (winner)
                    {
                        case Winner.None:
                            {
                                //currentState를 상대턴으로 변경
                                //서버에게 상대가 게임을 진행할 수 있도록 메세지 전달
                                CurrentState = GameState.OpponentTurn;
                                socket.Emit("select", data);
                                break;
                            }
                        case Winner.Player:
                            {
                                //승리 팝업창 표시
                                //서버에게 player가 승리했음을 알림
                                CurrentState = GameState.GameOver;
                                socket.Emit("win", data);
                                SetLog("승리");
                                gameOverPanelManager.SetMessage("승리하였습니다.");
                                gameOverPanelManager.Show();
                                break;
                            }
                        case Winner.Tie:
                            {
                                //무승부 팝업창 표시
                                //서버에게 player가 비겼음을 알림
                                CurrentState = GameState.GameOver;
                                socket.Emit("tie", data);
                                SetLog("무승부");
                                gameOverPanelManager.SetMessage("무승부입니다.");
                                gameOverPanelManager.Show();
                                break;
                            }
                    }
                }
            }
        }
    }

    private void InitSocket()
    {
        GameObject socketObject = GameObject.Find("SocketIO");
        socket = socketObject.GetComponent<SocketIOComponent>();
        socket.On("connect", EventConnect);         //서버 연결
        socket.On("join", EventJoin);               //방 입장
        socket.On("play", EventPlay);               //게임시작
        socket.On("selected", EventSelected);       //상대방의 게임시작
        socket.On("lose", EventLose);               //졌을때
        socket.On("tie", EventTie);                 //비겼을때
    }
    #region Socket.io Events
    //서버에 연결 됐을때
    void EventConnect(SocketIOEvent e)
    {
        SetLog("서버에 접속함");

        //connect button은 숨김
        connectButton.gameObject.SetActive(false);
    }
    //방에 들어갔을때
    void EventJoin(SocketIOEvent e)
    {
        //Ready 버튼 활성화
        readyButton.gameObject.SetActive(true);

        //서버에서 roomId 가져오기
        string roomId = e.data.GetField("roomId").str;
        string clientId = e.data.GetField("clientId").str;

        clientInfo = new ClientInfo(roomId, clientId);

        SetLog("방에 입장");
        SetLog("Room ID: " + roomId);
        SetLog("Client ID: " + clientId);
    }
    //게임시작 이벤트
    void EventPlay(SocketIOEvent e)
    {
        SetLog("게임시작");

        bool isFirst = false;
        e.data.GetField(ref isFirst, "first");

        //턴과 Marker 지정
        if (isFirst)
        {
            playerMarkerType = MarkerType.Circle;
            CurrentState = GameState.playerTurn;
        }
        else
        {
            playerMarkerType = MarkerType.Cross;
            CurrentState = GameState.OpponentTurn;
        }
        //Cells 오브젝트 표시
        cellsObject.SetActive(true);
        //Ready button 숨기기
        readyButton.gameObject.SetActive(false);
    }
    //상대방이 Cell을 선택했을 경우
    void EventSelected(SocketIOEvent e)
    {
        //상대가 어느 위치를 선택했는지 인덱스로 알수 있다.
        int index = -1;
        e.data.GetField(ref index, "index");

        //상대가 무슨 모양을 했는지 보여주는것          플레이어 마커타입이 써클이라면 써클, 아니라면 크로스
        MarkerType markerType = (playerMarkerType == MarkerType.Circle) ? MarkerType.Cross : MarkerType.Circle;
        cells[index].GetComponent<Cell>().MarkerType = markerType;

        CurrentState = GameState.playerTurn;
    }
    //플레이어가 이겼을때 상대방에게 표시
    void EventLose(SocketIOEvent e)
    {
        //상대가 어느 위치를 선택했는지 인덱스로 알수 있다.
        int index = -1;
        e.data.GetField(ref index, "index");
        
        MarkerType markerType = (playerMarkerType == MarkerType.Circle) ? MarkerType.Cross : MarkerType.Circle;
        cells[index].GetComponent<Cell>().MarkerType = markerType;

        CurrentState = GameState.GameOver;
        SetLog("패배");

        gameOverPanelManager.SetMessage("패배하였습니다.");
        gameOverPanelManager.Show();
    }
    //무승부일때 상대방에게 표시
    void EventTie(SocketIOEvent e)
    {
        //상대가 어느 위치를 선택했는지 인덱스로 알수 있다.
        int index = -1;
        e.data.GetField(ref index, "index");

        MarkerType markerType = (playerMarkerType == MarkerType.Circle) ? MarkerType.Cross : MarkerType.Circle;
        cells[index].GetComponent<Cell>().MarkerType = markerType;

        CurrentState = GameState.GameOver;
        SetLog("무승부");

        gameOverPanelManager.SetMessage("무승부입니다.");
        gameOverPanelManager.Show();
       
    }
    #endregion

    //void Test(SocketIOEvent e)
    //{
    //    string msg = e.data.GetField("message").str;
    //    Debug.Log(msg);

    //    JSONObject data = new JSONObject();
    //    data.AddField("msg", "저도 반갑습니다.");
    //    socket.Emit("hello", data);
    //}
    void SetActveTouchCells(bool actve)
    {
        foreach (Cell cell in cells)
        {
            cell.SetActiveTouch(actve);
        }
    }
    Winner Checkwinner()
    {
        //1. 가로체크
        for (int i = 0; i < rowColNum; i++)
        {
            //비교를 위한 첫번째 Cell
            Cell cell = cells[rowColNum * i];
            int num = 0;

            //첫번째 Cell이 PlayerMarkerType과 다르면 for Loop 빠져나옴
            if (cell.MarkerType != playerMarkerType) continue;

            for (int j = 1; j < rowColNum; j++)
            {
                int index = i * rowColNum + j;
                if (cell.MarkerType == cells[index].MarkerType)
                {
                    ++num;
                }
            }

            if (cell.MarkerType != MarkerType.None && num == rowColNum - 1)
            {
                return Winner.Player;
            }
        }
        //2. 세로체크
        for (int i = 0; i < rowColNum; i++)
        {
            Cell cell = cells[i];
            int num = 0;

            //첫번째 Cell이 PlayerMarkerType과 다르면 for Loop 빠져나옴
            if (cell.MarkerType != playerMarkerType) continue;

            for (int j = 1; j < rowColNum; j++)
            {
                int index = j * rowColNum + i;
                if (cell.MarkerType == cells[index].MarkerType)
                {
                    ++num;
                }
            }

            if (cell.MarkerType != MarkerType.None && num == rowColNum -1)
            {
                return Winner.Player;
            }
        }
        //3. 왼쪽 대각선체크

        //첫번째 Cell이 PlayerMarkerType과 다르면 for Loop 빠져나옴
        if (cells[0].MarkerType == playerMarkerType)
        {
            Cell cell = cells[0];
            int num = 0;

            for (int i = 1; i < rowColNum; i++)
            {
                int index = i * rowColNum + i;
                if (cell.MarkerType == cells[index].MarkerType)
                {
                    ++num;
                }
            }

            if (cell.MarkerType != MarkerType.None && num == rowColNum - 1)
            {
                return Winner.Player;
            }
        }
        //4. 오른쪽 대각선체크
        if (cells[2].MarkerType == playerMarkerType)
        {
            Cell cell = cells[2];
            int num = 0;

            for (int i = 1; i < rowColNum; i++)
            {
                int index = i * rowColNum + rowColNum - i -1;
                if (cell.MarkerType == cells[index].MarkerType)
                {
                    ++num;
                }
            }

            if (cell.MarkerType != MarkerType.None && num == rowColNum - 1)
            {
                return Winner.Player;
            }
        }
        //5. 비겼는지 체크
        {
            int num = 0;
            foreach (Cell cell in cells)
            {
                if (cell.MarkerType == MarkerType.None) ++num;
            }
            if (num == 0) return Winner.Tie;
        }
        return Winner.None;
    }
    #region UI Events
    public void OnClickConnect()
    {
        connectButton.interactable = false;

        //서버에 접속
        if (socket)
            socket.Connect();
    }

    public void OnClickReady()
    {
        readyButton.interactable = false;
        JSONObject data = new JSONObject();
        data.AddField("roomId", clientInfo.roomId);
        data.AddField("clientId", clientInfo.clientId);

        socket.Emit("ready", data);
        SetLog("준비완료");
    }
    #endregion

    void SetLog(string message)
    {
        logText.text += message + "\n";
    }
}


/*if (cells[0].MarkerType != MarkerType.None && 
            cells[1].MarkerType != MarkerType.None && 
            cells[2].MarkerType != MarkerType.None && 
            cells[3].MarkerType != MarkerType.None && 
            cells[4].MarkerType != MarkerType.None && 
            cells[5].MarkerType != MarkerType.None && 
            cells[6].MarkerType != MarkerType.None && 
            cells[7].MarkerType != MarkerType.None && 
            cells[8].MarkerType != MarkerType.None && 
            cells[9].MarkerType != MarkerType.None && 
            cells[10].MarkerType != MarkerType.None && 
            cells[11].MarkerType != MarkerType.None && 
            cells[12].MarkerType != MarkerType.None && 
            cells[13].MarkerType != MarkerType.None && 
            cells[14].MarkerType != MarkerType.None && 
            cells[15].MarkerType != MarkerType.None)
        {
            Debug.Log("무승부");
        }
        else
        {
            CellR(0);
            CellR(4);
            CellR(8);
            CellR(12);

            CellT(0);
            CellT(1);
            CellT(2);
            CellT(3);

            CellE(0);
            CellEs(3);
            //if (cells[0].MarkerType != MarkerType.None && cells[1].MarkerType != MarkerType.None && cells[2].MarkerType != MarkerType.None)
            //{
            //    if (cells[0].MarkerType == cells[1].MarkerType && cells[1].MarkerType == cells[2].MarkerType)
            //    {
            //        Debug.Log(cells[0].MarkerType + " 승리");
            //    }
            //}
            //if (cells[3].MarkerType != MarkerType.None && cells[4].MarkerType != MarkerType.None && cells[5].MarkerType != MarkerType.None)
            //{
            //    if (cells[3].MarkerType == cells[4].MarkerType && cells[4].MarkerType == cells[5].MarkerType)
            //    {
            //        Debug.Log(cells[3].MarkerType + " 승리");
            //    }
            //}
            //if (cells[6].MarkerType != MarkerType.None && cells[7].MarkerType != MarkerType.None && cells[8].MarkerType != MarkerType.None)
            //{
            //    if (cells[6].MarkerType == cells[7].MarkerType && cells[7].MarkerType == cells[8].MarkerType)
            //    {
            //        Debug.Log(cells[6].MarkerType + " 승리");
            //    }
            //}


            //if (cells[0].MarkerType != MarkerType.None && cells[3].MarkerType != MarkerType.None && cells[6].MarkerType != MarkerType.None)
            //{
            //    if (cells[0].MarkerType == cells[3].MarkerType && cells[3].MarkerType == cells[6].MarkerType)
            //    {
            //        Debug.Log(cells[0].MarkerType + " 승리");
            //    }
            //}
            //if (cells[1].MarkerType != MarkerType.None && cells[4].MarkerType != MarkerType.None && cells[7].MarkerType != MarkerType.None)
            //{
            //    if (cells[1].MarkerType == cells[4].MarkerType && cells[4].MarkerType == cells[7].MarkerType)
            //    {
            //        Debug.Log(cells[1].MarkerType + " 승리");
            //    }
            //}
            //if (cells[2].MarkerType != MarkerType.None && cells[5].MarkerType != MarkerType.None && cells[8].MarkerType != MarkerType.None)
            //{
            //    if (cells[2].MarkerType == cells[5].MarkerType && cells[5].MarkerType == cells[8].MarkerType)
            //    {
            //        Debug.Log(cells[2].MarkerType + " 승리");
            //    }
            //}


            //if (cells[0].MarkerType != MarkerType.None && cells[4].MarkerType != MarkerType.None && cells[8].MarkerType != MarkerType.None)
            //{
            //    if (cells[0].MarkerType == cells[4].MarkerType && cells[4].MarkerType == cells[8].MarkerType)
            //    {
            //        Debug.Log(cells[0].MarkerType + " 승리");
            //    }
            //}
            //if (cells[2].MarkerType != MarkerType.None && cells[4].MarkerType != MarkerType.None && cells[6].MarkerType != MarkerType.None)
            //{
            //    if (cells[2].MarkerType == cells[4].MarkerType && cells[4].MarkerType == cells[6].MarkerType)
            //    {
            //        Debug.Log(cells[2].MarkerType + " 승리");
            //    }
            //}
        }
    }
    //가로
    void CellR(int i)
    {
        if (cells[i].MarkerType != MarkerType.None && cells[i + 1].MarkerType != MarkerType.None && cells[i + 2].MarkerType != MarkerType.None && cells[i + 3].MarkerType != MarkerType.None)
        {
            if (cells[i].MarkerType == cells[i + 1].MarkerType && cells[i + 1].MarkerType == cells[i + 2].MarkerType && cells[i + 2].MarkerType == cells[i + 3].MarkerType)
            {
                Debug.Log(cells[i].MarkerType + " 승리");
            }
        }
    }
    //세로
    void CellT(int i)
    {
        if (cells[i].MarkerType != MarkerType.None && cells[i + 4].MarkerType != MarkerType.None && cells[i + 8].MarkerType != MarkerType.None && cells[i + 12].MarkerType != MarkerType.None)
        {
            if (cells[i].MarkerType == cells[i + 4].MarkerType && cells[i + 4].MarkerType == cells[i + 8].MarkerType && cells[i + 8].MarkerType == cells[i + 12].MarkerType)
            {
                Debug.Log(cells[i].MarkerType + " 승리");
            }
        }
    }
    //왼쪽 대각선
    void CellE(int i)
    {
        if (cells[i].MarkerType != MarkerType.None && cells[i + 5].MarkerType != MarkerType.None && cells[i + 10].MarkerType != MarkerType.None && cells[i + 15].MarkerType != MarkerType.None)
        {
            if (cells[i].MarkerType == cells[i + 5].MarkerType && cells[i + 5].MarkerType == cells[i + 10].MarkerType && cells[i + 10].MarkerType == cells[i + 15].MarkerType)
            {
                Debug.Log(cells[i].MarkerType + " 승리");
            }
        }
    }
    //오른쪽 대각선
    void CellEs(int i)
    {
        if (cells[i].MarkerType != MarkerType.None && cells[i + 3].MarkerType != MarkerType.None && cells[i + 6].MarkerType != MarkerType.None && cells[i + 9].MarkerType != MarkerType.None)
        {
            if (cells[i].MarkerType == cells[i + 3].MarkerType && cells[i + 3].MarkerType == cells[i + 6].MarkerType && cells[i + 6].MarkerType == cells[i + 9].MarkerType)
            {
                Debug.Log(cells[i].MarkerType + " 승리");
            }
        }
    }
    */


