using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoverBall
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerController : MonoBehaviour
    {
        const int UPDATE_RATE = 2;

        enum MoveDir
        {
            Stay,
            Up,
            Down
        }

        [Header("Setting")]
        [SerializeField]
        float theshold = 0.25f;

        [SerializeField]
        float moveSpeed = 2.0f;

        [Header("Bound")]
        [SerializeField]
        Transform boundUp;

        [SerializeField]
        Transform boundDown;

        bool isControlable = false;
        Rigidbody2D rigid;

        MoveDir moveDir;

        PacketCoordinate packetCoordinate;
        PacketCoordinate cachePacketCoordinate;

        struct PacketCoordinate
        {
            public float x;
            public float y;
            public float width;
            public float heigth;
        }

        void Awake()
        {
            GameController.Instance.OnGameStart += OnGameStart;
            GameController.Instance.OnGameOver += OnGameOver;

            rigid = GetComponent<Rigidbody2D>();
        }

        void Update()
        {
            if (Time.frameCount % UPDATE_RATE == 0)
            {
                InputHandler();
            }
        }

        void FixedUpdate()
        {
            MovementHandler();
        }

        void OnDestroy()
        {
            GameController.Instance.OnGameStart -= OnGameStart;
            GameController.Instance.OnGameOver -= OnGameOver;
        }

        void InputHandler()
        {
            if (!isControlable)
            {
                moveDir = MoveDir.Stay;
                return;
            }

            packetCoordinate = ProcessPacket(UDPReceiver.Instance.LastReceivedPacket);
            float deltaY = cachePacketCoordinate.y - packetCoordinate.y;

            if (Mathf.Approximately(deltaY, 0.0f))
            {
                moveDir = MoveDir.Stay;
            }
            else if (Mathf.Abs(deltaY) <= theshold)
            {
                moveDir = MoveDir.Stay;
            }
            else
            {
                moveDir = (deltaY > 0.0f) ? MoveDir.Up : MoveDir.Down;
            }

            Debug.Log("Move Dir : " + moveDir);
        }

        void MovementHandler()
        {
            Vector2 direction = Vector2.zero;

            if (MoveDir.Up == moveDir)
                direction = Vector2.up;
            else if (MoveDir.Down == moveDir)
                direction = Vector2.down;
            else if (MoveDir.Stay == moveDir)
                direction = Vector2.zero;

            Vector2 targetPosition = rigid.position + (direction * moveSpeed * Time.fixedDeltaTime);
            targetPosition.y = Mathf.Clamp(targetPosition.y, boundDown.position.y, boundUp.position.y);

            rigid.MovePosition(targetPosition);
        }

        PacketCoordinate ProcessPacket(string packet)
        {
            if (packet == "")
                return cachePacketCoordinate;

            PacketCoordinate packetCoordinate = new PacketCoordinate();
            string trimPacket = packet.Trim();

            string[] splitPacket = trimPacket.Split(',');

            try
            {
                packetCoordinate.x = float.Parse(splitPacket[0]);
                packetCoordinate.y = float.Parse(splitPacket[1]);
                packetCoordinate.width = float.Parse(splitPacket[2]);
                packetCoordinate.heigth = float.Parse(splitPacket[3]);
            }
            catch (Exception)
            {
                Debug.Log("Parsing packet error...");
            }

            return packetCoordinate;
        }
        
        void OnGameStart()
        {
            isControlable = true;
            cachePacketCoordinate = ProcessPacket(UDPReceiver.Instance.LastReceivedPacket);
        }

        void OnGameOver()
        {
            isControlable = false;
        }
    }
}
