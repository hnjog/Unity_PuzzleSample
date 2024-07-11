using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.ETC;

public class BaseBlock : MonoBehaviour
{
    [SerializeField]
    protected JewelType blockJewelType = JewelType.None;

    [SerializeField]
    protected const byte blockPoint = 100;

    public JewelType jewelType
    {
        get { return blockJewelType; }
        set
        {
            if (value > JewelType.JewelTypeCount)
                value = JewelType.None;
            else if (value < JewelType.None)
                value = JewelType.None;

            blockJewelType = value;
        }
    }

    public byte Point
    { get { return blockPoint; } set { } }

}
