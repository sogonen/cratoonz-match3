using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropPool : MonoBehaviour
{
    public Sprite[] dropSprites;
    public GameObject dropPrefab;
    public int poolSize;

    private Queue<GameObject> pool;

    private void Awake()
    {
        // Create the drop pool
        pool = new Queue<GameObject>();
        CreatePool();
    }

    private void CreatePool()
    {
        // Instantiate the drop objects and add them to the pool
        for (int i = 0; i < poolSize; i++)
        {
            GameObject drop = Instantiate(dropPrefab, transform);
            drop.SetActive(false);
            pool.Enqueue(drop);
        }
    }

    public GameObject GetDrop()
    {
        if (pool.Count == 0)
        {
            CreatePool();
        }

        GameObject drop = pool.Dequeue();
        drop.SetActive(true);
        return drop;
    }

    public void ReturnDrop(GameObject drop)
    {
        drop.transform.SetParent(transform);
        drop.SetActive(false);
        pool.Enqueue(drop);
    }
}