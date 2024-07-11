using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.ETC;
using System.Linq;

public class BlockManager : MonoBehaviour
{
    private Dictionary<JewelType, List<GameObject>> blocksByType = new Dictionary<JewelType, List<GameObject>>();
    private const int blocksPullCount = 300;

    public GameObject[] jewelPrefabs;

    void Awake()
    {
        InitializeBlocks();
    }

    private void InitializeBlocks()
    {
        for(int i = 1; i < (int)JewelType.JewelTypeCount; i++)  
        {
            blocksByType[(JewelType)i] = new List<GameObject>();

            for (int j = 0; j < blocksPullCount; j++)
            {
                GameObject block = Instantiate(jewelPrefabs[i - 1]);
                block.SetActive(false);
                AddBlockToList((JewelType)i, block);
            }
        }
    }

    private void AddBlockToList(JewelType type, GameObject block)
    {
        if (!blocksByType.ContainsKey(type))
        {
            blocksByType[type] = new List<GameObject>();
        }
        blocksByType[type].Add(block);
    }

    public GameObject GetRandomBlock(JewelType type)
    {
        if (blocksByType.ContainsKey(type) && blocksByType[type].Count > 0)
        {
            int randomIndex = Random.Range(0, blocksByType[type].Count);
            GameObject randomBlock = blocksByType[type][randomIndex];
            blocksByType[type].RemoveAt(randomIndex);
            randomBlock.SetActive(true);
            return randomBlock;
        }
        else
        {
            return null;
        }
    }

    public void FillBoard(GameObject[,] board, int boardSize)
    {
        for (int i = 0; i < boardSize; i++)
        {
            for (int j = 0; j < boardSize; j++)
            {
                JewelType randomType = (JewelType)Random.Range(1, (int)JewelType.JewelTypeCount);
                GameObject randomBlock = GetRandomBlock(randomType);
                if (randomBlock != null)
                {
                    board[i, j] = randomBlock;
                    randomBlock.transform.position = new Vector3(i, j, 0f);
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
