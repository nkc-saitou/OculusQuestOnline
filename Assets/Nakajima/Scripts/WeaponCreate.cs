using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        PLAYER_CIRCLE,      // プレイヤーの周囲に生成
        HAND_DISPLAY,       // 手の角度から生成
    }

    public class WeaponCreate : MonoBehaviour
    {
        void Start()
        {

        }

        void Update()
        {

        }

        // 現在のステート
        public CreateState currentState;

        // 生成を基準とするオブジェクト
        [SerializeField, Header("<武器生成の基準オブジェクト>")]
        private GameObject spawnOriginObj;

        // 生成する武器のリスト
        [SerializeField]
        private List<GameObject> weaponList = new List<GameObject>();

        // プレイヤー中心からの半径
        [Header("<中心からの半径>")]
        public float radius = 1.0f;

        /// <summary>
        /// 武器を生成(自分の周りに)
        /// </summary>
        public void Create()
        {
            // ステートごとの生成方法で実行
            switch (currentState)
            {
                // プレイヤーの周囲から
                case CreateState.PLAYER_CIRCLE:
                    Create_Circle();
                    break;
                // 手の角度から
                case CreateState.HAND_DISPLAY:
                    Create_Display();
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
                weapon.transform.parent = spawnOriginObj.transform;
            }
        }

        /// <summary>
        /// 手の角度から武器を生成
        /// </summary>
        private void Create_Display()
        {

        }
    }
}


