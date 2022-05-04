using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class swing : MonoBehaviour
{
    public AK.Wwise.Event pattack;
    [SerializeField]
    private int damage = 50;
    private int berserkDamage = 100;
    [SerializeField]
    GameObject impact;
    [SerializeField]
    GameObject swoosh;

    AudioManager manager;
    EnemyHealth script;
    bool isAnim = false;
    bool isatk = false;
    //float t = 0;
    float dt = 0;
    //float step = 90f / 5;
    public Animator animator;
    public bool hit = false;
    public PlayerHealth PlayerHealth;
    private int lifesteal;
    int bonus;

    private void OnEnable()
    {
        PlayerHealth = this.GetComponentInParent<PlayerHealth>();
        impact.SetActive(false);
    }

    private void Start()
    {
        manager = FindObjectOfType<AudioManager>();
            lifesteal = 10;
        //Debug.Log(manager.name);
    }

    private void FixedUpdate()
    {
        if(dt >= 0)
        {
            dt -= Time.fixedDeltaTime;
        }
    }

    public void attack(bool atk = true, bool held = false, int mod = 1)
    {
        if (!isAnim && atk)
        {
            isatk = true;
            isAnim = true;
            bonus = mod;
            //t = 0;
            manager.PlaySound("swing");
            pattack.Post(gameObject);

            //For special held action
            if (held)
            {
                animator.SetTrigger("Hold_2");
                Invoke("CreateSwoosh", 0.25f);
                Invoke("UnAttack", 0.5f);
            }
            else
            {
                animator.SetTrigger("Swing");
                Invoke("UnAttack", 0.2f);
            }

            dt = 0;
        }
        else if(!isAnim && !atk)
        {
            isAnim = true;
            //t = 0;
            manager.PlaySound("swing");
            animator.SetTrigger("Slam");
            //Invoke("UnAttack", 0.55f);
        }
    }

    public void CreateSwoosh()
    {
        Instantiate(swoosh);
    }

    public void UnAttack()
    {
        isAnim = false;
        isatk = false;
    }

    public bool getStatus()
    {
        return isAnim;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (PlayerHealth.block)
        {
            //do nothing
        }
        else if (!collision.isTrigger && collision.CompareTag("enemy") && isatk && dt <= 0 && hit && PlayerHealth.berserking)
        {
            impact.SetActive(false);

            script = collision.GetComponent<EnemyHealth>();
            script.minusHealth(berserkDamage * bonus, 1);
            dt = 0.56f;

            GameObject.FindObjectOfType<CameraShake>()?.shakeCam(2, 0.1f, true);

            PlayerHealth.addHealth(lifesteal);
            impact.SetActive(true);
            impact.transform.position = transform.position;
            StartCoroutine(Wait());
        }
        else if (!collision.isTrigger && collision.CompareTag("enemy") && isatk && dt <= 0 && hit)
        {
            impact.SetActive(false);

            script = collision.GetComponent<EnemyHealth>();
            script.minusHealth(damage * bonus, 1);
            dt = 0.56f;

            //PlayerHealth.enrage(10);

            GameObject.FindObjectOfType<CameraShake>()?.shakeCam(2, 0.1f, true);

            impact.SetActive(true);
            impact.transform.position = transform.position;
            StartCoroutine(Wait());
        }
        else if(!collision.isTrigger && collision.CompareTag("Dummy") && isatk && dt <= 0 && hit)
        {
            impact.SetActive(false);

            collision.GetComponent<DummyHealth>()?.doDamage(damage);
            dt = 0.56f;

            GameObject.FindObjectOfType<CameraShake>()?.shakeCam(2, 0.1f, true);

            impact.SetActive(true);
            impact.transform.position = transform.position;
            StartCoroutine(Wait());
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (PlayerHealth.block)
        {
            //do nothing
        }
        else if (!collision.isTrigger && collision.CompareTag("enemy") && isatk && dt <= 0 && hit && PlayerHealth.berserking)
        {
            impact.SetActive(false);

            script = collision.GetComponent<EnemyHealth>();
            script.minusHealth(berserkDamage * bonus, 1);
            dt = 0.56f;

            GameObject.FindObjectOfType<CameraShake>()?.shakeCam(2, 0.1f, true);

            PlayerHealth.addHealth(lifesteal);
            impact.SetActive(true);
            impact.transform.position = transform.position;
            StartCoroutine(Wait());
        }
        else if (!collision.isTrigger && collision.CompareTag("enemy") && isatk && dt <= 0 && hit)
        {
            impact.SetActive(false);

            script = collision.GetComponent<EnemyHealth>();
            script.minusHealth(damage * bonus, 1);
            dt = 0.56f;

            //PlayerHealth.enrage(10);

            GameObject.FindObjectOfType<CameraShake>()?.shakeCam(2, 0.1f, true);

            impact.SetActive(true);
            impact.transform.position = transform.position;
            StartCoroutine(Wait());
        }
        else if (!collision.isTrigger && collision.CompareTag("Dummy") && isatk && dt <= 0 && hit)
        {
            impact.SetActive(false);

            collision.GetComponent<DummyHealth>()?.doDamage(damage);
            dt = 0.56f;

            GameObject.FindObjectOfType<CameraShake>()?.shakeCam(2, 0.1f, true);

            impact.SetActive(true);
            impact.transform.position = transform.position;
            StartCoroutine(Wait());
        }
    }

    internal void setHit(bool lean)
    {
        hit = lean;
    }

    IEnumerator Wait()
    {
        yield return new WaitForSeconds(1);
        impact.SetActive(false);
    }
}
