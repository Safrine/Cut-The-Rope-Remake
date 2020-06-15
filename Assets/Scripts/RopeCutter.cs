using System.Collections.Generic;
using UnityEngine;

public class RopeCutter : MonoBehaviour
{
    #region Fields
    [SerializeField]
    private GameObject candy;

    private List<GameObject> touchedHooks;
    #endregion

    #region Unity functions

    private void Start()
    {
        touchedHooks = new List<GameObject>();
    }

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit.collider != null)
                //On touche un link (la corde)
                if (hit.collider.tag == "Link")
                {
                    //Enlever le spring
                    GameObject hittedLink = hit.collider.gameObject;
                    GameObject parentRope = hittedLink.transform.parent.gameObject;

                    //Permet de ne couper qu'une fois une corde
                    if (touchedHooks.Contains(parentRope))
                        return;
                    touchedHooks.Add(parentRope);

                    Hook hook = parentRope.GetComponent<Hook>();

                    SpringJoint2D[] springs = candy.GetComponents<SpringJoint2D>();
                    for (int i = 0; i < springs.Length; i++)
                    {
                        if (springs[i].isActiveAndEnabled)
                        {
                            if (springs[i].connectedBody.transform.parent.name == parentRope.name)
                            {
                                Destroy(springs[i]);
                                break;
                            }
                        }
                        
                    }

                    //Enlever l'attache du Hook
                    if (parentRope.transform.Find("SpringAttache"))
                        Destroy(parentRope.transform.Find("SpringAttache").gameObject);

                    //Couper la corde
                    hook.updateCreateLinkList(hit.collider.gameObject);

                    //Enlever le link pour briser la chaine
                    Destroy(hit.collider.gameObject);
                    
                }
        }

        if(candy)
            CheckRemainingSpring();
    }
    #endregion

    #region Spring management
    //S'il reste qu'un Hook, on lui retire son SpringJoint
    void CheckRemainingSpring()
    {
        SpringJoint2D[] springs = candy.GetComponents<SpringJoint2D>();
        if(springs.Length == 1)
            Destroy(springs[0]);
    }
    #endregion
}
