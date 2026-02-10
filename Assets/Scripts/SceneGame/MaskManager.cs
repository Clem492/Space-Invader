using System.Collections.Generic;
using UnityEngine;

public class MaskManager : MonoBehaviour
{
    public static MaskManager instance;
    [SerializeField]
    private GameObject enemyMask_prefab, playerMask_prefab;

    private int enemyMaskPoolSize = 100;
    private int playerMaskPoolSize = 100;

    private Dictionary<GameObject, Queue<GameObject>> maskPoolDictionary = new Dictionary<GameObject, Queue<GameObject>>();

    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else Destroy(gameObject);

        Queue<GameObject> maskQueue = new Queue<GameObject>();

        // Instantiation de tous les masks ennemis
        for (int i = 0; i < enemyMaskPoolSize; i++)
        {
            GameObject mask = Instantiate(enemyMask_prefab);
            mask.SetActive(false);
            maskQueue.Enqueue(mask);
        }
        maskPoolDictionary.Add(enemyMask_prefab, maskQueue);

        maskQueue = new Queue<GameObject>();

        // Instantiation de tous les masks players
        for (int i = 0; i < playerMaskPoolSize; i++)
        {
            GameObject mask = Instantiate(playerMask_prefab);
            mask.SetActive(false);
            maskQueue.Enqueue(mask);
        }
        maskPoolDictionary.Add(playerMask_prefab, maskQueue);
    }

    public GameObject GetMask(GameObject prefab)
    {
        if (maskPoolDictionary.TryGetValue(prefab, out Queue<GameObject> maskQueue) && maskQueue.Count > 0)
        {
            GameObject mask = maskQueue.Dequeue();
            mask.SetActive(true);
            return mask;
        }
        Debug.Log("je n'ai pas trouver le mask");
        return null;
    }

    public void ReturnMask(GameObject mask, GameObject prefab)
    {
        mask.SetActive(false);

        if (mask.name.Contains(enemyMask_prefab.name))
        {
            if (maskPoolDictionary.TryGetValue(enemyMask_prefab, out var enemyMaskQueue))
            {
                enemyMaskQueue.Enqueue(mask);
            }
        }
        else if (mask.name.Contains(playerMask_prefab.name))
        {
            if (maskPoolDictionary.TryGetValue(playerMask_prefab, out var playerMaskQueue))
            {
                playerMaskQueue.Enqueue(mask);
            }
        }
        else
        {
            Debug.Log("Tentative de retourner un ennemi à un pool inexistant !");
        }

        //if (maskPoolDictionary.TryGetValue(prefab, out var maskQueue))
        //{
        //    maskQueue.Enqueue(mask);
        //}
        //else
        //{
        //    Debug.Log("Tentative de retourner un ennemi à un pool inexistant !");
        //}
    }
}