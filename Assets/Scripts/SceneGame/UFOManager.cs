using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UFOManager : MonoBehaviour
{
    //Compter le nombre de tire en fonction du niveau faire apparaitre l'ufo
    //donner le bon nombre de points 
    public GameObject ufoPrefab;
    public GameObject ufoSpawningPoint1;
    public GameObject ufoSpawningPoint2;


    public int compteurTirUfo = 0;
    public int compteurUfoScore = 0;

    
    private Queue<int> shootRequiredToSpawn = new Queue<int>();

    private void Start()
    {
        UfoSpawningCondition();
    }

    private void Update()
    {
        SpawnUfo();
        if (!GameManager.Instance.ufoActive)
        {
            compteurUfoScore = 0;
        }
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

    private void SpawnUfo()
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
                    Instantiate(ufoPrefab, ufoSpawningPoint1.transform.position, Quaternion.identity);
                    GameManager.Instance.ufoActive = true;
                }
                else if (random == 1)
                {
                    Instantiate(ufoPrefab, ufoSpawningPoint2.transform.position, Quaternion.identity);
                    GameManager.Instance.ufoActive = true;
                }

            }
        }
        catch
        {
            Debug.Log("shootRequiredToSpawn est vide");
            compteurTirUfo = 0;
        }
        
       
    }
}
