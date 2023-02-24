using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropPool : MonoBehaviour
{
    public Sprite[] dropSprites;
    public GameObject dropPrefab;
    public int poolSize = 100;

    private List<GameObject>[] dropPool;

    void Awake()
    {
        CreatePool();
    }

    void CreatePool()
    {
        dropPool = new List<GameObject>[dropSprites.Length];
        for (int i = 0; i < dropSprites.Length; i++)
        {
            dropPool[i] = new List<GameObject>();
            for (int j = 0; j < poolSize; j++)
            {
                GameObject drop = Instantiate(dropPrefab, transform);
                drop.SetActive(false);
                dropPool[i].Add(drop);
            }
        }
    }

    public GameObject GetDrop(int dropType)
    {
        for (int i = 0; i < dropPool[dropType].Count; i++)
        {
            if (!dropPool[dropType][i].activeInHierarchy)
            {
                dropPool[dropType][i].SetActive(true);
                return dropPool[dropType][i];
            }
        }
        GameObject dropObject = Instantiate(dropPrefab, transform);
        dropPool[dropType].Add(dropObject);
        return dropObject;
    }

    public void ReturnDrop(GameObject drop)
    {
        drop.transform.SetParent(this.transform);
        drop.SetActive(false);
    }
}