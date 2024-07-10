using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockField : MonoBehaviour
{
    private BlockManager blockManager;
    const int boardSize = 8;
    const float blockInterval = 1.5f;
    const int zeroStartX = -5;
    const int zeroStartY = 5;
    private GameObject[,] board = new GameObject[boardSize, boardSize];

    void Start()
    {
        blockManager = FindObjectOfType<BlockManager>();
        FillBoard();
    }

    private void FillBoard()
    {
        blockManager.FillBoard(board, boardSize);
        for(int i = 0; i < boardSize; i++)
        {
            for(int j = 0; j < boardSize; j++)
            {
                float x = zeroStartX + blockInterval * i;
                float y = zeroStartY - blockInterval * j;

                board[i,j].gameObject.SetActive(true);
                board[i, j].gameObject.transform.position = new Vector3(x, y, 0);
            }
        }
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
