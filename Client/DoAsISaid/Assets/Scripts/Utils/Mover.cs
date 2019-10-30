using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoverBall
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Mover : MonoBehaviour
    {
        const int UPDATE_RATE = 2;

        [SerializeField]
        float speed = 2.0f;

        [SerializeField]
        float disableDelay = 5.0f;

        Rigidbody2D rigid;
        bool isContrable = false;
        float disableTimer;

        void Awake()
        {
            rigid = GetComponent<Rigidbody2D>();
        }

        void OnEnable()
        {
            disableTimer = Time.time + disableDelay;
        }

        void Update()
        {
            if (Time.frameCount % UPDATE_RATE == 0)
            {
                isContrable = GameController.Instance.IsGameStart;
                
                if (disableTimer < Time.time && gameObject.activeSelf)
                {
                    gameObject.SetActive(false);
                }
            }
        }

        void FixedUpdate()
        {
            if (isContrable)
            {
                rigid.MovePosition(rigid.position + (Vector2.left * speed * Time.fixedDeltaTime));
            }
        }
    }
}
