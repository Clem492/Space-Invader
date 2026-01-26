using UnityEngine;

public class PlayerMissile : MonoBehaviour
{
    public float speed = 10f;
    public float maxHeight = 10f;

    [SerializeField] private EnemyManager enemyManager;
    [SerializeField] private GameObject explosionEnemi;


    private void Update()
    {
        transform.Translate(Vector3.up *  speed * Time.deltaTime);

        if (transform.position.y > maxHeight) ResetMissle();


 
    
    }

    public void ResetMissle()
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
                GameObject go = collision.GetComponent<EnemyScript>().EnemyType.prefab;
                enemyManager.ReturnEnemy(collision.gameObject, go);


                ResetMissle();
                GameManager.Instance.SaveScore();
                
            }

        }
        
    }
    

}
