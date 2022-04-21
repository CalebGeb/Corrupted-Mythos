using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class PlayerHealth : MonoBehaviour
{
    public static Action<bool> BerserkEffect = delegate { };

    public bool berserk;
    public bool berserking;
    public float rageCounter;
    public bool block;
    public bool perfectBlock;

    public int points;
    public Text pointScore;
    public Text livesCount;
    public int deathCount;

    public GameObject Pcamera;
    public GameObject Fcamera;

    public Color hurtFlash;
    public int health, check=0, maxHealth;
    public Transform player;
    public Transform spawn;
    PlayingUICntrl UICntrl;
    public LazyUIBar hpBar;
    public LazyUIBar rageMeter;
    public PlayerMovement script;
    public int lives;
    public ParticleSystem berserkBarEffect;

    [HideInInspector]
    public CorruptedNode node;

    float timer;
    public int hpGainItems;

    public bool inv = false;

    [Space]
    [SerializeField]
    Animator PlayerAnim;

    private void OnEnable()
    {
        EnemyHealth.AddPoints += PointsAddition;
        wispLoc.BerserkIncrement += enrage;
    }
    private void OnDisable()
    {
        EnemyHealth.AddPoints -= PointsAddition;
        wispLoc.BerserkIncrement -= enrage;
    }

    private void Start()
    {
        hpBar = GameObject.Find("HPBar")?.GetComponent<LazyUIBar>();
        rageMeter = GameObject.Find("Berserk")?.GetComponent<LazyUIBar>();
        hurtFlash = Color.red;
        health = 100;
        rageCounter = 0;

        if (rageMeter != null)
        {
            rageMeter.maxHP = 100;
            SetBerserkUI();
        }

        hpBar.maxHP = health;
        hpBar.setCurHP(health);

        maxHealth = health;

        script = gameObject.GetComponent<PlayerMovement>();
        lives = 5;
        if (livesCount != null) //Don't forget your null checking especially if you're not grabbing it in code
        {
            livesCount.text = lives.ToString();
        }
        berserk = false;
        UICntrl = GameObject.Find("Canvas")?.GetComponent<PlayingUICntrl>();
    }

    
    void Update()
    {
        if(timer >= 0)
        {
            timer -= Time.deltaTime;
        }
    }

    public void minusHealth(int damage, bool blockable = true)
    {
        if (!inv)
        {
            if (perfectBlock && blockable)
            {
                hurtFlash = Color.yellow;
                damage = 0;
            }
            else if (block && blockable)
            {
                hurtFlash = Color.gray;
                damage -= 15;
            }

            if (timer <= 0 && berserking)
            {
                damage -= 10;
            }

            if (timer <= 0)
            {
                health -= damage;
                //update UI
                hpBar.loseHP(damage);
                timer = 0.25f;

                enrage(5);

                StartCoroutine(FlashObject(gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>(), Color.white, hurtFlash, 1f, 0.5f));
            }
            if (perfectBlock)
            {
                hurtFlash = Color.red;
            }
            // respawn script
            if (health <= 0)
            {
                //RespawnPlayer();
                PlayerAnim.SetTrigger("Die");
                lives--;
                livesCount.text = lives.ToString();
                script.paused = true;

                //update UI
                if (lives <= 0)
                {
                    UICntrl.ShowDeathReload();
                    Time.timeScale = 0f;
                }
                else
                {
                    Invoke("DeathScreen", 0.2f);
                }
            }
        }
        
    }

    void DeathScreen()
    {
        script.paused = true;
        UICntrl.ShowDeath();
        Time.timeScale = 0f;
    }

    public void killPlayer()
    {
        lives--;
        livesCount.text = lives.ToString();
        BerserkEffect(true);
        if (lives <= 0)
        {
            UICntrl.ShowDeathReload();
            Time.timeScale = 0f;
        }
        else
        {
            DeathScreen();
        }
    }

    public void addHealth(int gain)
    {
        if (health + gain <= maxHealth)
        {
            health += gain;
        }
        else
        {
            health = maxHealth;
        }

        hpBar.gainHP(gain);
    }

    public void RespawnPlayer()
    {
        GameObject sr = gameObject.transform.GetChild(0).gameObject;
        //PlayerAnim.SetTrigger("Respawn"); - this does not exist, make it exist

        if (Pcamera.activeInHierarchy == false)
        {
            Pcamera.SetActive(true);
            Fcamera.SetActive(false);
        }


        UICntrl.HideDeath();

        player.position = spawn.position;
        health = 100;
        rageCounter = 0;
        if (rageMeter != null)
        {
            SetBerserkUI();
        }
        hpBar.setCurHP(health);
        this.GetComponent<CharacterController2D>().m_FacingRight = true;
        sr.GetComponent<SpriteRenderer>().color = Color.white;
        this.transform.localScale = new Vector2(Mathf.Abs(this.transform.localScale.x), this.transform.localScale.y);
        StopAllCoroutines();
        script.StopAllCoroutines();
        script.spawning();

        if (node != null)
        {
            node.ResetNodeActivity();
            node.active = false;
            node = null;
        }

        sr.GetComponent<Animator>()?.SetFloat("Speed", 0f);
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;

        deathCount += 1;
        points = points - (points / 10 / 2);
        BerserkEffect(true);
    }
    IEnumerator FlashObject(SpriteRenderer toFlash, Color originalColor, Color flashColor, float flashTime, float flashSpeed)
    {
        float flashingFor = 0;
        Color newColor = flashColor;
        while (flashingFor < flashTime)
        {
            toFlash.color = newColor;
            flashingFor += Time.deltaTime;
            yield return new WaitForSeconds(flashSpeed);
            flashingFor += flashSpeed;
            if (newColor == flashColor)
            {
                newColor = originalColor;
            }
            else
            {
                newColor = flashColor;
            }
        }
    }

    public void doPotion()
    {
        if(hpGainItems > 0 && health != maxHealth)
        {
            addHealth(30);
            hpGainItems--;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("checkpoint"))
        {
            spawn.position = new Vector3(other.transform.position.x, other.transform.position.y + 1, transform.position.z);
            check += 1;

        }
    }

    public void enrage(float val)
    {
        if (rageMeter != null)
        {
            if (!block)
            {
            rageCounter += val;
            SetBerserkUI();
            }
        }
    }

    public void FillBerserk()
    {
        float diff = 100 - rageCounter;
        enrage(diff);
    }
    
    public void SetBerserkUI()
    {
        rageMeter.setCurHP(rageCounter);
        if (rageCounter >= 100)
        {
            if (berserk == false)
            {
                BerserkEffect(false);
            }
            berserk = true;
        }
    }

    public void TurnOffBerserkEffect()
    {
        BerserkEffect(true);
    }

    public void PointsAddition(int addVal)
    {
        points += addVal;
    }

    public void knockAround(bool flip, int knockup)
    {
        StartCoroutine(knockUpSide(flip, knockup));
    }

    IEnumerator knockUp()
    {
            Vector2 upPos = new Vector2(transform.position.x, transform.position.y + 4);
            Vector2 orgPos = transform.position;
            float t = 0;

            while (t <= 0.3f)
            {
                t += Time.deltaTime;
                transform.position = Vector2.Lerp(orgPos, upPos, t / 0.3f);

                yield return null;
            }
            while (t <= 0.5f)
            {
                t += Time.deltaTime;
                transform.position = Vector2.Lerp(upPos, orgPos, (t - 0.3f) / 0.25f);

                yield return null;
            }
    }

    IEnumerator knockUpSide(bool flip, int knockup)
    {
        if (block)
        {
            yield break;
        }

        Vector2 upPos;
        if (flip)
        {
            upPos = new Vector2(transform.position.x + knockup, transform.position.y + knockup);
        }
        else
        {
            upPos = new Vector2(transform.position.x-knockup, transform.position.y + knockup);
        }
        
        Vector2 orgPos = transform.position;
        float t = 0;

        while (t <= 0.3f)
        {
            t += Time.deltaTime;
            transform.position = Vector2.Lerp(orgPos, upPos, t / 0.3f);

            yield return null;
        }
    }
}
