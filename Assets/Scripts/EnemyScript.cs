using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    public EnemyData.EnemyType EnemyType;
    public int ScoreData;

    public Sprite sprite01;
    public Sprite sprite02;
    private SpriteRenderer spriteRenderer;
    private bool isSprite01;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null )
        {
            spriteRenderer.sprite = sprite01;
            isSprite01 = true;
        }
        else
        {
            Debug.Log("[EnemyScript] SpriteRenderer is not assigned");
        }
    }
    private void ChangeSprite()
    {
        isSprite01 = !isSprite01;
        // spriteRenderer.sprite = (condition) ? (si vrai) : (si faux)
        spriteRenderer.sprite = isSprite01 ? sprite01 : sprite02;
    }
}
