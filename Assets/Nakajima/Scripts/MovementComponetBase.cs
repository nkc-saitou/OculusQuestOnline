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
        [Header("移動手段")]
        public MovementState movementState;

        // 移動する力
        [Header("移動力")]
        public Vector3 velocity;
        // 移動方向
        protected Vector3 moveVec;
        // 現在の方向
        [Header("現在の方向")]
        public Vector3 movementDir;

        // 入力方向
        [Header("入力が検知された方向")]
        public Vector3 inputVec;
                     
        // 移動速度(最小、最大)
        [Header("最低スピード")]
        public float minSpeed;
        [Header("最速スピード")]
        public float maxSpeed;
        // 加速力
        [Header("加速スピード")]
        public float accelSpeed;

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
