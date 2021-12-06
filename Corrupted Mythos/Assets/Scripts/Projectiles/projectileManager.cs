using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class projectileManager : MonoBehaviour
{
    [Space]
    [Tooltip("The prefab from which the projectile will be instantiated")]
    [SerializeField]GameObject projPref;
    [SerializeField]
    float tmax;
    float timer = 0f;
    Animator anim;
    AudioManager manager;

    private void Start()
    {
        manager = FindObjectOfType<AudioManager>();
        anim = gameObject.transform.GetChild(0).GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(timer > 0)
        {
            timer -= Time.deltaTime;
        }
    }

    public bool attemptProjectileLaunch(GameObject launcher)
    {
        if(timer <= 0)
        {
            Vector2 dir = (GameObject.FindGameObjectWithTag("Player").transform.position - launcher.transform.position).normalized;

            StartCoroutine(Launch(dir, launcher));

            anim.SetTrigger("Attack");
            manager.PlaySound("FirelingCast");

            timer = tmax;
            return true;
        }
        else
        {
            return false;
        }
    }

    IEnumerator Launch(Vector2 dir, GameObject launcher)
    {
        yield return new WaitForSeconds(0.9f);
        if (dir.x > 0)
        {
            Vector2 start = launcher.transform.position;
            start.x = start.x + 1f;
            GameObject newProj = Instantiate(projPref, start, Quaternion.identity);
            newProj.GetComponent<fireGiantProjectile>().origin = gameObject.transform.position;
        }
        else
        {
            Vector2 start = launcher.transform.position;
            start.x = start.x - 1f;
            GameObject newProj = Instantiate(projPref, start, Quaternion.identity);
            newProj.GetComponent<fireGiantProjectile>().origin = gameObject.transform.position;
        }
    }
}
