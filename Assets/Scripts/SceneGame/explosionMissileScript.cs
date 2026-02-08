using System.Collections;
using UnityEngine;

public class explosionMissileScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(DestroyExplosion());
    }

    private IEnumerator DestroyExplosion()
    {
        for (int i = 0; i < 16; i++)
        {
            yield return new WaitForEndOfFrame();
        }
        Destroy(gameObject);
    }
}
