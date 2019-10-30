using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoverBall
{
    public class Spawner : MonoBehaviour
    {
        [SerializeField]
        int poolAmount = 1;

        [SerializeField]
        float spawnRate = 0.65f;

        [SerializeField]
        GameObject prefab;

        [SerializeField]
        Transform[] spawnOrigin;

        bool spawnable = false;
        float spawnTimer;

        GameObject[] pools;

        void Awake()
        {
            Initialize();
        }

        void LateUpdate()
        {
            SpawnHandler();
        }

        void OnDestroy()
        {
            GameController.Instance.OnGameStart -= OnGameStart;
            GameController.Instance.OnGameOver -= OnGameOver;
        }

        void Initialize()
        {
            Random.InitState(Random.Range(0, 100));

            GameController.Instance.OnGameStart += OnGameStart;
            GameController.Instance.OnGameOver += OnGameOver;

            pools = new GameObject[poolAmount];

            for (int i = 0; i < poolAmount; ++i)
            {
                pools[i] = Instantiate<GameObject>(prefab, Vector3.zero, Quaternion.identity);
                pools[i].SetActive(false);
            }
        }

        void SpawnHandler()
        {
            if (spawnable && spawnTimer < Time.time)
            {
                Spawn();
                spawnTimer = Time.time + spawnRate;
            }
        }

        void Spawn()
        {
            int index = Random.Range(0, spawnOrigin.Length);

            for (int i = 0; i < poolAmount; ++i)
            {
                if (pools[i].activeSelf)
                    continue;

                pools[i].transform.position = spawnOrigin[index].position;
                pools[i].SetActive(true);

                break;
            }
        }

        void OnGameStart()
        {
            spawnable = true;
            spawnTimer = Time.time + spawnRate;
        }

        void OnGameOver()
        {
            spawnable = false;
            spawnTimer = 0.0f;
        }
    }
}
