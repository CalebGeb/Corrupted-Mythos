using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OWFrostGiantHealth : EnemyHealth
{
    public AK.Wwise.Event victory;


    bool inv = true;
    [Space]
    [SerializeField]
    float stagertime;
    private int points = 100;

    private void Start()
    {
        script = gameObject.GetComponent<PlayerMovement>();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    public override void minusHealth(int damage, int knockback = 0)
    {
        if (!inv)
        {
            takeDamage(damage, knockback, points); //Find a way to make frost giant take less knockback (this will probably be larger overhaul)
            inv = true;
            StartCoroutine(FlashObject(transform.GetChild(0).GetComponent<SpriteRenderer>(), Color.white, Color.yellow, 0.3f, 0.1f));
        }
        else
        {
            //Probably do some fancy flashing to show hes immune?
            StartCoroutine(FlashObject(transform.GetChild(0).GetComponent<SpriteRenderer>(), Color.white, Color.yellow, 0.5f, 0.1f));
        }

        if (script != null)
        {
            victory.Post(gameObject);
            script.killCount++;
            script.GodBarctrl.IncrementBar(script.GodBarctrl.GetFullSize() / 15);
        }
    }

    public void changeInv()
    {
        inv = false;
        em.setStgr(stagertime, true);
        Invoke("resetInv", stagertime);
    }
    void resetInv()
    {
        inv = true;
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

        toFlash.color = originalColor;
    }
}
