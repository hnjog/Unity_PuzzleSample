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


    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                GameObject clickedObject = hit.collider.gameObject;
                Debug.Log("Clicked object: " + clickedObject.name);

                GameObject parentObject = clickedObject.transform.parent.gameObject;

                // board 배열에서 clickedObject를 찾기
                for (int i = 0; i < boardSize; i++)
                {
                    for (int j = 0; j < boardSize; j++)
                    {
                        if (board[i, j] != null && board[i, j].Equals(parentObject))
                        {
                            Debug.Log($"Clicked object is at ({i}, {j}) in the board");
                            break;
                        }
                    }
                }
            }
        }
    }

}
