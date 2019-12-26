using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TicTacToeManager : MonoBehaviour
{
    //플레이어의 마커타입
    private MarkerType playerMarkerType;
    //현제 게임의 상태
    private enum GameState { None, playerTurn, OpponentTurn, GameOver}
    private GameState currentState;

    [SerializeField] Cell[] cells;
    private enum GameTransform { width, height , wDiagonal, hDiagonal }

    List<Cell> width = new List<Cell>();
    List<Cell> height = new List<Cell>();


    private void Start()
    { 
        //임시 코드
        playerMarkerType = MarkerType.Circle;
        currentState = GameState.playerTurn;
    }
    private void Update()
    {
        if (currentState == GameState.playerTurn || currentState == GameState.OpponentTurn)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector2 pos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
                RaycastHit2D hitInfo = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(pos), Vector2.zero);
                if (hitInfo && hitInfo.transform.tag == "Cell")
                {
                    Cell cell = hitInfo.transform.GetComponent<Cell>();
                    if (currentState == GameState.playerTurn)
                    {
                        cell.MarkerType = playerMarkerType;
                        width.Add(cell);
                    }
                    else
                    {
                        cell.MarkerType = (playerMarkerType == MarkerType.Circle) ? MarkerType.Cross : MarkerType.Circle;
                        height.Add(cell);
                    }
                    currentState = (currentState == GameState.playerTurn) ? GameState.OpponentTurn : GameState.playerTurn;
                }

            }
        }
        if (cells[0].MarkerType != MarkerType.None && 
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
            //int s = 4;
            //switch (s)
            //{
            //    case 2:
            //        break;
            //    case 3:
            //        break;
            //    case 4:
            //        CellR(0);
            //        CellR(4);
            //        CellR(8);
            //        CellR(12);

            //        CellT(0);
            //        CellT(1);
            //        CellT(2);
            //        CellT(3);

            //        CellE(0);
            //        CellEs(3);
            //        break;
            //    case 5:
            //        break;
            //    case 6:
            //        break;
            //}
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
}



