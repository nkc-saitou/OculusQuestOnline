using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nakajima.Movement
{
    /// <summary>
    /// Movement基底クラス
    /// </summary>
    public class MovementComponetBase : MonoBehaviour
    {
        /// <summary>
        /// Movementのステート
        /// </summary>
        public enum MovementState
        {
            // スティック移動
            MOVE_STICK,
            // 傾きで移動
            MOVE_INCLINATION,
        }
        [Header("＜移動手段＞")]
        public MovementState movementState;


        // 移動する力
        private Vector3 velocity;
        public Vector3 Velocity {
            get { return velocity; }
        }

        // 入力方向
        [Header("＜入力が検知された方向＞")]
        public Vector3 inputVec;
                     
        // 移動速度(最小、最大)
        [Header("＜最低スピード＞")]
        public float minSpeed = 10;
        [Header("＜最速スピード＞")]
        public float maxSpeed = 60;
        // 加速力
        [Header("＜加速スピード＞")]
        public float accelSpeed = 0.5f;
        // 現在のスピード
        private float currentSpeed;

        /// <summary>
        /// 初期化
        /// </summary>
        void Start()
        {

        }

        /// <summary>
        /// 更新処理
        /// </summary>
        void Update()
        {

        }

        /// <summary>
        /// 移動力を加える
        /// </summary>
        /// <param name="_vec"></param>
        public void AddInputVector(Vector3 _vec)
        {
            // 入力があるなら加速
            if(Mathf.Abs(_vec.x) >= 0.5f || Mathf.Abs(_vec.z) >= 0.5f) {
                currentSpeed = maxSpeed;
            }
            else if(Mathf.Abs(_vec.x) < 0.5f && Mathf.Abs(_vec.z) < 0.5f) {
                currentSpeed = minSpeed;
            }

            // カメラの方向から、x-z平面の単位ベクトルを取得
            Vector3 cameraForward = Vector3.Scale(Camera.main.transform.forward, new Vector3(1, 0, 1)).normalized;

            // 方向キーの入力値とカメラの向きから、移動方向の決定
            Vector3 moveForward = cameraForward * _vec.z + Camera.main.transform.right * _vec.x;

            // 移動方向にスピードを掛ける。ジャンプや落下がある場合は、別途Y軸方向の速度ベクトルを足す。
            velocity = moveForward * currentSpeed + new Vector3(0, velocity.y, 0);

            Debug.Log("移動力 : " + velocity); 

            // キャラクターの向きを進行方向に
            if (moveForward != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(moveForward);
            }
        }

        /// <summary>
        /// 移動処理
        /// </summary>
        protected virtual void Move(float _value)
        {

        }

        /// <summary>
        /// 自身が持っているMovementクラスを返す
        /// </summary>
        /// <returns>プレイヤーのMovement</returns>
        public MovementComponetBase GetMyMovementComponent()
        {
            return this;
        }
    }
}
