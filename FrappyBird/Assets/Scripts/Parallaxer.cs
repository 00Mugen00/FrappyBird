using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallaxer : MonoBehaviour {

    class PoolObject
    {
        public Transform transform;
        public bool inUse;
        public PoolObject(Transform t)
        {
            transform = t;
        }
        public void Use()
        {
            inUse = true;
        }
        public void Dispose()
        {
            inUse = false;
        }
    }

    [System.Serializable]
    public struct YSpawnRange
    {
        public float min;
        public float max;
    }

    private static readonly int DISPOSE_POSITION = 1000;
    private static readonly int INITIAL_SPAWNTIMER = 0;

    //Prefab to respawn
    public GameObject Prefab;
    public int poolSize;
    public float shiftSpeed;
    public float spawnRate;

    public YSpawnRange ySpawnRange;
    public Vector3 defaultSpawnPos;
    //Particle prewarm
    public bool spawnImmediate;
    public Vector3 immediateSpawnPos;
    public Vector2 targetAspectRatio;

    float spawnTimer;
    float targetAspect;
    PoolObject[] poolObjects;
    GameManager game;

    void Awake()
    {
        Configure();
    }

    void Start()
    {
        game = GameManager.Instance;
    }

    void OnEnable()
    {
        GameManager.OnGameOverConfirmed += OnGameOverConfirmed;
    }

    void OnDisable()
    {
        GameManager.OnGameOverConfirmed -= OnGameOverConfirmed;

    }

    void OnGameOverConfirmed()
    {
        for(int i=0; i<poolObjects.Length; i++)
        {
            poolObjects[i].Dispose();
            poolObjects[i].transform.position = Vector3.one * DISPOSE_POSITION;
        }
        Configure();
    }

    void Update()
    {
        if (game.GameOver) return;

        Shift();
        spawnTimer += Time.deltaTime;
        if(spawnTimer > spawnRate)
        {
            Spawn();
            spawnTimer = INITIAL_SPAWNTIMER;
        }
    }

    void Configure()
    {
        targetAspect = targetAspectRatio.x / targetAspectRatio.y;
        poolObjects = new PoolObject[poolSize];
        for(int i=0; i<poolObjects.Length; i++)
        {
            GameObject gameObject = Instantiate(Prefab) as GameObject;
            gameObject.SetActive(true);
            Transform transform = gameObject.transform;
            transform.SetParent(base.transform);
            transform.position = Vector3.one * DISPOSE_POSITION;
            poolObjects[i] = new PoolObject(transform);
        }
        if (spawnImmediate)
        {
            SpawnImmediate();
        }
    }

    void Spawn()
    {
        Transform transform = GetPoolObject();
        if (transform == null) return; //if true, this indicates that poolSize is too small
        Vector3 position = Vector3.zero;
        position.y = Random.Range(ySpawnRange.min, ySpawnRange.max);
        position.x = (defaultSpawnPos.x * Camera.main.aspect) / targetAspect;
        transform.position = position;
    }

    void SpawnImmediate()
    {
        Transform transform = GetPoolObject();
        if (transform == null) return; //if true, this indicates that poolSize is too small
        Vector3 position = Vector3.zero;
        position.x = (immediateSpawnPos.x * Camera.main.aspect) / targetAspect;
        position.y = Random.Range(ySpawnRange.min, ySpawnRange.max);
        transform.position = position;
        Spawn();
    }

    void Shift()
    {
        for (int i = 0; i < poolObjects.Length; i++)
        {
            poolObjects[i].transform.position += Vector3.left * shiftSpeed * Time.deltaTime;
            CheckDisposeObject(poolObjects[i]);
        }
    }

    void CheckDisposeObject(PoolObject poolObject)
    {
        if(poolObject.transform.position.x < (-defaultSpawnPos.x * Camera.main.aspect) / targetAspect)
        {
            poolObject.Dispose();
            poolObject.transform.position = Vector3.one * DISPOSE_POSITION;
        }
    }

    Transform GetPoolObject()
    {
        //retrieving first available pool object
        for (int i = 0; i < poolObjects.Length; i++)
        {
            if (!poolObjects[i].inUse)
            {
                poolObjects[i].Use();
                return poolObjects[i].transform;
            }
        }
        return null;
    }
}
