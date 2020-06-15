using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicHat : MonoBehaviour {

    #region Fields
    [SerializeField]
    private GameObject myPair;
    [SerializeField]
    private GameObject slideHook;

    private GameObject newCandy;
    private float addedVelocity = 40;
    #endregion

    #region Unity functions
    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Création new candy au niveau du nouveau chapeau
        if(collision.gameObject.tag == "Candy")
        {
            
            float collisionForce = collision.relativeVelocity.magnitude;

            newCandy = Instantiate(collision.gameObject, myPair.transform);
            CheckJoints();

            newCandy.transform.rotation = myPair.transform.rotation;
            newCandy.transform.localPosition = new Vector3(0, -25, 0);

            //Certains levels n'auront pas de slide donc par defaut velocity apres chapeau = 40
            if (slideHook)
            {
                float ratio = slideHook.GetComponent<DragObject>().getRatio();
                addedVelocity = getAddedVelocity(ratio);
            }
            
            newCandy.GetComponent<Rigidbody2D>().velocity = -myPair.transform.up * addedVelocity;

            //On enleve l'ancien
            Destroy(collision.gameObject);
        }
    }
    #endregion

    #region Joints management
    //On retire les potentiels Joints du candy
    void CheckJoints()
    {
        SpringJoint2D[] sj = newCandy.GetComponents<SpringJoint2D>();
        if (sj.Length > 0)
            foreach(SpringJoint2D joint in sj)
                Destroy(joint);

        HingeJoint2D[] hj = newCandy.GetComponents<HingeJoint2D>();
        if (hj.Length > 0)
            foreach (HingeJoint2D joint in hj)
                Destroy(joint);
    }
    #endregion

    #region Candy propulsion management
    int getAddedVelocity(float ratio)
    {
        //+ la corde est détendu + le candy est propulsé
        int addedVelocity = 0;

        if (0 <= ratio && ratio <= 0.2)
            addedVelocity = 45;
        else if (0.2 < ratio && ratio <= 0.5)
            addedVelocity = 40;
        else if (0.5 < ratio && ratio <= 0.8)
            addedVelocity = 30;
        else
            addedVelocity = 20;

        return addedVelocity;
    }
    #endregion
}
