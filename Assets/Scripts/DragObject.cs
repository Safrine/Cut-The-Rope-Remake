using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragObject : MonoBehaviour {

    #region Fields
    //Candy
    [SerializeField]
    private GameObject candy;
    private Dictionary<string, float> springsDistance = new Dictionary<string, float>();

    //Link
    [SerializeField]
    private GameObject linkPrefab;
    private List<GameObject> createdLink;
    private float linkSize;

    //Slide le Hook
    private Vector3 offset;
    private float ratio;

    //Slider
    private Transform parent;
    private Renderer rend;
    private Vector3 size;
    private float width;
    #endregion

    #region Unity functions
    void Start()
    {
        //Recup largeur du slide
        parent = transform.parent;
        rend = parent.GetComponent<Renderer>();
        size = rend.bounds.size;
        width = size.x;

        //Recup taille du link
        Renderer rendLink = linkPrefab.GetComponent<Renderer>();
        Vector3 sizeLink = rendLink.bounds.size;
        linkSize = size.y;

        createdLink = new List<GameObject>();

        //Position du hook sur le slide (0 : tendu; 1 : détendu)
        ratio = (transform.position.x - (parent.position.x - width / 2)) / width;
        CheckSprings();
    }
    
    void Update()
    {
        if(candy)
            CheckSprings();
    }

    void OnMouseDown()
    {
        //offset = object world pos - mouse world pos
        offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    void OnMouseDrag()
    {
        //Placement du hook
        Vector3 tmpNewPos = Camera.main.ScreenToWorldPoint(Input.mousePosition) + offset;
        Vector3 newPos = new Vector3(tmpNewPos.x, transform.position.y, tmpNewPos.z);

        //Depassement droite
        if (tmpNewPos.x >= parent.position.x + width / 2)
            newPos.x = parent.position.x + width / 2;

        //Depassement gauche
        if (tmpNewPos.x <= parent.position.x - width / 2)
            newPos.x = parent.position.x - width / 2;

        transform.position = newPos;

        //Position du hook sur le slide (0 : tendu; 1 : détendu)
        ratio = (transform.position.x - (parent.position.x - width / 2)) / width;
        
        if (transform.Find("SpringAttache"))
            UpdateRopeSpring();

       //UpdateRope();
    }
    #endregion
    
    #region Springs management
    void CheckSprings()
    {
        //Desactiver les ressorts quand les cordes sont détendues
        bool enable = false;
        if (ratio < 0.70)
            enable = true;

        SpringJoint2D[] sj = candy.GetComponents<SpringJoint2D>();
        for (int i = 0; i < sj.Length; i++)
        {
            sj[i].enabled = enable;
        }
    }

    void UpdateRopeSpring()
    {
        if (candy.GetComponents<SpringJoint2D>().Length > 0)
        {
            SpringJoint2D[] sj = candy.GetComponents<SpringJoint2D>();
            for (int i = 0; i < sj.Length; i++)
            {
                //Les autres Ropes
                Rigidbody2D connectedRB = sj[i].connectedBody;
                if (connectedRB.transform.parent.name != transform.name)
                {
                    if (!springsDistance.ContainsKey(connectedRB.transform.parent.name))
                        springsDistance.Add(connectedRB.transform.parent.name, sj[i].distance);

                    sj[i].distance = springsDistance[connectedRB.transform.parent.name] * ratio;
                }
            }
        }
    }
    #endregion

    #region Get & Set
    public float getRatio()
    {
        return ratio;
    }
    #endregion

    #region Function for improvement
    void UpdateRope()
    {
        float distance = Vector2.Distance(transform.position, candy.transform.position);

        Hook hook = GetComponent<Hook>();
        createdLink = hook.getCreatedLinkList();

        int numberOfLinkToPut = Mathf.RoundToInt(distance / linkSize);

        //S'il faut ajouter des link
        if (numberOfLinkToPut > createdLink.Count - 1)
        {
            //Creation link
            GameObject link = Instantiate(linkPrefab, transform);
            HingeJoint2D joint = link.GetComponent<HingeJoint2D>();
            joint.autoConfigureConnectedAnchor = false;
            joint.anchor = new Vector2(0f, (linkSize / 2));
            joint.connectedAnchor = new Vector2(0f, -(linkSize / 4));

            //Joint avec le dernier
            joint.connectedBody = hook.getPreviousRB();
            hook.setPreviousRB(link.GetComponent<Rigidbody2D>());

            //Mise a jour liste des links
            createdLink.Add(link);
            hook.setCreatedLinkList(createdLink);

            //Mise a jour Candy joint
            hook.getCandyJoint().connectedBody = hook.getPreviousRB();

            Debug.Log("IN IF ");
        }

        //Il faut en enlever
        else
        {
            Debug.Log("ELSE ");
        }
    }
    #endregion
}
