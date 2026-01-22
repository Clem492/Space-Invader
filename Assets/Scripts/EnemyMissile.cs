using UnityEngine;

public class EnemyMissile : MonoBehaviour
{
    public float speed = 10f;
    public float minHeight = 10f;

    private void Update()
    {
        transform.Translate(Vector3.down * speed * Time.deltaTime);

        if (transform.position.y < minHeight) ResetMissle();




    }

    public void ResetMissle()
    {
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            /*EnemyManager enemyManager = FindFirstObjectByType<EnemyManager>();
            if (enemyManager != null)
            {
                enemyManager.ReturnEnemy(collision.gameObject, collision.gameObject);
            }*/

            Destroy(collision.gameObject);
        }

    }

}
