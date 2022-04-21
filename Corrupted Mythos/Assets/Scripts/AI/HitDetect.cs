using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitDetect : MonoBehaviour
{
    public StateManager em;
    public Animator fganim;
    [SerializeField]
    SpineAnimCntrler sanim;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" &&  em.stagr <= 0)
        {
            em.attack = true;
            fganim.SetTrigger("Attack");
            if(sanim != null)
            {
                sanim.StopSpineAnim(0);
                sanim.DoSpineAnim(0, "Attack");
            }
        }
    }
}
