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

    void Start()
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

    // Update is called once per frame
    void Update()
    {
        
    }
}
