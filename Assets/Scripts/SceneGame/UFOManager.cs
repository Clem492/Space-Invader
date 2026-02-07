using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UFOManager : MonoBehaviour
{
    //Compter le nombre de tire en fonction du niveau faire apparaitre l'ufo
    //donner le bon nombre de points 
    public GameObject ufoPrefab;
    private GameObject[] ufoPool;
    public GameObject ufoSpawningPoint1;
    public GameObject ufoSpawningPoint2;

    public int poolSize = 1;
    public int compteurTirUfo = 0;
    private int currentUFOIndex = 0;


    public Queue<int> shootRequiredToSpawn = new Queue<int>();

    private void Start()
    {
        UfoSpawningCondition();
        ufoPool = new GameObject[poolSize];
        for (int i = 0; i < poolSize; i++)
        {
            ufoPool[i] = Instantiate(ufoPrefab);
            ufoPool[i].SetActive(false);
            
        }

    }

    private void Update()
    {
        

    }

    //fonction qui me donne chaque possibilité de spawn pour l'ufo 
    public void UfoSpawningCondition()
    {
        if (GameManager.Instance.currentLevel == GameManager.Level.level1)
        {
            shootRequiredToSpawn.Clear();
            shootRequiredToSpawn.Enqueue(22);
            for (int i = 0; i< 5; i++)
            {
                shootRequiredToSpawn.Enqueue(14);
            }
            
            for (int i = 0; i < shootRequiredToSpawn.Count; i++)
            {
                Debug.Log(shootRequiredToSpawn.ElementAt(i));
            }
           
        }
        if (GameManager.Instance.currentLevel == GameManager.Level.level2)
        {
            shootRequiredToSpawn.Clear();
            shootRequiredToSpawn.Enqueue(22);
            for (int i = 0; i < 3; i++)
            {
                shootRequiredToSpawn.Enqueue(14);
            }
            for (int i = 0; i < shootRequiredToSpawn.Count; i++)
            {
                Debug.Log(shootRequiredToSpawn.ElementAt(i));
            }
        }
        if (GameManager.Instance.currentLevel == GameManager.Level.level3)
        {
            shootRequiredToSpawn.Clear();
            shootRequiredToSpawn.Enqueue(22);
            for (int i = 0; i < 3; i++)
            {
                shootRequiredToSpawn.Enqueue(14);
            }
            for (int i = 0; i < shootRequiredToSpawn.Count; i++)
            {
                Debug.Log(shootRequiredToSpawn.ElementAt(i));
            }
        }
        if (GameManager.Instance.currentLevel == GameManager.Level.level4)
        {
            shootRequiredToSpawn.Clear();
            shootRequiredToSpawn.Enqueue(22);
            shootRequiredToSpawn.Enqueue(29);
            shootRequiredToSpawn.Enqueue(14);
            for (int i = 0; i < shootRequiredToSpawn.Count; i++)
            {
                Debug.Log(shootRequiredToSpawn.ElementAt(i));
            }
        }
        
    }

    public void SpawnUfo()
    {
        

        for (int i = 0; i < poolSize; i++)
        {
            int index = (currentUFOIndex + i) % poolSize;

            if (!ufoPool[index].activeSelf)
            {

                try
                {
                    if (compteurTirUfo == shootRequiredToSpawn.Peek())
                    {
                        shootRequiredToSpawn.Dequeue();
                        compteurTirUfo = 0;
                        int random = Random.Range(0, 2);
                        if (random == 0)
                        {
                            GameManager.Instance.ufoActive = true;
                            ufoPool[index].transform.position = ufoSpawningPoint1.transform.position;
                            ufoPool[index].transform.rotation = Quaternion.identity;
                            ufoPool[index].SetActive(true);
                        }
                        else if (random == 1)
                        {
                            GameManager.Instance.ufoActive = true;
                            ufoPool[index].transform.position = ufoSpawningPoint2.transform.position;
                            ufoPool[index].transform.rotation = Quaternion.identity;
                            ufoPool[index].SetActive(true);
                        }
                    }
                }
                catch
                {
                    Debug.Log("shootRequiredToSpawn est vide");
                    compteurTirUfo = 0;
                }


                currentUFOIndex = (index + 1) % poolSize;


                return; //sortir après avoir trouvé un missile 
            }
        }

    }
}
