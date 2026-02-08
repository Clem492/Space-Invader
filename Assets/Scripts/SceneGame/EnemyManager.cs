
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using static UnityEngine.Rendering.DebugUI.Table;

public class EnemyManager : MonoBehaviour
{
    [SerializeField]
    private GameObject player;
    private Vector3 playerPos;
    private float playerBoundaryX;

    public EnemyPool enemyPool;
    public int rows = 5;
    public int cols = 11;
    public float spacing = 1.5f;
    public float _stepDistance = 0.5f;
    public float stepDistanceVertical = 1f;

    public Vector2 startPosition = new Vector2(-6.5f, 7.5f);

    private GameObject[,] enemies;
    private bool isPaused = false;
    

    private enum MoveState { MoveRight, MoveLeft }
    private MoveState currentState = MoveState.MoveRight;

    public GameObject missileAPrefab;
    public GameObject LaserPrefab;
    public GameObject MissileCPrefab;
    public Transform missilePoint;
    public float missileInterval = 2.0f;

    public int explosionDuration = 17;
    public GameObject explosionPrefab;
    private GameObject explosionDurationFrame;

    

    public int remainingEnemies;

    //carre pour la transition
    public GameObject carreInvisible;
    private Vector3 carreStartPos;
    private bool transitingMovement;
    private bool transitingLevel;
    private bool spawning;
    
    public UFOManager UFOManager;

    private bool needSwtichLevel = false;
    private bool firstStart = true;

   
    public GameObject[] tabShield;


    public int poolSize = 3;
    private GameObject[] missilePool;
    private int currentMissileIndex = 0; //permet de commencer la recherce à cette index 
    private Missile missile;



    void Start()
    {
        missile = Missile.MissileAPrefab;
        transitingLevel = false;
        playerBoundaryX = player.GetComponent<PlayerScript>().boundary;
        playerPos = player.transform.position;
        enemies = new GameObject[rows, cols];
        carreStartPos = carreInvisible.transform.position;


        missilePool = new GameObject[poolSize];
        for (int i = 0; i < poolSize; i++)
        {
            if (missile == Missile.MissileAPrefab)
            {
                missilePool[i] = Instantiate(missileAPrefab);

                missile = Missile.LaserPrefab;
            }
            else if (missile == Missile.LaserPrefab)
            {
                Debug.Log("dog");
                missilePool[i] =  Instantiate(LaserPrefab);
                missile = Missile.MissileCPrefab;
            }
            else if (missile == Missile.MissileCPrefab)
            {
                Debug.Log("cat");
                missilePool[i] = Instantiate(MissileCPrefab);
                missile = Missile.MissileAPrefab;
            }
            missilePool[i].SetActive(false);
        }

    }

    private void Update()
    {
        
        MoveCarre();

        

        if (needSwtichLevel && !GameManager.Instance.ufoActive)
        {
            needSwtichLevel = false;
            StartCoroutine(TransitingLevel());
        }
        if (firstStart && GameManager.Instance.currentMenu == GameManager.GameMenu.Game)
        {
            firstStart = false;
            StartCoroutine(DoSpawn());
        }
    }

    private enum Missile
    {
        MissileAPrefab,
        LaserPrefab,
        MissileCPrefab
    }

    

    private IEnumerator DoSpawn()
    {
        StartCoroutine(SpawnEnemies());
        yield return new WaitUntil(() => !spawning);
        StartCoroutine(HandleEnemyMovement());
        StartCoroutine(EnemyShooting());
    }

    private IEnumerator SpawnEnemies()
    {
        spawning = true;
        GameManager.Instance.IsPaused = true;
        var enemyTypes = enemyPool.GetEnemyTypes();

        for (int row = 0; row < rows; row++)
        {
            var enemyType = GetEnemyTypeForRow(row, enemyTypes);
            for (int col = 0; col < cols; col++)
            {
                GameObject enemy = enemyPool.GetEnemy(enemyType.prefab);

                if (enemy != null)
                {
                    float xPos = startPosition.x + (col * spacing);
                    float yPos = startPosition.y - (row * spacing);


                    enemy.transform.position = new Vector3(xPos, yPos, 0);

                    EnemyScript enemyScript = enemy.GetComponent<EnemyScript>();
                    if (enemyScript != null)
                    {
                        enemyScript.EnemyType = enemyType;
                        enemyScript.ScoreData = enemyType.points;
                    }

                    enemies[row, col] = enemy;
                    remainingEnemies++;

                    yield return new WaitForEndOfFrame();

                }
            }
        }
        spawning = false;
        GameManager.Instance.IsPaused = false;
    }

