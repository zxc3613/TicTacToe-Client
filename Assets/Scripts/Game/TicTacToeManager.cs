using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TicTacToeManager : MonoBehaviour
{
    [SerializeField] Cell cell;

    void Start()
    {
        cell.MarkerType = MarkerType.Circle;
    }

    void Update()
    {
        
    }
}
