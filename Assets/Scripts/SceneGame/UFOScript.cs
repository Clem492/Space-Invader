using System.Collections;
using UnityEngine;

public class UFOScript : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(DestroyUFO());
    }

    private IEnumerator DestroyUFO()
    {
        yield return new WaitForSeconds(2);
        Destroy(gameObject);
    }
}
