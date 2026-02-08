using System.Collections;
using TMPro;
using UnityEngine;

public class UFOScript : MonoBehaviour
{
    [SerializeField] private Sprite UFOExplosion;
    [SerializeField] private Sprite UFOSprite;

    private Vector3 spawningPoint1;
    private Vector3 spawningPoint2;

    public bool right;
    public bool left;
    public bool exploding; 

    private int UFOSpeed = 3;

    private TextMeshProUGUI ufoText;

    private void Start()
    {
        ufoText = GetComponentInChildren<TextMeshProUGUI>();
        spawningPoint1 = new Vector3(9.60999966f, 6.57000017f, 1.25903952f);
        spawningPoint2 = new Vector3(-9.10000038f, 6.57000017f, 1.25903952f);
        
    }

    private void Update()
    {
        MoveUFO();
    }

    //faire le mouvement 
    private void MoveUFO()
    {
        //regarder de quel côté l'ufo a spawn
        //si la position est plus grande que 0 il va a gauche sinon il va a droite
        if (transform.position.x > 0 && ! left)
        {
            right = true;
            
        }
        if (transform.position.x < 0 && !right)
        {
            left = true;
            
        }

        if (right)
        {
            transform.Translate(-UFOSpeed * Time.deltaTime, 0, 0);
            if (transform.position.x <= spawningPoint2.x)
            {
                GameManager.Instance.ufoActive = false;
                right = false;
                gameObject.SetActive(false);
                AudioManager.instance.ufoHightpich.Stop();

            }
        }
        if (left)
        {
            transform.transform.Translate(UFOSpeed * Time.deltaTime, 0, 0);
            if (transform.position.x >= spawningPoint1.x)
            {
                GameManager.Instance.ufoActive = false;
                left = false;
                gameObject.SetActive(false);
                AudioManager.instance.ufoHightpich.Stop();

            }
        }

    }

    public IEnumerator DestroyUFO(int score)
    {
        AudioManager.instance.ufoHightpich.Stop();
        AudioManager.instance.ufoLowpitch.Play();
        exploding = true;
        GameManager.Instance.ufoActive = false;
        gameObject.GetComponent<SpriteRenderer>().sprite = UFOExplosion;
        for (int i = 0; i < 22; i++)
        {
            yield return new WaitForEndOfFrame();
        }
        exploding = false;
        gameObject.GetComponent<SpriteRenderer>().enabled = false;
        ufoText.text = score.ToString();

        for (int i = 0; i < 73; i++)
        {
            yield return new WaitForEndOfFrame();

        }
        right = false;
        left = false;
        gameObject.SetActive(false);
        AudioManager.instance.ufoLowpitch.Stop();
        gameObject.GetComponent<SpriteRenderer>().enabled = true;
        gameObject.GetComponent<BoxCollider2D>().enabled = true;
        gameObject.GetComponent<SpriteRenderer>().sprite = UFOSprite;
        ufoText.text = "";
    }
  
}
