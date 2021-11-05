using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController2D cntrler;
    public Animator animatior;
    [SerializeField]
    PlayerHealth playerHP;
    [Space]
    public float speed;
    public swing weap;
    public GameObject pause;
    public GameObject dashbox;

    private Inputs pcontroller;
    private float dir;
    private bool jump = false;
    public bool paused = false;
    private bool Bactive;
    public PlayerHealth playerHealth;
    public Transform impact;

    void Start()
    {
        impact = this.transform.GetChild(0);
        pcontroller = new Inputs();
        pcontroller.Enable();
        playerHealth = this.GetComponentInParent<PlayerHealth>();
    }

    void Update()
    {
        dir = 0;

        if (pcontroller.player.Pause.triggered)
        {
            pause.SetActive(true);
            paused = true;
            Time.timeScale = 0f;
        }

        if (!weap.getStatus() && !paused)
        {
            dir = pcontroller.player.movement.ReadValue<Vector2>().x * speed;
            animatior.SetFloat("Speed", Mathf.Abs(dir));

            if (pcontroller.player.DashR.triggered)
            {
                StartCoroutine(Dash(1f));
            }
            else if (pcontroller.player.DashL.triggered)
            {
                StartCoroutine(Dash(-1f));
            }
        }
        else
        {
            animatior.SetFloat("Speed", 0);
        }

        if (pcontroller.player.jump.triggered && !paused && !weap.getStatus())
        {
            jump = true;
        }
        if (pcontroller.player.attack.triggered && !paused)
        {
            weap.attack();
        }
        if (pcontroller.player.Heal.triggered)
        {
            playerHP.doPotion();
        }
        if (pcontroller.player.berserk.triggered)
        {
            if (playerHealth.berserk)
            {
                this.GetComponent<SpriteRenderer>().color = Color.red;
                impact.GetComponent<SpriteRenderer>().color = Color.magenta;
                speed += 20;
                StartCoroutine(rageMode());
            }
        }
        ///*
        if (Bactive)
        {
            playerHealth.rageCounter -= Time.deltaTime;
                if(playerHealth.rageCounter <= 0)
                {
                    Bactive = false;
                    playerHealth.rageCounter = 0; 
                }
            playerHealth.rageMeter.value = playerHealth.rageCounter;
        }
        //*/
    }
    private void FixedUpdate()
    {
        if (!paused)
        {
            cntrler.Move(dir * Time.fixedDeltaTime, false, jump);
            jump = false;
        }
    }

    public void spawning()
    {
        StartCoroutine(SpawnedIn());
    }

    public void ToggleSwingBox(bool lean)
    {
        GetComponentInChildren<swing>()?.setHit(lean);
    }

    IEnumerator SpawnedIn()
    {
        paused = true;
        yield return new WaitForSeconds(1);
        paused = false;
    }

    IEnumerator Dash(float dir)
    {
        //preperatory work
        paused = true;
        float t = 0;
        playerHealth.inv = true;
        dashbox.SetActive(true);
        Vector2 targPos = new Vector2(transform.position.x + (5 * dir), transform.position.y);
        Vector2 orgPos = transform.position;
        int layermask = LayerMask.GetMask("Platforms");
        RaycastHit2D hit = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y - 1), new Vector2(dir, 0), 5, layermask);
        if (hit)
        {
            t = 1;
        }
        //Move
        while (t < 0.25f)
        {
            t += Time.deltaTime;
            transform.position = Vector2.Lerp(orgPos, targPos, (t / 0.25f));

            yield return null;
            Debug.Log(t + " " + transform.position.ToString());
        }
        //End
        playerHealth.inv = false;
        if (!hit)
        {
            transform.position = targPos;
        }
        dashbox.SetActive(false);
        paused = false;
    }

    IEnumerator rageMode()
    {
        int i;
        //deplete the berserk mode bar
        Bactive = true;

        for (i=0;i<5;i++) {
            yield return new WaitForSeconds(2);
            playerHealth.rageCounter -= 20;
            playerHealth.rageMeter.value = playerHealth.rageCounter;
        }

        //yield return new WaitForSeconds(10);

        playerHealth.rageCounter = 0;
        playerHealth.rageMeter.value = playerHealth.rageCounter;

        speed -= 20;
        playerHealth.berserk = false;
        Debug.Log("unberserking");
        this.GetComponent<SpriteRenderer>().color = Color.white;
        impact.GetComponent<SpriteRenderer>().color = Color.white;
    }
}
