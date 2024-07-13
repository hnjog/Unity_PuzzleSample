using System;
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

    private Tuple<int, int> selectedPosition1 = null;
    private Tuple<int, int> selectedPosition2 = null;

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

                // board 배열에서 부모 게임 오브젝트를 찾기
                Tuple<int, int> position = FindPositionInBoard(parentObject);
                if (position != null)
                {
                    Debug.Log($"Clicked object is at ({position.Item1}, {position.Item2}) in the board");

                    // 첫 번째 선택인 경우
                    if (selectedPosition1 == null)
                    {
                        selectedPosition1 = position;
                        Debug.Log($"First selection at ({selectedPosition1.Item1}, {selectedPosition1.Item2})");
                    }
                    // 두 번째 선택인 경우
                    else if (selectedPosition2 == null)
                    {
                        selectedPosition2 = position;
                        Debug.Log($"Second selection at ({selectedPosition2.Item1}, {selectedPosition2.Item2})");

                        // 두 선택 간 비교
                        CompareSelectedPositions();
                    }
                }
            }
        }
    }

    void CompareSelectedPositions()
    {
        if (selectedPosition1 != null && selectedPosition2 != null)
        {
            (int x1, int y1) = selectedPosition1.ToValueTuple();
            (int x2, int y2) = selectedPosition2.ToValueTuple();

            //Debug.Log($"Comparing ({selectedPosition1.Item1}, {selectedPosition1.Item2}) and ({selectedPosition2.Item1}, {selectedPosition2.Item2})");

            int disValue = Math.Abs(x1 - x2) + Math.Abs(y1 - y2);

            if(disValue == 1)
            {
                SwapPositions(selectedPosition1, selectedPosition2);
            }
        }

        // 선택 초기화
        selectedPosition1 = null;
        selectedPosition2 = null;
    }

    void SwapPositions(Tuple<int, int> pos1, Tuple<int, int> pos2)
    {
        // 첫 번째 오브젝트의 board 위치와 transform 위치 저장
        int x1 = pos1.Item1;
        int y1 = pos1.Item2;
        Vector3 pos1Transform = board[x1, y1].transform.position;
        GameObject obj1 = board[x1, y1];

        // 두 번째 오브젝트의 board 위치와 transform 위치 저장
        int x2 = pos2.Item1;
        int y2 = pos2.Item2;
        Vector3 pos2Transform = board[x2, y2].transform.position;
        GameObject obj2 = board[x2, y2];

        // 두 오브젝트의 board 위치와 transform 위치 swap
        board[x1, y1].transform.position = pos2Transform;
        board[x2, y2].transform.position = pos1Transform;
        board[x1, y1] = obj2;
        board[x2, y2] = obj1;

        CheckMatches();
    }

    Tuple<int, int> FindPositionInBoard(GameObject parentObject)
    {
        for (int i = 0; i < boardSize; i++)
        {
            for (int j = 0; j < boardSize; j++)
            {
                if (board[i, j] != null && board[i, j].Equals(parentObject))
                {
                    return new Tuple<int, int>(i, j);
                }
            }
        }
        return null;
    }

    void CheckMatches()
    {
        for (int i = 0; i < boardSize; i++)
        {
            for (int j = 0; j < boardSize; j++)
            {
                // 가로 3개 체크
                if (j + 2 < boardSize &&
                    board[i, j].GetComponent<BaseBlock>().jewelType == board[i, j + 1].GetComponent<BaseBlock>().jewelType &&
                    board[i, j + 1].GetComponent<BaseBlock>().jewelType == board[i, j + 2].GetComponent<BaseBlock>().jewelType)
                {
                    RemoveBlocks(i,j);
                    RemoveBlocks(i,j + 1);
                    RemoveBlocks(i,j + 2);
                }

                // 세로 3개 체크
                if (i + 2 < boardSize &&
                         board[i, j].GetComponent<BaseBlock>().jewelType == board[i + 1, j].GetComponent<BaseBlock>().jewelType &&
                         board[i + 1, j].GetComponent<BaseBlock>().jewelType == board[i + 2, j].GetComponent<BaseBlock>().jewelType)
                {
                    RemoveBlocks(i, j);
                    RemoveBlocks(i + 1, j);
                    RemoveBlocks(i + 2, j);
                }
            }
        }
    }

    void RemoveBlocks(int x, int y)
    {
        board[x, y].SetActive(false);
        //board[x, y] = null;

        // block이 없어지는 것은 하나의 타이밍
        // 이후 그 타이밍이 끝난 후,
        // board에서 빈 녀석들을 찾아 그 위쪽 녀석들을 떨어트린다

        //for (int i = y; i > 0; i--)
        //{
        //    board[x, i] = board[x, i - 1];
        //}

        
    }
}
