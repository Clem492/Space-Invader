using UnityEngine;

public class PixelPerfectCollision : MonoBehaviour
{
    public SpriteRenderer shieldSprite; // SpriteRenderer du bouclier 
    private Texture2D shieldTexture;    // Texture associée au sprite du bouclier 
    private Texture2D shieldTextureStart;
    public GameObject maskPrefab;       // prefab du MissileSplash avec son SpriteMask
    public float yOffset = 0;           //Offset verticale en espace monde 

    public int poolSize = 1;
    private GameObject[] maskPool;
    private int currentMaskIndex = 0; //permet de commencer la recherce à cette index 

    private void Start()
    {
        shieldTextureStart = gameObject.GetComponent<SpriteRenderer>().sprite.texture;
        //crée une copie de la texture pour la rendre modifiable 
        shieldTexture = Instantiate(shieldSprite.sprite.texture);

        shieldSprite.sprite = Sprite.Create(shieldTexture, shieldSprite.sprite.rect, new Vector2(0.5f, 0.5f), shieldSprite.sprite.pixelsPerUnit);

        if (!shieldTexture.isReadable)
        {
            Debug.LogError("?? La Texture bouclier doit être lisible !");
        }
        maskPool = new GameObject[poolSize];
        for (int i = 0; i < poolSize; i++)
        {
            maskPool[i] = Instantiate(maskPrefab);
            maskPool[i].SetActive(false);
        }

    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Missile"))
        {
            BoxCollider2D missileCollider = collision.GetComponent<BoxCollider2D>();
            if (missileCollider == null)
            {
                Debug.Log("Le missile n'a pas de collider2D");
                return;
            }
            if (IsPixelHitAndModify(missileCollider, out Vector2 worldImpactPoint, out Vector2 uvImpactPoint))
            {
                //TODO: Pool à créer les gars 
                InstantiateMaskAtPosition(worldImpactPoint);

                //resetMissile
                collision.gameObject.GetComponent<PlayerMissile>()?.ResetMissle();
                collision.gameObject.GetComponent<EnemyMissile>()?.ResetMissle();
            }
        }

        if (collision.CompareTag("enemy"))
        {
            BoxCollider2D enemyCollider = collision.GetComponent<BoxCollider2D>();
            if (enemyCollider == null)
            {
                Debug.Log("L'enemie n'a pas de collider2D");
                return;
            }
            if (IsPixelHitAndModify(enemyCollider, out Vector2 worldImpactPoint, out Vector2 uvImpactPoint))
            {
                //TODO: Pool à créer les gars 
                InstantiateMaskAtPosition(worldImpactPoint);
            }

        }
    }

    private bool IsPixelHitAndModify(BoxCollider2D missileCollider, out Vector2 worldImpactPoint, out Vector2 uvImpactPoint)
    {
        worldImpactPoint = Vector2.zero;
        uvImpactPoint = Vector2.zero;

        //.Bounds vous permet de récupérer les limites d'un collider
        Bounds missileBounds = missileCollider.bounds;

        Vector3 bottomLeft = shieldSprite.transform.InverseTransformPoint(missileBounds.min);
        Vector3 topRight = shieldSprite.transform.InverseTransformPoint(missileBounds.max);

        bottomLeft.y += yOffset;
        topRight.y += yOffset;

        Bounds spriteBounds = shieldSprite.sprite.bounds;

        //prendre en compte les dimensiosn de la tuexture et le rect du sprite
        Rect textureRect = shieldSprite.sprite.textureRect;

        //normaliser les coordonées du missile dans l'espace UV
        float uMin = (bottomLeft.x -spriteBounds.min.x)/ spriteBounds.size.x;
        float vMin = (bottomLeft.y -spriteBounds.min.y)/ spriteBounds.size.y;
        
        float uMax = (topRight.x -spriteBounds.min.x)/ spriteBounds.size.x;
        float vMax = (topRight.y -spriteBounds.min.y)/ spriteBounds.size.y;

        //verifier si les UV du missile sont dans les limites de la texture
        uMin = Mathf.Clamp01(uMin);
        vMin = Mathf.Clamp01(vMin);
        uMax = Mathf.Clamp01(uMax);
        vMax = Mathf.Clamp01(vMax);

        //déterminer le point d'impact UV
        uvImpactPoint = new Vector2((uMin + uMax) / 2, (vMin + vMax) / 2);

        worldImpactPoint = shieldSprite.transform.TransformPoint(new Vector3(spriteBounds.min.x + uvImpactPoint.x * spriteBounds.size.x,
                                                                             spriteBounds.min.y + uvImpactPoint.y * spriteBounds.size.y, 0));

        bool pixelModified = false;

        //Parcourir les piexels touchés par la texture missile
        for (float u = uMin; u <= uMax;u += 1.0f/shieldTexture.width)
        {
            for (float v = vMin; v <= vMax;v += 1.0f/ shieldTexture.height)
            {
                int x = Mathf.FloorToInt(textureRect.x + u * textureRect.width);
                int y = Mathf.FloorToInt(textureRect.y + v * textureRect.height);

                //vérifier si les coordonées de la texture sont valide aka "actif"
                if (x >= 0 && x < shieldTexture.width && y >= 0 && y < shieldTexture.height)
                {
                    //lire la couleur du pixel dans la texture
                    Color pixel = shieldTexture.GetPixel(x, y);

                    if (pixel.a > 0)
                    {
                        shieldTexture.SetPixel(x, y, new Color (0,0,0,0));
                        pixelModified = true;
                    }
                }
            }
        }
        if (pixelModified)
        {
            shieldTexture.Apply();
        }

        return pixelModified;
    }

    private void InstantiateMaskAtPosition(Vector2 wordlPosition)
    {
        //TODO : implémenter un système de pooling
        GameObject maskInstance = Instantiate (maskPrefab, wordlPosition, Quaternion.identity);

        //positionne le masque sur le même z que le bouclier 
        maskInstance.transform.position = new Vector3 (wordlPosition.x, wordlPosition.y, shieldSprite.transform.position.z);


    }

    public void ResetShield()
    {
        GameObject[] goToDestroy = GameObject.FindGameObjectsWithTag("Splash");
        foreach (GameObject go in goToDestroy)
        {
            Destroy (go);
        }

        shieldTexture = Instantiate(shieldTextureStart);

        shieldSprite.sprite = Sprite.Create(shieldTexture, shieldSprite.sprite.rect, new Vector2(0.5f, 0.5f), shieldSprite.sprite.pixelsPerUnit);
    }

}
