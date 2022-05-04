using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StandingStone : MonoBehaviour
{
    public AK.Wwise.Event yay;

    [SerializeField]
    ParticleSystem effect;
    PlayerHealth script;
    bool triggered;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" && !effect.isPlaying)
        {
            yay.Post(gameObject);
            effect.Play();
            script = collision.GetComponent<PlayerHealth>();

            if (!triggered) { 
                script.points += 100;
                if (script.deathCount <= 5)
                {
                    if (script.deathCount <= 3)
                    {
                        if (script.deathCount <= 0)
                        {
                            script.points += 200;
                            Debug.Log("adding points: check 200");
                        }
                        script.points += 100;
                        Debug.Log("adding points: check 100");
                    }
                    script.points += 100;
                    Debug.Log("adding points: check 100");
                }
                Debug.Log("adding points: check 100");

                script.deathCount = 0;

                if(script.pointScore != null)
                    script.pointScore.text = script.points.ToString();


                //script.lives++;
            }
            triggered = true;
        }
    }
}
