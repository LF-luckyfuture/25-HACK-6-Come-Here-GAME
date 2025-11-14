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
    private bool isWaitingForNextWave = false;

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
                if (waveCounter <= 0 && !isWaitingForNextWave)
                {
                    StartCoroutine(WaitBeforeNextWave());
                }
            }
        }

        transform.position = target.position;

        // 清理远离的敌人
        CleanupDistantEnemies();
    }

    // 在下一波开始前等待一段时间
    IEnumerator WaitBeforeNextWave()
    {
        isWaitingForNextWave = true;

        // 等待所有敌人都被清除，或者等待固定时间
        float waitTime = 3f; // 可以调整这个等待时间
        Debug.Log($"等待 {waitTime} 秒后开始下一波");
        yield return new WaitForSeconds(waitTime);

        // 也可以选择等待直到场景中没有敌人
        // yield return new WaitUntil(() => GameObject.FindGameObjectsWithTag("Enemy").Length == 0);

        GoToNextWave();
        isWaitingForNextWave = false;
    }

    void CleanupDistantEnemies()
    {
        if (spawnedEnemies.Count == 0) return;

        int checkTarget = Mathf.Min(enemyToCheck + checkPerFrame, spawnedEnemies.Count);

        for (int i = enemyToCheck; i < checkTarget; i++)
        {
            if (i < spawnedEnemies.Count && spawnedEnemies[i] != null)
            {
                if (Vector3.Distance(transform.position, spawnedEnemies[i].transform.position) > despawnDistance)
                {
                    Destroy(spawnedEnemies[i]);
                    spawnedEnemies.RemoveAt(i);
                    i--; // 调整索引
                    checkTarget--;
                }
            }
            else if (i < spawnedEnemies.Count)
            {
                spawnedEnemies.RemoveAt(i);
                i--;
                checkTarget--;
            }
        }

        enemyToCheck = checkTarget;
        if (enemyToCheck >= spawnedEnemies.Count)
        {
            enemyToCheck = 0;
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
            // 所有波次结束后的处理
            // 方案1：循环波次
            currentWave = 0;
            Debug.Log("重新开始波次循环！");

            // 方案2：停止生成并显示胜利信息
            // Debug.Log("所有波次已完成！游戏胜利！");
            // isSpawning = false;
            // return;

            // 方案3：增加难度后继续
            // IncreaseDifficulty();
            // currentWave = 0;
        }

        waveCounter = waves[currentWave].waveLength;
        spawnCounter = waves[currentWave].timeBetweenSpawns;

        Debug.Log($"进入第 {currentWave + 1} 波，持续时间: {waveCounter}秒，生成间隔: {spawnCounter}秒");
    }

    // 可选：随着波次增加难度
    private void IncreaseDifficulty()
    {
        foreach (var wave in waves)
        {
            // 例如：减少生成间隔，增加波次长度等
            wave.timeBetweenSpawns *= 0.9f; // 生成更快
            wave.waveLength *= 1.1f; // 波次更长
        }
    }

    // 添加一个方法来手动开始/停止生成
    public void SetSpawning(bool spawning)
    {
        isSpawning = spawning;
    }

    // 添加一个方法来获取当前波次信息
    public int GetCurrentWave()
    {
        return currentWave + 1;
    }

    public int GetTotalWaves()
    {
        return waves.Count;
    }
}

[System.Serializable]
public class WaveInfo
{
    public GameObject enemyToSpawn;
    public float waveLength = 10f;
    public float timeBetweenSpawns = 3f;
}