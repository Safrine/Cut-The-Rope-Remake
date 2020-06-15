using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    #region Fields
    //Frog "animation"
    [SerializeField]
    private GameObject frog;
    [SerializeField]
    private Sprite closeFrog;
    private SpriteRenderer spriteRenderer;

    //Menu Panels
    [SerializeField]
    private GameObject pauseMenuUI;
    [SerializeField]
    private GameObject winMenuUI;
    [SerializeField]
    private GameObject winMenuLastLevelUI;
    [SerializeField]
    private GameObject lostMenuUI;
    [SerializeField]
    private GameObject pauseButtonUI;
    
    //Check candy out of game
    private GameObject[] candies;
    [SerializeField]
    private GameObject background;
    private Renderer rend;
    private Vector3 size;

    //Audio
    [SerializeField]
    private AudioSource[] winAudio;
    #endregion

    #region Unity functions
    private void Awake()
    {
        Time.timeScale = 1f;
    }
    
    void Start () {
        spriteRenderer = frog.GetComponentInChildren<SpriteRenderer>();
        rend = background.GetComponent<Renderer>();
        size = rend.bounds.size;
    }
	
	void Update () {
        candies = GameObject.FindGameObjectsWithTag("Candy");
        CheckCandyOutOfCamera();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Candy")
            Gagne(collision.gameObject);
    }
    #endregion

    #region Win and Lost management
    //Check si le candy sur le jeu est sorti du terrain
    void CheckCandyOutOfCamera()
    {
        if(candies.Length > 0)
        {
            GameObject candy = candies[0].gameObject;

            if (candy.transform.position.x > background.transform.position.x + size.x/2
                || candy.transform.position.x < background.transform.position.x - size.x/2
                || candy.transform.position.y > background.transform.position.y + size.y/2
                || candy.transform.position.y < background.transform.position.y - size.y/2)
                GameOver(candy);
        }
    }

    //Gagné : on referme la bouche du Frog, on detruit le candy + ouverture du panel associé
    void Gagne(GameObject candy)
    {
        //Sound effect
        winAudio[0].Play();

        //Refermer la bouche
        spriteRenderer.sprite = closeFrog;
        GameObject child = frog.transform.GetChild(0).gameObject;
        child.transform.localScale = new Vector3(10, 10, 10);

        //Detruire bonbon
        Destroy(candy);

        //Load Win menu
        LoadWinMenu();
    }

    //Perdu : detruit le candy + ouverture du panel associé
    public void GameOver(GameObject candy)
    {
        //Sound effect
        winAudio[1].Play();

        //Detruire bonbon
        Destroy(candy);

        //Load Lost menu
        LoadLostMenu();
    }

    #endregion

    #region Panels management
    public void LoadWinMenu()
    {
        //Si dernier level dispo
        if (SceneManager.GetActiveScene().buildIndex == SceneManager.sceneCountInBuildSettings-1)
            winMenuLastLevelUI.SetActive(true);
        else
            winMenuUI.SetActive(true);

        pauseButtonUI.SetActive(false);
        Time.timeScale = 0f;
    }

    public void LoadLostMenu()
    {
        lostMenuUI.SetActive(true);
        pauseButtonUI.SetActive(false);
        Time.timeScale = 0f;
    }

    public void LoadPauseMenu()
    {
        pauseMenuUI.SetActive(true);
        pauseButtonUI.SetActive(false);
        Time.timeScale = 0f;
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        pauseButtonUI.SetActive(true);
        Time.timeScale = 1f;
    }

    public void RetrySameLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    
    public void LoadMainMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void LoadNextLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    #endregion
}
