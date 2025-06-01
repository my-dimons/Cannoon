using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UiSounds : MonoBehaviour
{
    [Header("Sounds")]
    public AudioSource sound;
    public AudioClip enteringHover;
    public AudioClip exitingHover;
    public AudioClip click;
    
    GameManager gameManager;

    private void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
    }
    public void Click()
    {
        sound.PlayOneShot(click, 1f * gameManager.soundVolume);
    }
    
    public void EnteringHover()
    {
        sound.PlayOneShot(enteringHover, 1f * gameManager.soundVolume);
    }

    public void ExitingHover()
    {
        sound.PlayOneShot(exitingHover, 1f * gameManager.soundVolume);
    }
}
