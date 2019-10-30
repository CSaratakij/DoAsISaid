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

        Rigidbody2D rigid;
        bool isContrable = false;

        void Awake()
        {
            rigid = GetComponent<Rigidbody2D>();
        }

        void Update()
        {
            if (Time.frameCount % UPDATE_RATE == 0)
            {
                isContrable = GameController.Instance.IsGameStart;
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
