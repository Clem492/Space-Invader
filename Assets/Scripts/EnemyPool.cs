using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPool : MonoBehaviour
{
    [SerializeField]
    private EnemyData enemyData;

    private Dictionary<GameObject, Queue<GameObject>> poolDictionary
        = new Dictionary<GameObject, Queue<GameObject>>();

    void Start()
    {
      //initialiser un pool pour chaque type d'ennemie avec des quantités spécifiques
      for (int i = 0; i < enemyData.enemyTypes.Count; i++)
      {
            var enemyType = enemyData.enemyTypes[i];

            if (poolDictionary.ContainsKey(enemyType.prefab))
            {
                Debug.LogWarning($" le prefab {enemyType.prefab.name} pour le type {enemyType.name} "+ $"est déjà dans le pool");
                continue;
            }

            int poolSize = GetPoolSizeForEnemyType(i);

            Queue<GameObject> enemyQueue = new Queue<GameObject>();

            for (int j = 0; j< poolSize; j++)
            {
                GameObject enemy = Instantiate(enemyType.prefab);
                enemy.SetActive(false);
                enemyQueue.Enqueue(enemy);
            }
            poolDictionary.Add(enemyType.prefab, enemyQueue);
      }

    }

    public GameObject GetEnemy(GameObject prefab)
    {
        Debug.Log("j'entre dans le get enemy");
        if (poolDictionary.TryGetValue(prefab, out Queue<GameObject> enemyQueue) && enemyQueue.Count > 0)
        {
            Debug.Log("la condition est chelou mais je rentre");
            GameObject enemy = enemyQueue.Dequeue();
            enemy.SetActive(true);

            return enemy;
        }

        return null;
    }

    public void ReturnToPool(GameObject enemy, GameObject prefab)
    {
        enemy.SetActive(false);
        if (poolDictionary.TryGetValue(prefab, out Queue<GameObject> enemyQueue))
        {
            enemyQueue.Enqueue(enemy);
        }
        else
        {
            Debug.Log("Tentative de retourner un enemie à un pool inexistant !");
        }
    }

    public List<EnemyData.EnemyType> GetEnemyTypes()
    {
        return enemyData.enemyTypes;
    }

    private int GetPoolSizeForEnemyType(int index)
    {
        switch (index)
        {
            case 0: //type A
                return 22;
            case 1: //type B
                return 22;
            case 2: //type C
                return 11;
            default:
                return 0;
        }
    }
    // Update is called once per frame
    void Update()
    {
        Debug.Log(poolDictionary.Count);
    }
}