    IEnumerator HandleEnemyMovement()
    {
        while (remainingEnemies >0)
        {

            /*SoundToEnnemieRemaining();*/
            bool boundaryReached = false;

            for (int row = rows - 1; row >= 0; row--)
            {
                for (int col = 0; col < cols; col++)
                {
                    
                    if (enemies[row, col] != null && enemies[row, col].activeSelf)
                    {
                        yield return new WaitUntil(() => !GameManager.Instance.IsPaused && !GameManager.Instance.isExploding);
                        Vector3 direction = currentState == MoveState.MoveRight ? Vector3.right : Vector3.left;
                        
                        MoveEnemy(enemies[row, col], direction, _stepDistance);

                        if (enemies[row, col] == null)
                        {
                            continue;
                        }
                        
                        EnemyScript enemyScript = enemies[row, col].GetComponent<EnemyScript>();
                        if (enemyScript != null) enemyScript.ChangeSprite();

                        if (ReachedBoundery(enemies[row, col])) boundaryReached = true;

                        yield return null;
                    }
                }
            }

            if (boundaryReached)
            {
                yield return MoveAllEnemiesDown();
                currentState = currentState == MoveState.MoveRight ? MoveState.MoveLeft : MoveState.MoveRight;
            }
        }
    }

    IEnumerator MoveAllEnemiesDown()
    {
        for (int row = rows - 1; row >= 0; row--)
        {
            for (int col = 0; col < cols; col++)
            {
                if (enemies[row, col] != null && enemies[row, col].activeSelf)
                {
                    Vector3 direction = Vector3.down;
                    
                    MoveEnemy(enemies[row, col], direction, _stepDistance);
                    
                    yield return null;
                }
            }
        }
    }

    

    IEnumerator EnemyShooting()
    {
       while (true)
        {
            yield return new WaitUntil(() => !GameManager.Instance.IsPaused && !GameManager.Instance.isExploding);

            yield return new WaitForSeconds(Random.Range(missileInterval, missileInterval * 2));
            List<GameObject> shooters = GetBottomEnemies();
            if (shooters.Count > 0 && !GameManager.Instance.IsPaused && !GameManager.Instance.isExploding)
            {
                GameObject shooter = shooters[Random.Range(0, shooters.Count)];

                FireMissile(shooter);
            }
            if (shooters.Count == 0)
            {
                
            }
        }
    }

    private List<GameObject> GetBottomEnemies()
    {
        List<GameObject> bottomEnemies = new List<GameObject>();
        for (int col = 0; col <cols; col++)
        {
            for(int row = rows - 1; row >= 0; row--)
            {
                if (enemies[row, col] != null && enemies[row, col].activeSelf)
                {
                    bottomEnemies.Add(enemies[row, col]);
                    break;
                }
            }
        }
        return bottomEnemies;
    }

    private void FireMissile(GameObject shooter)
    {
        //rechercher le Fire Point dans les enfants de l'enemie
        Transform firePoint = shooter.transform.Find("FirePoint");
        if (firePoint != null)
        {
            //TODO : Implémenter le pool de missile 

            for (int i = 0; i < poolSize; i++)
            {
                int index = (currentMissileIndex + i) % poolSize;

                if (!missilePool[index].activeSelf)
                {
                    missilePool[index].transform.position = firePoint.position;
                    missilePool[index].transform.rotation = firePoint.rotation;
                    missilePool[index].SetActive(true);
                    currentMissileIndex = (index + 1) % poolSize;
                    return; //sortir après avoir trouvé un missile 
                }
            }


        }
        else
        {
            Debug.Log($"FirePoint non trouvé pour l'enemi : {shooter.name}");
        }
    }

    private void MoveEnemy(GameObject enemy, Vector3 direction, float stepDistance)
    {
        if (enemy == null)
        {
            return;
        }

        Vector3 newPosition = enemy.transform.position + direction * stepDistance;

        newPosition.x = Mathf.Round(newPosition.x * 100f) / 100f;
        newPosition.y = Mathf.Round(newPosition.y * 100f) / 100f;
        newPosition.z = Mathf.Round(newPosition.z * 100f) / 100f;

        enemy.transform.position = newPosition;
        if (enemy.transform.position.y <= -9.5f)
        {

            StartCoroutine(GameManager.Instance.GameOver());
        }

    }

