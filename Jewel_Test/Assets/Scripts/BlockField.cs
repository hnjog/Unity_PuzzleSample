using Assets.Scripts.ETC;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockField : MonoBehaviour
{
    private BlockManager blockManager;
    const int boardSize = 8;
    const float blockInterval = 1.5f;
    const float zeroStartX = -5.0f;
    const float zeroStartY = -5.5f;
    private GameObject[,] board = new GameObject[boardSize, boardSize];

    private Tuple<int, int> selectedPosition1 = null;
    private Tuple<int, int> selectedPosition2 = null;

    private List<Tuple<int, int>> removedBlocks = new List<Tuple<int, int>>();
    private bool isChange = false;

    void Start()
    {
        // 가능성
        blockManager = FindObjectOfType<BlockManager>();
        FillBoard();
        CheckMatches();
    }

    private void FillBoard()
    {
        blockManager.FillBoard(board, boardSize);
        for(int i = 0; i < boardSize; i++)
        {
            for(int j = 0; j < boardSize; j++)
            {
                float x = zeroStartX + blockInterval * i;
                float y = zeroStartY + blockInterval * j;

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
        removedBlocks.Clear(); // 제거된 블록 정보 초기화

        for (int i = 0; i < boardSize; i++)
        {
            for (int j = 0; j < boardSize; j++)
            {
                // 가로 3개 체크
                if (j + 2 < boardSize &&
                    board[i, j].GetComponent<BaseBlock>().jewelType == board[i, j + 1].GetComponent<BaseBlock>().jewelType &&
                    board[i, j + 1].GetComponent<BaseBlock>().jewelType == board[i, j + 2].GetComponent<BaseBlock>().jewelType)
                {
                    removedBlocks.Add(new Tuple<int, int>(i, j));
                    removedBlocks.Add(new Tuple<int, int>(i, j + 1));
                    removedBlocks.Add(new Tuple<int, int>(i, j + 2));
                }

                // 세로 3개 체크
                if (i + 2 < boardSize &&
                     board[i, j].GetComponent<BaseBlock>().jewelType == board[i + 1, j].GetComponent<BaseBlock>().jewelType &&
                     board[i + 1, j].GetComponent<BaseBlock>().jewelType == board[i + 2, j].GetComponent<BaseBlock>().jewelType)
                {
                    removedBlocks.Add(new Tuple<int, int>(i, j));
                    removedBlocks.Add(new Tuple<int, int>(i + 1, j));
                    removedBlocks.Add(new Tuple<int, int>(i + 2, j));
                }
            }
        }

        // 제거된 블록 처리
        foreach (var pos in removedBlocks)
        {
            RemoveBlocks(pos.Item1, pos.Item2);
        }

        if(isChange)
        {
            ShiftBlocksDown();
            FillEmptySpaces();
            isChange = false;
            CheckMatches();
        }
    }

    void RemoveBlocks(int x, int y)
    {
        board[x, y].SetActive(false);
        isChange = true;
    }

    void ShiftBlocksDown()
    {
        for (int x = 0; x < boardSize; x++)
        {
            for (int y = 0; y < boardSize; y++)
            {
                if (!board[x, y].activeInHierarchy)
                {
                    for (int i = y + 1; i < boardSize; i++)
                    {
                        if (board[x, i].activeInHierarchy)
                        {
                            GameObject temp = board[x, y];
                            board[x, y] = board[x, i];
                            board[x, i] = temp;

                            board[x, y].transform.position = board[x, i].transform.position;

                            board[x, y].SetActive(true);
                            board[x, i].SetActive(false);
                            break;
                        }
                    }
                }
            }
        }
    }

    void FillEmptySpaces()
    {
        for (int x = 0; x < boardSize; x++)
        {
            for (int y = 0; y < boardSize; y++)
            {
                if (!board[x, y].activeInHierarchy)
                {
                    JewelType randomType = (JewelType)UnityEngine.Random.Range((int)JewelType.Red, (int)JewelType.JewelTypeCount);
                    GameObject newBlock = blockManager.GetRandomBlock(randomType);
                    newBlock.transform.position = board[x, y].transform.position;
                    board[x, y] = newBlock;
                    board[x, y].SetActive(true);
                }
            }
        }
    }
}
