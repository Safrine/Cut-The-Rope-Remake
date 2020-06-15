using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Candy : MonoBehaviour {

    #region Fields
    [SerializeField]
    private GameObject gameManager;
    #endregion

    #region Unity functions
    private void Update()
    {
        SpringJoint2D[] springs = GetComponents<SpringJoint2D>();
        for (int i = 0; i < springs.Length; i++)
        {
            if (!springs[i].connectedBody)
            {
                Destroy(springs[i]);
            }

        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Collision avec les pics
        if(collision.gameObject.tag == "Spike")
            gameManager.GetComponent<GameManager>().GameOver(gameObject);
    }
    #endregion
}
