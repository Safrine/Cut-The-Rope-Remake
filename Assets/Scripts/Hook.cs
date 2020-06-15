using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hook : MonoBehaviour
{
    #region Fields
    //Candy
    [SerializeField]
    private GameObject candy;
    private bool isCandyDead = false;

    //Hook
    [SerializeField]
    private Rigidbody2D hook;

    //Rope
    [SerializeField]
    private GameObject linkPrefab;
    [SerializeField]
    private int numberOfLinks;
    [SerializeField]
    private Material ropeMat;

    private List<LineRenderer> lineRenderers;
    private List<GameObject> createdLink;
    private List<List<GameObject>> linkList;

    private Rigidbody2D previousRB;

    private Vector3 linkSize;
    private HingeJoint2D joint;
    
    private bool isRopeCut = false;
    #endregion

    #region Unity functions
    void Start()
    {
        //Rope
        linkList = new List<List<GameObject>>();
        createdLink = new List<GameObject>();
        lineRenderers = new List<LineRenderer>();

        foreach(Transform t in transform)
            if(t.name == "LineDrawer")
                lineRenderers.Add(t.GetComponent<LineRenderer>());

        Renderer rend = linkPrefab.GetComponent<Renderer>();
        linkSize = rend.bounds.size;
        
        previousRB = hook;
        
        GenerateRope();
        
    }

    void Update()
    {
        //Si candy destroy 
        if (!candy)
            isCandyDead = true;

        //Si possibilité de couper
        if (!candy && !isCandyDead && !isRopeCut)
        {
            updateCreateLinkList(linkList[1][linkList[1].Count - 1]);
        }

        //Si corde disparu
        if (lineRenderers.Count > 0 && lineRenderers[0].material.color.a <= 0f)
        {
            StopCoroutine("FadeOut");
            StopCoroutine("FadeOut2");

            //Destroy lineRenderer
            lineRenderers.Clear();

            //Destroy les links
            foreach (Transform t in transform)
                if (t.tag == "Link")
                    Destroy(t.gameObject);
        }

        else
        {
            RenderLine();
        }
            
    }
    #endregion

    #region Rope management
    //Place des liens les uns collés aux autres
    void GenerateRope()
    {
        for (int i = 0; i < numberOfLinks; i++)
        {
            CreateLink();
            linkList.Add(createdLink);

            //Connecte le dernier lien avec le candy
            if (i == numberOfLinks - 1)
                ConnectRopeEnd();
        }
    }

    //Place les links
    void CreateLink()
    {
        GameObject link = Instantiate(linkPrefab, transform);
        float height = linkSize.y;

        HingeJoint2D joint = link.GetComponent<HingeJoint2D>();
        joint.autoConfigureConnectedAnchor = false;
        joint.anchor = new Vector2(0f, (height / 2));
        joint.connectedAnchor = new Vector2(0f, -(height / 4));

        joint.connectedBody = previousRB;
        previousRB = link.GetComponent<Rigidbody2D>();

        createdLink.Add(link);
    }

    //Connecte le dernier avec le candy
    public void ConnectRopeEnd()
    {
        Renderer rend = candy.GetComponent<Renderer>();
        Vector3 size = rend.bounds.size;
        float height = size.y;
        
        joint = candy.AddComponent<HingeJoint2D>();
        joint.autoConfigureConnectedAnchor = false;
        joint.anchor = Vector2.zero;
        joint.connectedAnchor = new Vector2(0f, -height / 2);
        joint.connectedBody = previousRB;

    }
    
    //Draw the rope
    void RenderLine()
    {
        for (int i = 0; i < lineRenderers.Count; i++)
        {
            lineRenderers[i].positionCount = linkList[i].Count;

            for (int j = 0; j < linkList[i].Count; j++)
            {
                //le dernier aura le lien avec candy
                if (i == lineRenderers.Count - 1 && j == linkList[i].Count - 1)
                {
                    if(candy)
                        lineRenderers[i].SetPosition(j, candy.transform.position);
                    else
                        lineRenderers[i].SetPosition(j, linkList[i][j].transform.position);
                }
                else
                    lineRenderers[i].SetPosition(j, linkList[i][j].transform.position);
            }
        }

    }

    //Cut the rope into 2 separate ropes
    public void updateCreateLinkList(GameObject linkToRemove)
    {
        isRopeCut = true;

        //Nouvelles listes de liens
        List<GameObject> list1 = new List<GameObject>();
        List<GameObject> list2 = new List<GameObject>();

        bool check = false;

        foreach(GameObject link in linkList[0])
        {
            if (GameObject.ReferenceEquals(link, linkToRemove))
                check = true;

            else
            {
                if (check)
                    list2.Add(link);
                else
                    list1.Add(link);
            }
        }

        linkList.Clear();
        linkList.Add(list1);
        linkList.Add(list2);
        
        //Drawer for the other part of the rope
        GameObject emptyObject = new GameObject("TmpDrawer");
        LineRenderer newLr = emptyObject.AddComponent<LineRenderer>();
        newLr.material = ropeMat;
        lineRenderers.Add(newLr);

        //"Destroy" effet fondu
        startFading();
    }

    void startFading()
    {
        StartCoroutine("FadeOut");
        StartCoroutine("FadeOut2");
    }

    IEnumerator FadeOut()
    {
        for (float f = 1f; f >= -0.05f; f -= 0.05f)
        {
            Color c = lineRenderers[0].material.color;
            c.a = f;
            lineRenderers[0].material.color = c;
            yield return new WaitForSeconds(0.05f);
        }
    }

    IEnumerator FadeOut2()
    {
        for (float f = 1f; f >= -0.05f; f -= 0.05f)
        {
            Color c = lineRenderers[1].material.color;
            c.a = f;
            lineRenderers[1].material.color = c;
            yield return new WaitForSeconds(0.05f);
        }

    }
    #endregion

    #region Get & Set
    public List<GameObject> getCreatedLinkList()
    {
        return createdLink;
    }

    public void setCreatedLinkList(List<GameObject> newList)
    {
        createdLink = newList;
    }

    public Rigidbody2D getPreviousRB()
    {
        return previousRB;
    }

    public void setPreviousRB(Rigidbody2D newPreviousRB)
    {
        previousRB = newPreviousRB;
    }

    public HingeJoint2D getCandyJoint()
    {
        return joint;
    }

    public void setCandyJoint(HingeJoint2D newJoint)
    {
        joint = newJoint;
    }
    #endregion
}
