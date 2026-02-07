using TMPro;
using Unity.VisualScripting;
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
        if (collision.CompareTag("ufo"))
        {
            collision.GetComponent<BoxCollider2D>().enabled = false;
            //stoper l'ufo
            UFOScript ufoScript = collision.GetComponent<UFOScript>();
            TextMeshProUGUI ufoText = collision.GetComponentInChildren<TextMeshProUGUI>();
            ufoScript.right = true;
            ufoScript.left = true;
            //afficher et ajouter le score de l'ufo
            //détruire l'ufo 
            if (GameManager.Instance.compteurUfoScore == 1)
            {
                GameManager.Instance.AddScore(300);
                ufoScript.StartCoroutine(ufoScript.DestroyUFO(300));
                GameManager.Instance.SaveScore();
            }
            else if (GameManager.Instance.compteurUfoScore == 2)
            {
                GameManager.Instance.AddScore(150);
                ufoScript.StartCoroutine(ufoScript.DestroyUFO(150));
                GameManager.Instance.SaveScore();
            }
            else if (GameManager.Instance.compteurUfoScore == 3)
            {
                GameManager.Instance.AddScore(100);
                ufoScript.StartCoroutine(ufoScript.DestroyUFO(100));
                GameManager.Instance.SaveScore();
            }
            else if (GameManager.Instance.compteurUfoScore > 3)
            {
                GameManager.Instance.AddScore(50);
                ufoScript.StartCoroutine(ufoScript.DestroyUFO(50));
                GameManager.Instance.SaveScore();
            }

            ResetMissle();
        }
    } 

}
