﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nakajima.Player;

/// <summary>
/// 武器の生成クラス
/// </summary>
namespace Nakajima.Weapon
{
    /// <summary>
    /// 武器の生成方法ステート
    /// </summary>
    public enum CreateState
    {
        PLAYER_CIRCLE,                  // プレイヤーの周囲に生成
        HAND_DISPLAY,                   // 手の角度から生成
        HAND_DISPLAY_COCK,        // 手の角度(コック式)
        HAND_FORWARD,               // 手の向いてる方向(前方)から生成
    }

    public class WeaponCreate : MonoBehaviour
    {
        void Start()
        {

        }

        /// <summary>
        /// 更新処理
        /// </summary>
        void Update()
        {
            if (WeaponUnfold == false) return;

            if(ActiveHand != null) UnfoldUpdate(ActiveHand);
        }

        // 現在のステート
        public CreateState currentState;

        // 生成を基準とするオブジェクト
        [SerializeField, Header("<武器生成の基準オブジェクト(プレイヤーの周囲に生成)>")]
        private GameObject spawnOriginObj_Circle;
        [Header("<武器生成の基準オブジェクト(コントローラーの角度から生成)>")]
        public GameObject spawnOriginObj_Display;
        // 生成中のオブジェクト
        private GameObject spawnObj;

        // 生成する武器のリスト
        [SerializeField]
        private List<GameObject> weaponList = new List<GameObject>();
        // 生成中の武器リスト
        public List<GameObject> createWeaponList = new List<GameObject>();

        // プレイヤー中心からの半径
        [Header("<中心からの半径>")]
        public float radius = 1.0f;

        // 展開中の武器の回転速度
        [SerializeField, Header("<武器の回転速度>")]
        private float rotateSpeed;

        // アクティブな手(武器生成)
        private PlayerHand activeHand;
        public PlayerHand ActiveHand
        {
            set { activeHand = value; }
            get { return activeHand; }
        }
        // 武器を生成可能かどうか
        private bool canCreate = true;
        public bool CanCreate
        {
            set { canCreate = value; }
            get { return canCreate; }
        }
        // 武器を展開中かどうか
        private bool isUnfold;
        public bool WeaponUnfold
        {
            set { isUnfold = value; }
            get { return isUnfold; }
        }

        /// <summary>
        /// 武器の生成方法の切り替え
        /// </summary>
        public void ChangeCreateState()
        {
            // 生成方法の切り替え
            switch (currentState)
            {
                case CreateState.PLAYER_CIRCLE:
                    currentState = CreateState.HAND_DISPLAY;
                    break;
                case CreateState.HAND_DISPLAY:
                    currentState = CreateState.HAND_DISPLAY_COCK;
                    break;
                case CreateState.HAND_DISPLAY_COCK:
                    currentState = CreateState.HAND_FORWARD;
                    break;
                case CreateState.HAND_FORWARD:
                    currentState = CreateState.PLAYER_CIRCLE;
                    break;
            }
        }

        /// <summary>
        /// 武器を生成(自分の周りに)
        /// </summary>
        public void Create()
        {
            // 既に展開中ならリターン
            if (WeaponUnfold == true || CanCreate == false) return;

            // ステートごとの生成方法で実行
            switch (currentState)
            {
                // プレイヤーの周囲から
                case CreateState.PLAYER_CIRCLE:
                    Create_Circle();
                    break;
                // 手の角度から
                case CreateState.HAND_DISPLAY:
                    Create_Display(activeHand);
                    break;
                // 手の角度から
                case CreateState.HAND_DISPLAY_COCK:
                    Create_Display(activeHand);
                    break;
                // 手の方向から
                case CreateState.HAND_FORWARD:
                    Create_Forward(activeHand);
                    break;
            }

        }

        /// <summary>
        /// プレイヤーの周囲に武器を生成
        /// </summary>
        private void Create_Circle()
        {
            // 武器間の角度
            float angleDiff = 360.0f / weaponList.Count;

            // リストから武器を生成
            for(int index = 0; index < weaponList.Count; index++) {
                Vector3 spawnPos = transform.position;

                // 角度検出
                float angle = (90 - angleDiff * index) * Mathf.Deg2Rad;
                spawnPos.x += radius * Mathf.Cos(angle);
                spawnPos.y += 1.0f;
                spawnPos.z += radius * Mathf.Sin(angle);

                // 生成
                GameObject weapon = Instantiate(weaponList[index], spawnPos, Quaternion.identity);
                weapon.transform.parent = spawnOriginObj_Circle.transform;
                createWeaponList.Add(weapon);
            }
            WeaponUnfold = true;
        }

        /// <summary>
        /// 展開中の更新処理
        /// </summary>
        public void UnfoldUpdate(PlayerHand _hand)
        {
            // 武器を展開中でないならリターン
            if (WeaponUnfold == false) return;

            switch (currentState)
            {
                // プレイヤーの周囲から
                case CreateState.PLAYER_CIRCLE:
                    // 回転
                    break;
                // 手の角度から
                case CreateState.HAND_DISPLAY:
                    Create_Display(_hand);
                    break;
                // 手の方向から
                case CreateState.HAND_FORWARD:
                    Create_Forward(_hand);
                    break;
            }
        }

        /// <summary>
        /// 手の角度から武器を生成
        /// </summary>
        private void Create_Display(PlayerHand _hand)
        {
            // 武器が対応されている角度検知
            float angleDiff = 360.0f / weaponList.Count;
            // 左コントローラーの角度検知
            float controllerAngle = OVRInput.GetLocalControllerRotation(OVRInput.Controller.LTrackedRemote).z;

            if (WeaponUnfold == false) {
                spawnObj = Instantiate(weaponList[0], spawnOriginObj_Display.transform);
            }
            else {
                spawnObj.transform.position = spawnOriginObj_Display.transform.position;
            }

            // コントローラーの角度を実際の角度に
            float angle = 180.0f * controllerAngle;
            int angleState = (int)angle / (int)angleDiff;
            if(angleState <= weaponList.Count) {
                if (angleState < 0) angleState = angleState + 6;
                Destroy(spawnObj);
                createWeaponList.Remove(spawnObj);

                spawnObj = Instantiate(weaponList[angleState], spawnOriginObj_Display.transform.position, Quaternion.identity);
                createWeaponList.Add(spawnObj);
            }
            WeaponUnfold = true;
        }

        /// <summary>
        /// 手の角度から生成
        /// </summary>
        private void Create_Cock(PlayerHand _hand)
        {
            // 上方向を支点とする
            Vector3 originVec = Vector3.up;

            
        }

        /// <summary>
        /// 手の方向から生成
        /// </summary>
        private void Create_Forward(PlayerHand _hand)
        {

            // コントローラーの方向を取得
            Vector3 controllerDir = _hand.transform.forward;
            Debug.Log(controllerDir);

            // 前方
            if(controllerDir.z >= 0.5f) {
                Debug.Log("前");
            }
            // 後方
            else if(controllerDir.z <= -0.5f) {
                Debug.Log("後ろ");
            }
            // 右方
            if(controllerDir.x >= 0.5f) {
                Debug.Log("右");
            }
            // 左方
            else if(controllerDir.x <= -0.5f) {
                Debug.Log("左");
            }

            WeaponUnfold = true;
        }

        /// <summary>
        /// 武器の破棄
        /// </summary>
        public void DeleteWeapon()
        {
            // ステージから削除
            foreach(GameObject obj in createWeaponList) {
                Destroy(obj);
            }

            // 要素から削除
            createWeaponList.RemoveAll(s => s == null);
            createWeaponList.Clear();
        }
    }
}


