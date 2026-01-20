
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    #region
    [SerializeField]
    private GameObject player;
    private float playerBoundaryX;

    public EnemyPool enemyPool;
    public int rows = 5;//nb de rangées.
    public int columns = 11;//nombre de colonnes.
    public float spacing = 1.5f; //espacement entre les enemeies.
    public float stepDistance = 0.5f; //distance de déplacement par frame;
    public float stepDistanceVertical = 1f; //Distance de déplacement verticale par frame.

    public Vector2 StartPosition = new Vector2(-6.5f, 7.5f);

    private GameObject[,] enemies;
    private bool isPaused = false;
    private bool isExploding = false;

    private enum MoveState {MoveRight, MoveLeft}
    private MoveState currentState = MoveState.MoveRight;

    public GameObject missilePrefab;
    public Transform missilePoint;
    public float missileIntervale = 2.0f; //Intervalle minimum entre les tirs 
    #endregion
    private void Start()
    {
        playerBoundaryX = player.GetComponent<PlayerScript>().boundary;
        enemies = new GameObject[rows, columns];
        SpawnEnemies();
    }

    private void SpawnEnemies()
    {
        var enemyTypes = enemyPool.GetEnemyTypes();
        
        for (int row = 0; row < rows; row++)
        {
            Debug.Log("le premier for fonctionne ");
            var type = GetEnemyTypeForRow(row, enemyTypes);

            for (int col = 0; col < columns; col++)
            {
                Debug.Log("le deuxième for fonctionne aussi ");
                GameObject enemy = enemyPool.GetEnemy(type.prefab);
                if (enemy != null)
                {
                    Debug.Log("enemy n'est pas null");
                    float xPos = StartPosition.x +(col* spacing);
                    float yPos = StartPosition.y -(row* spacing);

                    

                    enemy.transform.position = new Vector3(xPos, yPos, 0);
                    EnemyScript enemyScript = enemy.GetComponent<EnemyScript>();
                    if (enemyScript != null)
                    {
                        Debug.Log("enemyScript n'est pas null");
                        enemyScript.EnemyType = type;
                        enemyScript.ScoreData = type.points;
                    }
                }
            }
        }
    }

    private EnemyData.EnemyType GetEnemyTypeForRow(int row, List<EnemyData.EnemyType> enemyTypes)
    {
        if (row == 0) //type C
        {
            return enemyTypes[2];

        }
        else if (row <=2) //type B
        {
            return enemyTypes[1];
        }
        else //type C
        {
            return enemyTypes[0];
        }
    }
}
