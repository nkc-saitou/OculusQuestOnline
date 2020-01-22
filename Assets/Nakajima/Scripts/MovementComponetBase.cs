using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

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

        // 移動支点
        [SerializeField, Header("移動の支点となるオブジェクト")]
        protected GameObject centerOrigin;
        // 方向基準
        [SerializeField, Header("移動方向の基準となるオブジェクト")]
        protected GameObject moveDirObj;
        // 移動方向の基準オブジェクトを返す
        public GameObject GetMoveDirObj() {
            return moveDirObj;
        }
        // 移動方向基準の前方取得
        public Vector3 GetMoveDirectionForward() {
            return moveDirObj.transform.forward;
        }
        // 移動方向基準の右方向取得
        public Vector3 GetMoveDirectionRight() {
            return moveDirObj.transform.right;
        }

        // 基準とするカメラ
        [SerializeField]
        private Camera myCamera;
        public Camera GetMyCamera {
            get { return myCamera; }
        }

        // 移動する力
        private Vector3 velocity;
        public Vector3 Velocity {
            get { return velocity; }
        }
                     
        // 移動速度(最小、最大)
        [Header("＜最低スピード＞")]
        public float minSpeed = 10.0f;
        [Header("＜中間スピード＞")]
        public float halfSpeed = 15.0f;
        [Header("＜最速スピード＞")]
        public float maxSpeed = 20.0f;
        // 加速力
        [Header("＜加速スピード＞")]
        public float accelSpeed = 0.5f;
        // 現在のスピード
        private float currentSpeed = 15.0f;

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
            
            // 前方方向とそれ以外でスピードを変更
            if(Mathf.Abs(_vec.x) <= 0.2f && _vec.z > 0.7f) currentSpeed = maxSpeed;
            else if (_vec.z < 0.0f || Mathf.Abs(_vec.x) > 0.2f) currentSpeed = halfSpeed;
            
            // カメラの方向から、x-z平面の単位ベクトルを取得
            Vector3 cameraForward = Vector3.Scale(myCamera.transform.forward, new Vector3(1, 0, 1)).normalized;

            // 方向キーの入力値とカメラの向きから、移動方向の決定
            Vector3 moveVec = cameraForward * _vec.z + myCamera.transform.right * _vec.x;

            // 移動方向にスピードを掛ける。ジャンプや落下がある場合は、別途Y軸方向の速度ベクトルを足す。
            velocity = moveVec * currentSpeed;
        }

        /// <summary>
        /// VRヘッドセットのトラッキング
        /// </summary>
        /// <param name="_vec"></param>
        public void AddInputVector_Tracking(Vector3 _vec)
        {
            // 傾きが少ない場合は考慮しない
            if (Mathf.Abs(_vec.x) < 0.2f) {
                _vec.x = 0.0f;
            }
            if(Mathf.Abs(_vec.z) < 0.2f) {
                _vec.z = 0.0f;
            }

            // 移動量を加算
            _vec = _vec.normalized;
            velocity = _vec * currentSpeed;
        }

        /// <summary>
        /// 移動処理
        /// </summary>
        public virtual void Move(Vector3 _vec)
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
