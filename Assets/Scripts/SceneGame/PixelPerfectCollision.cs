using UnityEngine;

public class PixelPerfectCollision : MonoBehaviour
{
    public SpriteRenderer shieldSprite; // SpriteRenderer du bouclier
    private Texture2D shieldTexture;    // Texture associée au sprite du bouclier
    public Texture2D originalShieldTexture; // Texture originale du bouclier
    public GameObject enemyMaskPrefab;       // Prefab enemy du MissileSplash SpriteMask
    public GameObject playerMaskPrefab;       // Prefab player du MissileSplash SpriteMask
    public float yOffset = -0.5f;          // Offset vertical en espace monde
    public int projectileLayer;

    private void Start()
    {
        // Crée une copie de la texture pour la rendre modifiable
        shieldTexture = Instantiate(shieldSprite.sprite.texture);

        shieldSprite.sprite = Sprite.Create(shieldTexture, shieldSprite.sprite.rect, new Vector2(0.5f, 0.5f), shieldSprite.sprite.pixelsPerUnit);

        if (!shieldTexture.isReadable)
        {
            Debug.LogError("Doesn't have a a texture");
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.layer == projectileLayer || collision.CompareTag("EnemyMissile"))
        {
            BoxCollider2D missileCollider = collision.GetComponent<BoxCollider2D>();
            if (missileCollider == null)
            {
                Debug.Log("Le projectile n'a pas de Collider2D");
                return;
            }

            if (IsPixelHitAndModify(missileCollider, out Vector2 worldImpactPoint, out Vector2 uvImpactPoint))
            {
                InstantiateMaskAtPosition(worldImpactPoint, missileCollider.tag);

                // ResetMissile
                collision.gameObject.GetComponent<PlayerMissile>()?.ResetMissle();
                collision.gameObject.GetComponent<EnemyMissile>()?.ResetMissle();
            }
        }
        else if (collision.tag == "Enemy")
        {
            BoxCollider2D enemyCollider = collision.GetComponent<BoxCollider2D>();
            if (IsPixelHitAndModify(enemyCollider, out Vector2 worldImpactPoint, out Vector2 uvImpactPoint))
            {
                InstantiateMaskAtPosition(collision.transform.position, "Enemy");
            }
        }
        else
        {
            Debug.Log("pas le bon layer");
        }
    }

    private bool IsPixelHitAndModify(BoxCollider2D missileCollider, out Vector2 worldImpactPoint, out Vector2 uvImpactPoint)
    {
        worldImpactPoint = Vector2.zero;
        uvImpactPoint = Vector2.zero;

        // .bounds vous permet de récupérer les limites d'un collider
        Bounds missileBounds = missileCollider.bounds;

        Vector3 bottomLeft = shieldSprite.transform.InverseTransformPoint(missileBounds.min);

        Vector3 topRight = shieldSprite.transform.InverseTransformPoint(missileBounds.max);

        bottomLeft.y += yOffset;
        topRight.y += yOffset;

        Bounds spriteBounds = shieldSprite.sprite.bounds;

        // Prendre en compte les dimensions de la texture et le rect du sprite
        Rect textureRect = shieldSprite.sprite.textureRect;

        // Normaliser les coordonnées du missile dans l'espace UV
        float uMin = (bottomLeft.x - spriteBounds.min.x) / spriteBounds.size.x;
        float vMin = (bottomLeft.y - spriteBounds.min.y) / spriteBounds.size.y;
        float uMax = (topRight.x - spriteBounds.min.x) / spriteBounds.size.x;
        float vMax = (topRight.y - spriteBounds.min.y) / spriteBounds.size.y;

        // Vérifier si les UV u missile sont des les limites de la texture
        uMin = Mathf.Clamp01(uMin);
        vMin = Mathf.Clamp01(vMin);
        uMax = Mathf.Clamp01(uMax);
        vMax = Mathf.Clamp01(vMax);

        // Déterminer le point d'impact UV
        uvImpactPoint = new Vector2((uMin + uMax) / 2f, (vMin + vMax) / 2f);

        worldImpactPoint = shieldSprite.transform.TransformPoint(
            new Vector3(
                spriteBounds.min.x + uvImpactPoint.x * spriteBounds.size.x,
                spriteBounds.min.y + uvImpactPoint.y * spriteBounds.size.y,
                0
            )
        );

        bool pixelModified = false;

        // Parcourir les pixels touchés par la texture missile
        for (float u = uMin; u <= uMax; u += 1.0f / shieldTexture.width)
        {
            for (float v = vMin; v <= vMax; v += 1.0f / shieldTexture.height)
            {
                int x = Mathf.FloorToInt(textureRect.x + u * textureRect.width);
                int y = Mathf.FloorToInt(textureRect.y + v * textureRect.height);

                // Vérifier si les coordonées de la texture sont valide aka "actif"
                if (x >= 0 && x < shieldTexture.width && y >= 0 && y < shieldTexture.height)
                {
                    // Lire la couleur du pixel dans la texture
                    Color pixel = shieldTexture.GetPixel(x, y);

                    if (pixel.a > 0)
                    {
                        shieldTexture.SetPixel(x, y, new Color(0, 0, 0, 0));
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

    private void InstantiateMaskAtPosition(Vector2 worldPosition, string tag)
    {
        // Choix du prefab en fonction du tireur
        GameObject maskPrefab = tag == "PlayerMissile" ? playerMaskPrefab : enemyMaskPrefab;
        //GameObject maskInstance = Instantiate(maskPrefab, worldPosition, Quaternion.identity);
        GameObject maskInstance = MaskManager.instance.GetMask(maskPrefab);

        // Donne la taille en fonction du projectile tiré
        Vector3 scale = Vector3.one * 0.5f;
        switch (maskInstance.GetComponent<EnemyMissile>()?.name)
        {
            case "B":
                scale *= 1.25f;
                break;
            case "A":
                break;
            case "C":
                scale *= 0.75f;
                break;
            
        }
        if (tag == "enemy")
        {
            scale *= 3;
        }
        maskInstance.transform.localScale = scale;

        // Positionne le mask sur le même layer Z que le bouclier
        maskInstance.transform.position = new Vector3(worldPosition.x, tag == "PlayerMissile" ? worldPosition.y + -0.17f : worldPosition.y, shieldSprite.transform.position.z);
    }

    public void ResetShield()
    {
        shieldSprite.sprite = Sprite.Create(originalShieldTexture, shieldSprite.sprite.rect, new Vector2(0.5f, 0.5f), shieldSprite.sprite.pixelsPerUnit);
    }
}