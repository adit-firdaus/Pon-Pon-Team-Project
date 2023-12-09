using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floor : MonoBehaviour
{
    public Dictionary<Vector2, Block> blockMap = new();

    public Block GetBlock(Vector2 position)
    {
        return blockMap[position];
    }
}
