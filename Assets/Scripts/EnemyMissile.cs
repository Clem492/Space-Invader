using System.Collections;
using UnityEngine;

public class EnemyMissile : MonoBehaviour
{
    public float speed = 10f;
    public float minHeight = 10f;

    

    //Sprite utiliser pour les animations du missle
    [SerializeField] Sprite MissileFrame1, MissileFrame2, MissileFrame3, MissileFrame4;

    private void Start()
    {
        StartCoroutine(DoAnimationMissile());
    }

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

            
            Destroy(collision.gameObject);
            ResetMissle();
            GameManager.Instance.GameOver();
        }

    }

   

    public IEnumerator DoAnimationMissile()
    {
        
        gameObject.GetComponent<SpriteRenderer>().sprite = MissileFrame1;
        while (true)
        {
            for (int i = 0; i < 4; i++)
            {
               
                if (i  == 0)
                {
                    gameObject.GetComponent<SpriteRenderer>().sprite = MissileFrame1;
                }
                if (i == 1)
                {
                    gameObject.GetComponent<SpriteRenderer>().sprite = MissileFrame2;
                }
                if (i == 2)
                {
                    gameObject.GetComponent<SpriteRenderer>().sprite = MissileFrame3;
                }
                else
                {
                    gameObject.GetComponent<SpriteRenderer>().sprite = MissileFrame4;
                }
                for (int j = 0; j < 4; j++)
                {
                    yield return new WaitForEndOfFrame();
                }
            }

        }

    }

}
