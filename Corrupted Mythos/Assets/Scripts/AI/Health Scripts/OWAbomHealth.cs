using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OWAbomHealth : EnemyHealth
{
    public AK.Wwise.Event edamage;
    public AK.Wwise.Event death;


    [SerializeField]
    float foodChance = 0.1f;
    static float chanceMod = 0;
    public GameObject foodPref;
    public int AbomPoints = 50;

    private IoDAbomHealth icon;
    bool Icon = false;
    private float berserkGiven=10;

    private void Start()
    {
        script = player.GetComponent<PlayerMovement>();
        icon = this.GetComponent<IoDAbomHealth>();
        if(icon != null)
        {
            Icon = true;
        }
        BerserkGiver = berserkGiven;
    }

    public override void minusHealth(int damage, int knockback = 0)
    {
        takeDamage(damage, knockback, AbomPoints);
        edamage.Post(gameObject);

        if (health <= 0)
        {
            death.Post(gameObject);
            float drop = UnityEngine.Random.value;
            if (Icon)
            {
                icon.dead();
            }
            if (drop <= (foodChance + chanceMod))
            {
                GameObject food = Instantiate(foodPref);
                food.transform.position = this.transform.position;
                chanceMod = 0;
            }
            else
            {
                chanceMod += 0.05f;
            }

        }
    }
}
