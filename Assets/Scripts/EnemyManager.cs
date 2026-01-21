using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField]
    private GameObject player;
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
    private bool isExploding = false;

    private enum MoveState { MoveRight, MoveLeft }
    private MoveState currentState = MoveState.MoveRight;

    public GameObject missilePrefab;
    public Transform missilePoint;
    public float missileInterval = 2.0f;

    void Start()
    {
        playerBoundaryX = player.GetComponent<PlayerScript>().boundary;
        enemies = new GameObject[rows, cols];

        SpawnEnemies();

        StartCoroutine(HandleEnemyMovement());
    }

    private void SpawnEnemies()
    {
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

                    Debug.Log($"[EnemyManager] {enemy.name} est Ã  la position X: {xPos}; Y: {yPos}");

                    enemy.transform.position = new Vector3(xPos, yPos, 0);

                    EnemyScript enemyScript = enemy.GetComponent<EnemyScript>();
                    if (enemyScript != null)
                    {
                        enemyScript.EnemyType = enemyType;
                        enemyScript.ScoreData = enemyType.points;
                    }

                    enemies[row, col] = enemy;
                }
            }
        }
    }

    IEnumerator HandleEnemyMovement()
    {
        while (true)
        {
            yield return new WaitUntil(() => !GameManager.Instance.IsPause && !isExploding);

            bool boundaryReached = false;

            for (int row = rows - 1; row >= 0; row--)
            {
                for (int col = 0; col < cols; col++)
                {
                    if (enemies[row, col] != null && enemies[row, col].activeSelf)
                    {
                        Vector3 direction = currentState == MoveState.MoveRight ? Vector3.right : Vector3.left;

                        MoveEnemy(enemies[row, col], direction, _stepDistance);

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

    private void MoveEnemy(GameObject enemy, Vector3 direction, float stepDistance)
    {
        Vector3 newPosition = enemy.transform.position + direction * stepDistance;

        newPosition.x = Mathf.Round(newPosition.x * 100f) / 100f;
        newPosition.y = Mathf.Round(newPosition.y * 100f) / 100f;
        newPosition.z = Mathf.Round(newPosition.z * 100f) / 100f;

        enemy.transform.position = newPosition;
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
}