    public void ReturnEnemy(GameObject enemy, GameObject prefab)
    {
        for (int row= 0; row < rows; row++)
        {
            for (int col= 0; col < cols; col++)
            {
                if (enemies[row, col] == enemy)
                {
                    enemies[row, col] = null;

                }
            }
        }

        GameManager.Instance.AddScore(enemy.GetComponent<EnemyScript>().ScoreData);
        enemyPool.ReturnToPool(enemy, prefab);
        explosionDurationFrame = Instantiate(explosionPrefab, enemy.transform.position, Quaternion.identity);
        AudioManager.instance.invaderkilled.Play();
        remainingEnemies--;
        if (remainingEnemies <= 0)
        {
            needSwtichLevel = true;
            

        }
        if (!GameManager.Instance.isExploding)
            StartCoroutine(ExplosionCoroutine());
    }

    IEnumerator ExplosionCoroutine()
    {
        GameManager.Instance.isExploding = true;

        int duratin = explosionDuration;
        while (duratin > 0)
        { 
            duratin--;
            yield return new WaitForEndOfFrame();
        }
        GameManager.Instance.isExploding = false;
        Destroy(explosionDurationFrame);
    }

    private bool ReachedBoundery(GameObject enemy)
    {
        float xPos = enemy.transform.position.x;

        if (currentState == MoveState.MoveRight && xPos >= playerBoundaryX)
        {
            return true;
        }

        if (currentState == MoveState.MoveLeft && xPos <= -playerBoundaryX)
        {
            return true;
        }

        return false;
    }

    private EnemyData.EnemyType GetEnemyTypeForRow(int row, List<EnemyData.EnemyType> enemyTypes)
    {
        if (row == 0)
        {
            return enemyTypes[2];
        }
        else if (row <= 2)
        {
            return enemyTypes[1];
        }
        else
        {
            return enemyTypes[0];
        }
    }

    public IEnumerator TransitingLevel()
    {
        transitingLevel = true;
        //attendre 32frame
        for (int i = 0; i < 32; i++)
        {
            yield return new WaitForEndOfFrame();
        }
        //bouger le carée 
        transitingMovement = true;
        yield return new WaitUntil(() => carreInvisible.transform.position.x >= 0);
        player.SetActive(false);
        
        for (int i = 0; i < tabShield.Length; i++)
        {
            tabShield[i].GetComponent<PixelPerfectCollision>().ResetShield();
        }
        yield return new WaitUntil(() => carreInvisible.gameObject.transform.position.x >= 45);
        transitingMovement = false;
        carreInvisible.transform.position = carreStartPos;
        yield return new WaitForEndOfFrame();

        //change de niveau
        GameManager.Instance.NextLevel();
        GameManager.Instance.WhatLevel(ref _stepDistance, ref missileInterval);
        UFOManager.UfoSpawningCondition();
        StartCoroutine(SpawnEnemies());
        //mettre le joueur a la bonne position
        player.transform.position = playerPos;
        transitingLevel = false;

        yield return new WaitUntil(() => !spawning);
        //faire bouger les enemie et le joueur
        StartCoroutine(HandleEnemyMovement());
        player.SetActive(true);
    }

    private void MoveCarre()
    {
        if (GameManager.Instance.currentMenu == GameManager.GameMenu.Game && transitingMovement)
        {
            carreInvisible.transform.Translate(2, 0, 0);
        }
        
    }

 
    private void SoundToEnnemieRemaining()
    {
        if (remainingEnemies <= 55 && remainingEnemies > 42 && !AudioManager.instance.fastInvaders1.isPlaying)
        {
            Debug.Log(1);
            //jouer le premier son 
            AudioManager.instance.fastInvaders4.Stop();
            AudioManager.instance.fastInvaders1.Play();
        }
        else if (remainingEnemies <= 42 && remainingEnemies > 29 && !AudioManager.instance.fastInvaders2.isPlaying)
        {
            Debug.Log(2);
            //jouer le deuxième son
            AudioManager.instance.fastInvaders1.Stop();
            AudioManager.instance.fastInvaders2.Play();
        }
        else if (remainingEnemies <= 29 && remainingEnemies > 16 && !AudioManager.instance.fastInvaders3.isPlaying)
        {
            Debug.Log(3);
            //jouer le troisième son 
            AudioManager.instance.fastInvaders2.Stop();
            AudioManager.instance.fastInvaders3.Play();
        }
        else if (remainingEnemies <= 16 && remainingEnemies > 0 && !AudioManager.instance.fastInvaders4.isPlaying)
        {
            Debug.Log(4);
            //jouer le quatrième son 
            AudioManager.instance.fastInvaders3.Stop();
            AudioManager.instance.fastInvaders4.Play();
        }
    }
    
    //TODO : implémenter le son 
}