using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Audio : MonoBehaviour {

    #region Fields
    private AudioSource audioSrc;
    private float musicVolume = 1f;
    #endregion

    #region Unity functions
    private void Awake()
    {
        GameObject[] audios = GameObject.FindGameObjectsWithTag("Audio");

        if (audios.Length > 1)
            Destroy(gameObject);
        else
            DontDestroyOnLoad(transform.gameObject);
        
    }
    
    void Start()
    {
        audioSrc = GetComponent<AudioSource>();
    }
    
    void Update()
    {
        audioSrc.volume = musicVolume;
    }

    #endregion

    #region Volume management
    public void SetVolume(float vol)
    {
        musicVolume = vol;
    }
    #endregion
}
