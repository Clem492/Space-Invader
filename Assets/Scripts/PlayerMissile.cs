using UnityEngine;

public class PlayerMissile : MonoBehaviour
{
    public float speed = 10f;
    public float maxHeight = 10f;

    private void Update()
    {
        transform.Translate(Vector3.up *  speed * Time.deltaTime);

        if (transform.position.y > maxHeight) ResetMissle();


 
    
    }

    private void ResetMissle()
    {
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("enemy"))
        {
            EnemyManager enemyManager = FindFirstObjectByType<EnemyManager>();
            if (enemyManager != null)
            {
                enemyManager.ReturnEnemy(collision.gameObject, collision.gameObject);
                enemyManager.isExploding = true;
                enemyManager.StartCoroutine(enemyManager.TimeWhenTouched());
            }

            ResetMissle();
        }
        
    }

}
