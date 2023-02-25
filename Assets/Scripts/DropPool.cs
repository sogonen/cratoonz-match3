using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DropPool : MonoBehaviour
{
    public Sprite[] dropSprites;
    public GameObject dropPrefab;
    public int poolSize = 100;

    private List<GameObject> dropPool;

    void Awake()
    {
        CreatePool();
    }

    void CreatePool()
    {
        dropPool = new List<GameObject>();
        GameObject drop = Instantiate(dropPrefab, transform);
        drop.SetActive(false);
        dropPool.Add(drop);

    }

    public GameObject GetDrop(int dropType)
    {
        for (int i = 0; i < dropPool.Count; i++)
        {
            if (!dropPool[i].activeInHierarchy)
            {
                dropPool[i].SetActive(true);
                return dropPool[i];
            }
        }
        GameObject dropObject = Instantiate(dropPrefab, transform);
        dropPool.Add(dropObject);
        return dropObject;
    }

    public void ReturnDrop(GameObject drop)
    {
        drop.transform.SetParent(this.transform);
        drop.SetActive(false);
    }
}