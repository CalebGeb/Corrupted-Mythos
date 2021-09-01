using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class projectileManager : MonoBehaviour
{
    [Space]
    [Tooltip("The prefab from which the projectile will be instantiated")]
    [SerializeField]GameObject projPref;
    [SerializeField]float speed;
    float timer = 0f;

    // Update is called once per frame
    void Update()
    {
        if(timer > 0)
        {
            timer -= Time.deltaTime;
        }

        attemptProjectileLaunch(this.gameObject);
    }

    public void attemptProjectileLaunch(GameObject launcher)
    {
        if(timer <= 0)
        {
            GameObject newProj = Instantiate(projPref, launcher.transform.position, Quaternion.identity);
            //Rigidbody projRB = newProj.GetComponent<Rigidbody>();

            //projRB.AddForce((GameObject.Find("Target").transform.position - transform.position).normalized * speed);

            timer = 10f;
        }
    }
}