using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageFlash : MonoBehaviour
{
    [Tooltip("How long the sprite will flash to white for (In Seconds)")]
    public float flashTime;
    public Material flashMaterial;
    private SpriteRenderer spriteRenderer;
    public IEnumerator FlashWhite()
    {
        flashMaterial.SetColor("_FlashColor", Color.white);
        flashMaterial.SetFloat("_FlashAmount", 1f);
        yield return new WaitForSeconds(flashTime);
        flashMaterial.SetFloat("_FlashAmount", 0);
    }

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        flashMaterial = new Material(flashMaterial);

        flashMaterial = spriteRenderer.material;
    }
}
