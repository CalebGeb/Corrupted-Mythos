using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacles : MonoBehaviour
{
    public int damage;
    public int knockup;
    private PlayerHealth script;
    public bool flip;
    public GameObject OtherSideHit;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            script = collision.gameObject.GetComponent<PlayerHealth>();
            script.block = false;
            bool dash = collision.gameObject.GetComponent<PlayerMovement>().isDash;
            if (!dash)
            {
                script.minusHealth(damage, false);
                script.knockAround(flip, knockup);
                StartCoroutine(IFrames());
            }
        }
    }

    IEnumerator IFrames()
    {
        OtherSideHit.GetComponent<PolygonCollider2D>().enabled = false;
        this.GetComponent<PolygonCollider2D>().enabled = false;
        yield return new WaitForSeconds(1);
        OtherSideHit.GetComponent<PolygonCollider2D>().enabled = true;
        this.GetComponent<PolygonCollider2D>().enabled = true;
    }
}
