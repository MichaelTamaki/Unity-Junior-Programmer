using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridRow : MonoBehaviour
{
    public static readonly int blockWidth = 10;
    private int blockCount = 0;
    private GameObject[] blockObjs = new GameObject[blockWidth];

    public void InitializeBlocks()
    {
        for (int x = 0; x < blockWidth; x++)
        {
            GameObject blockObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            blockObj.transform.SetParent(gameObject.transform, false);
            blockObj.transform.localPosition = new Vector3(x, 0);
            blockObj.transform.localScale = new Vector3(0.92f, 0.92f, 0.92f);
            blockObj.SetActive(false);
            blockObjs[x] = blockObj;
        }
    }

    public bool PlaceBlock(int x, Color color)
    {
        blockCount++;

        GameObject blockObj = blockObjs[x];
        blockObj.SetActive(true);
        blockObj.GetComponent<Renderer>().material.SetColor("_Color", color);

        return blockCount == blockWidth;
    }

    public bool IsBlockActive(int x)
    {
        return blockObjs[x].activeSelf;
    }

    public void ResetBlocks()
    {
        blockCount = 0;
        foreach (GameObject blockObj in blockObjs)
        {
            blockObj.SetActive(false);
        }
    }
}
