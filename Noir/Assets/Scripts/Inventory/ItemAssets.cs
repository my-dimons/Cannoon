using UnityEngine;

public class ItemAssets : MonoBehaviour
{
    public static ItemAssets Instance { get; private set; }

    [Header("Cannons")]
    // Cannons
    public Sprite baseCannonSprite;
    public GameObject baseCannonPrefab;

    [Header("Cannonballs")]
    // Cannonballs
    public Sprite baseCannonballSprite;
    public GameObject baseCannonballPrefab;

    public Sprite bouncingCannonballSprite;
    public GameObject bouncingCannonballPrefab;

    private void Start()
    {
        Instance = this;
    }

}
