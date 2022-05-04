using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxeProjectileTest : MonoBehaviour
{
    public AK.Wwise.Event attack;

    [Tooltip("The damage dealt when an enemy is hit")]
    [SerializeField]
    int damage;
    [Tooltip("The lifetime of the projectile if it does not hit anything.")]
    [SerializeField]
    float lifetime;


    [Space]
    [SerializeField]
    float xSpeed;
    [SerializeField]
    float launchAngle;
    [SerializeField]
    float timeToPeak;
    [SerializeField]
    float rotSpeed;

    float a;
    float dir = 0;
    float t = 0;

    float xOriginal;
    float yOriginal;

    void Start()
    {
        attack.Post(gameObject);
        xOriginal = transform.position.x;
        yOriginal = transform.position.y;

        if (this.transform.localScale.x > 0)
        {
            dir = 1;
            transform.Rotate(new Vector3(0, 0, 180));
        }
        else
        {
            dir = -1;
        }
    }
    void FixedUpdate()
    {
        t += Time.deltaTime;

        float newX = xSpeed * t * dir;  //x where xOriginal is 0
        transform.position = new Vector2(xOriginal + (newX), (-1 * Mathf.Tan(launchAngle * Mathf.Deg2Rad) * (newX * newX)) / (2 * timeToPeak) + (xSpeed * Mathf.Tan(launchAngle * Mathf.Deg2Rad) * (newX * dir)) + yOriginal);
        transform.Rotate(new Vector3(0, 0, Time.deltaTime * (dir * -1) * rotSpeed));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag != "Player")
        {
            if(collision.gameObject.tag == "enemy")
            {
                
                collision.gameObject.GetComponent<EnemyHealth>().minusHealth(damage);
                FindObjectOfType<AudioManager>().PlaySound("swing");
            }
            else if (collision.name == "ForestFrontGrass")
            {
                FindObjectOfType<AudioManager>().StopSound("axeThrow");
                Destroy(this.gameObject);
            }
            else if (!collision.isTrigger && collision.gameObject.tag == "Dummy")
            {
                collision.GetComponent<DummyHealth>()?.doDamage(damage);
                FindObjectOfType<AudioManager>().PlaySound("swing");
            }
        }
    }

}
