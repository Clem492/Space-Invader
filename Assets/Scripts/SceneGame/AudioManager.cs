using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource shoot, ufoHightpich, ufoLowpitch, invaderkilled, fastInvaders4, fastInvaders3, fastInvaders2, fastInvaders1, explosion, spaceInvaders1;
    public static AudioManager instance;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }


}
