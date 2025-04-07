using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemAssets : MonoBehaviour
{
    public static ItemAssets Instance { get; private set; }

    // Cannons
    public Sprite baseCannonSprite;

    // Cannonballs
    public Sprite baseCannonballSprite;

    public Sprite bouncingCannonballSprite;
    private void Start()
    {
        Instance = this;
    }

}
