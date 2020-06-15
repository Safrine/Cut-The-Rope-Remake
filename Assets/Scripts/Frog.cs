using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Frog : MonoBehaviour {

    #region Fields
    [SerializeField]
    private Sprite openFrog;
    [SerializeField]
    private Sprite closeFrog;

    private GameObject child;
    private SpriteRenderer spriteRenderer;

    private AudioSource openMouthSoundEffect;

    #endregion

    #region Unity functions
    // Use this for initialization
    void Start () {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        child = transform.GetChild(0).gameObject;

        openMouthSoundEffect = GetComponent<AudioSource>();

    }
	
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Candy")
        {
            openMouthSoundEffect.Play();

            spriteRenderer.sprite = openFrog;
            child.transform.localScale = new Vector3(18, 18, 18);
        }
            
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Candy")
        {
            spriteRenderer.sprite = closeFrog;
            child.transform.localScale = new Vector3(10, 10, 10);
        }
            
    }
    #endregion
}
