using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public bool berserk;
    public float rageCounter;

    public int health, check=0, maxHealth;
    public Transform player;
    public Transform spawn;
    public GameObject death;
    public Slider hpBar;
    public Slider rageMeter;
    public PlayerMovement script;

    [HideInInspector]
    public CorruptedNode node;

    float timer;
    public int hpGainItems;

    public bool inv = false;

    private void Start()
    {
        health = 100;
        rageCounter = 0;

        hpBar.maxValue = health;
        hpBar.value = health;

        maxHealth = health;

        script = this.gameObject.GetComponent<PlayerMovement>();
    }

    
    void Update()
    {
        if(timer >= 0)
        {
            timer -= Time.deltaTime;
        }
    }

    public void minusHealth(int damage)
    {
        if (!inv)
        {
            if (timer <= 0 && berserk)
            {
                damage -= 10;
                health -= damage;
                //update UI
                hpBar.value = health;
                timer = 0.25f;

                //StartCoroutine(FlashObject(this.GetComponent<SpriteRenderer>(), Color.white, Color.red, 1f, 0.5f));
            }

            else if (timer <= 0)
            {
                health -= damage;
                //update UI
                hpBar.value = health;
                timer = 0.25f;

                rageCounter += 10;
                enrage();

                StartCoroutine(FlashObject(this.GetComponent<SpriteRenderer>(), Color.white, Color.red, 1f, 0.5f));
            }
            // respawn script
            if (health <= 0)
            {
                RespawnPlayer();
            }
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

        hpBar.value = health;
    }

    public void RespawnPlayer()
    {
        player.position = spawn.position;
        health = 100;
        rageCounter = 0;
        rageMeter.value = rageCounter;
        hpBar.value = health;
        this.GetComponent<CharacterController2D>().m_FacingRight = true;
        this.GetComponent<SpriteRenderer>().color = Color.white;
        this.transform.localScale = new Vector2(Mathf.Abs(this.transform.localScale.x), this.transform.localScale.y);
        StopAllCoroutines();
        script.spawning();

        if(node != null)
        {
            node.ResetNodeActivity();
            node.active = false;
            node = null;
        }

        GetComponent<Animator>()?.SetFloat("Speed", 0f);
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
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
            spawn.position = other.transform.position;
            check += 1;
            //Destroy(other);
        }
    }

    public void enrage()
    {
        //fill rage meter
        rageMeter.value = rageCounter;
        if (rageCounter >= 100)
        {
            berserk = true;
            Debug.Log("berserkable");
        }
    }
}
