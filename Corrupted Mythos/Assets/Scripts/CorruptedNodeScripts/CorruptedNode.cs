using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CorruptedNode : MonoBehaviour
{
    #region Serialized Fields
    [Tooltip("This is the list of enemies this node can spawn")]
    [SerializeField]
    List<EnemySpawner> EnemyList = new List<EnemySpawner>();
    [Tooltip("The list of barriers to activate when the arena goes live")]
    [SerializeField]
    List<GameObject> BarrierList = new List<GameObject>();
    [Tooltip("The list of spawnpoints attached to this node")]
    [SerializeField]
    List<GameObject> Spawners = new List<GameObject>();
    [Space]
    [Tooltip("The number of spawns")]
    [SerializeField]
    int SpawnCount;
    [Tooltip("The initial wave")]
    [SerializeField]
    int initWave;
    [Tooltip("The rest period for the player")]
    [SerializeField]
    float restT = 2;
    [Tooltip("The size of subsequent waves")]
    [SerializeField]
    int subWaves;
    [SerializeField]
    GameObject EndEffect;

    [Space]
    [SerializeField]
    bool ManualStart;
    #endregion

    //Internal variables
    int spawned = 0;
    public bool active = false;
    public GameObject E;
    List<GameObject> Enemies = new List<GameObject>();
    private Inputs pcontroller;
    private Transform target;
    private LevelEndHandler leh;
    bool init = false;
    float t = 0;
    bool end = false;

    // Start is called before the first frame update
    void Start()
    {
        if(EnemyList.Count == 0)
        {
            Debug.LogError("No enemies to spawn. They should be assigned in the editor.");
        }
        pcontroller = new Inputs();
        pcontroller.Enable();

        leh = GameObject.FindGameObjectWithTag("LevelEndHandler").GetComponent<LevelEndHandler>();
        init = leh.AddToList(this);

        E.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (ManualStart)
        {
            StartNodeActivity();
            ManualStart = false;
        }

        if (active && !end)
        {
            if(Enemies.Count == 0 && spawned >= SpawnCount)
            {
                EndNodeActivity();
            }
            else if(Enemies.Count == 0)
            {
                if (t <= 0)
                {
                    for (int i = 0; i < subWaves; i++)
                    {
                        if (spawned < SpawnCount)
                        {
                            SpawnEnemy();
                        }
                    }
                    t = restT;
                    return;
                }
                t -= Time.deltaTime;
            }
        }   
    }

    #region NodeActivity
    public void StartNodeActivity()
    {
        foreach(GameObject barrier in BarrierList)
        {
            barrier.SetActive(true);
        }

        foreach(GameObject spawner in Spawners)
        {
            spawner.SetActive(true);
        }

        foreach (EnemySpawner spawner in EnemyList)
        {
            spawner.InitSpawner();
        }

        for (int i = 0; i < initWave; i++)
        {
            SpawnEnemy();
        }

        t = restT;
        active = true;
    }

    public void SpawnEnemy()
    {
        int pick = Random.Range(0, EnemyList.Count);
        EnemyList[pick].Spawn(this);

        spawned++;
    }

    public void ResetNodeActivity()
    {
        foreach(GameObject enemy in Enemies)
        {
            Destroy(enemy);
        }
        Enemies.Clear();
        spawned = 0;
        foreach (GameObject barrier in BarrierList)
        {
            barrier.SetActive(false);
        }

        foreach (GameObject spawner in Spawners)
        {
            spawner.SetActive(false);
        }
    }

    public void EndNodeActivity()
    {
        if (!end)
        {
            Instantiate(EndEffect, new Vector2(transform.position.x, transform.position.y), new Quaternion(0, 0, 0, 0));
            GameObject.FindObjectOfType<CameraShake>().shakeCam(6, 3.4f, true);
            Invoke("DestroyObjs", 3);
            Destroy(this.gameObject, 3f);
            end = true;
        }
    }

    public void DestroyObjs()
    {
        foreach (GameObject barrier in BarrierList)
        {
            barrier.SetActive(false);
        }

        if (init)
        {
            leh.RemoveFromList(this);
        }
    }

    public void addEnemy(GameObject enemy)
    {
        Enemies.Add(enemy);
    }

    public void removeEnemy(GameObject enemy)
    {
        if (Enemies.Contains(enemy))
        {
            Enemies.Remove(enemy);
        }
    }
    #endregion

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" && collision.GetType() == typeof(BoxCollider2D))
        {
            target = collision.transform;
            pcontroller.player.NodeInteract.started += StartNode;
            E.SetActive(true);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" && collision.GetType() == typeof(BoxCollider2D))
        {           
            pcontroller.player.NodeInteract.started -= StartNode;
            E.SetActive(false);
        }
    }
    private void StartNode(InputAction.CallbackContext c)
    {
        StartNodeActivity();
        target.GetComponent<PlayerHealth>().node = this;
    }
}
