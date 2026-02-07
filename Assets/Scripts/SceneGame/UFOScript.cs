using System.Collections;
using UnityEngine;

public class UFOScript : MonoBehaviour
{
    private Vector3 spawningPoint1;
    private Vector3 spawningPoint2;

    private bool right;
    private bool left;

    private int UFOSpeed = 3;

    private void Start()
    {
        spawningPoint1 = new Vector3(9.60999966f, 6.57000017f, 1.25903952f);
        spawningPoint2 = new Vector3(-9.10000038f, 6.57000017f, 1.25903952f);
    }

    private void Update()
    {
        /*MoveUFO();*/
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
                Destroy(gameObject);
            }
        }
        if (left)
        {
            transform.transform.Translate(UFOSpeed * Time.deltaTime, 0, 0);
            if (transform.position.x >= spawningPoint1.x)
            {
                Destroy(gameObject);
            }
        }

    }
  
}
