using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyToSpawn;

    public float timeToSpawn;
    private float spawnCounter;

    public Transform minSpawn, maxSpawn;

    private Transform target;
    private float despawnDistance;

    public int checkPerFrame;
    private int enemyToCheck;

    private List<GameObject> spawnedEnemies = new List<GameObject>();

    public List<WaveInfo> waves;

    private int currentWave;
    private float waveCounter;

    // 添加波次状态控制
    private bool isSpawning = true;

    // Start is called before the first frame update
    void Start()
    {
        currentWave = -1;
        GoToNextWave();

        target = FindObjectOfType<Character>().transform;

        despawnDistance = Vector3.Distance(transform.position, maxSpawn.position) + 4f;
    }

    // Update is called once per frame
    void Update()
    {
        if (Character.instance.gameObject.activeSelf && isSpawning)
        {
            // 修复条件：只要当前波次有效就生成敌人
            if (currentWave >= 0 && currentWave < waves.Count)
            {
                // 波次时间倒计时
                waveCounter -= Time.deltaTime;

                // 生成敌人倒计时
                spawnCounter -= Time.deltaTime;
                if (spawnCounter <= 0)
                {
                    spawnCounter = waves[currentWave].timeBetweenSpawns;

                    GameObject newEnemy = Instantiate(waves[currentWave].enemyToSpawn, SelectSpawnPoint(), Quaternion.identity);
                    spawnedEnemies.Add(newEnemy);
                }

                // 检查是否切换到下一波
                if (waveCounter <= 0)
                {
                    GoToNextWave();
                }
            }
        }

        transform.position = target.position;

        // 清理远离的敌人
        CleanupDistantEnemies();
    }

    void CleanupDistantEnemies()
    {
        int checkTarget = enemyToCheck + checkPerFrame;

        while (enemyToCheck < checkTarget)
        {
            if (enemyToCheck < spawnedEnemies.Count)
            {
                if (spawnedEnemies[enemyToCheck] != null)
                {
                    if (Vector3.Distance(transform.position, spawnedEnemies[enemyToCheck].transform.position) > despawnDistance)
                    {
                        Destroy(spawnedEnemies[enemyToCheck]);
                        spawnedEnemies.RemoveAt(enemyToCheck);
                        checkTarget--;
                    }
                    else
                    {
                        enemyToCheck++;
                    }
                }
                else
                {
                    spawnedEnemies.RemoveAt(enemyToCheck);
                    checkTarget--;
                }
            }
            else
            {
                enemyToCheck = 0;
                checkTarget = 0;
                break; // 添加break避免无限循环
            }
        }
    }

    public Vector3 SelectSpawnPoint()
    {
        Vector3 spawnPoint = Vector3.zero;

        bool spawnVerticalEdge = Random.Range(0f, 1f) > .5f;

        if (spawnVerticalEdge)
        {
            spawnPoint.y = Random.Range(minSpawn.position.y, maxSpawn.position.y);

            if (Random.Range(0f, 1f) > .5f)
            {
                spawnPoint.x = maxSpawn.position.x;
            }
            else
            {
                spawnPoint.x = minSpawn.position.x;
            }
        }
        else
        {
            spawnPoint.x = Random.Range(minSpawn.position.x, maxSpawn.position.x);

            if (Random.Range(0f, 1f) > .5f)
            {
                spawnPoint.y = maxSpawn.position.y;
            }
            else
            {
                spawnPoint.y = minSpawn.position.y;
            }
        }

        return spawnPoint;
    }

    public void GoToNextWave()
    {
        currentWave++;

        if (currentWave >= waves.Count)
        {
            currentWave = 0;
            // 所有波次结束，可以选择循环或者停止生成
            Debug.Log("所有波次已完成！");
            isSpawning = false;
            return;
        }

        waveCounter = waves[currentWave].waveLength;
        spawnCounter = waves[currentWave].timeBetweenSpawns;

        Debug.Log($"进入第 {currentWave + 1} 波，持续时间: {waveCounter}秒，生成间隔: {spawnCounter}秒");
    }
}

[System.Serializable]
public class WaveInfo
{
    public GameObject enemyToSpawn;
    public float waveLength = 10f;
    public float timeBetweenSpawns = 1f;
}