using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using Photon.Pun;
using Saitou.Network;
using UniRx;
using UniRx.Triggers;

public class GuardManager : MonoBehaviourPunCallbacks
{
    public GameObject[] GuardPos;
    public GameObject guardPrefab;

    public int InstantiateCount;

    TestPlayerCreate create;

    // 今生成されているGuardObjectのIndex
    List<int> GuardIndex = new List<int>();

    // すべての添字が入った配列
    List<int> randomIndex = new List<int>();

    List<Guard> CreateGuardList = new List<Guard>();


    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    int RandomGuardExtract()
    {
        List<int> TMP = new List<int>();
        
        // 何も生成されていなければどこのバリアを生成しても良い
        if(GuardIndex.Count == 0)
        {
            TMP.Add(UnityEngine.Random.Range(0, randomIndex.Count));  //無条件でランダム表示する奴
            Debug.Log("初回 " + TMP[0]);
            return TMP[0];
        }

        // ランダムの判定用のリスト
        List<int> TempIndexLis = new List<int>();

        // すでに生成されているかどうかをチェック
        for(int i = 0; i < randomIndex.Count; i ++)
        {
            if (CheckDuplication(randomIndex[i]))
            {
                TempIndexLis.Add(randomIndex[i]);
            }
        }

        TMP = TempIndexLis.OrderBy(i => Guid.NewGuid()).Take(1).ToList();

        Debug.Log(TempIndexLis.Count);

        for(int i = 0; i < TempIndexLis.Count; i++)
        {
            //Debug.Log(TempIndexLis[i]);
        }


        return TMP[0];


    }

    bool CheckDuplication(int _index)
    {
        for (int j = 0; j < GuardIndex.Count; j++)
        {
            if (_index == GuardIndex[j])
            {
                //Debug.Log("重複" + _index);
                return false;
            }
        }

        return true;
    }

    // Start is called before the first frame update
    void Start()
    {
        create = FindObjectOfType<TestPlayerCreate>();

        create.OnPlayerCreate = (tmp) =>
        {
            if(PhotonNetwork.PlayerList.Length == 2 && TestOnlineData.PlayerID == 2)
            {
                for (int i = 0; i < GuardPos.Length; i++)
                {
                    randomIndex.Add(i);
                }

                for (int i = 0; i < InstantiateCount; i++)
                {
                    GuardIndex.Add(RandomGuardExtract());
                }

                for (int i = 0; i < GuardIndex.Count; i++)
                {
                    CreateGuard(GuardIndex[i]);
                }
            }

        };

        //this.UpdateAsObservable()
        //    .TakeUntilDestroy(this)
        //    .Where(_ => Input.GetMouseButtonDown(0))
        //    .Subscribe(_ => 
        //    {
        //        CreateGuardList[0].GuardDestroy();
        //    });

        //if (TestOnlineData.PlayerID == 2) return;

        //for(int i = 0; i < GuardPos.Length;i++)
        //{
        //    randomIndex.Add(i);
        //}

        //for(int i = 0; i < InstantiateCount; i++)
        //{
        //    GuardIndex.Add(RandomGuardExtract());
        //}

        //for(int i = 0; i<GuardIndex.Count;i++)
        //{
        //    CreateGuard(GuardIndex[i]);
        //}
    }

    void GuardChanger(Guard _guard)
    {
        _guard.DestroyAct = () => {
            CreateGuardList.Remove(_guard);
            GuardIndex.Remove(_guard.MyIndex);
            GuardIndex.Add(RandomGuardExtract());
            CreateGuard(GuardIndex[GuardIndex.Count - 1]);
        };
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(PhotonNetwork.PlayerList.Length);
    }

    void CreateGuard(int index)
    {
        //Guard _guard = PhotonNetwork.Instantiate("Barrier", GuardPos[index].transform.position, GuardPos[index].transform.rotation).GetComponent<Guard>();
        PhotonNetwork.Instantiate("Barrier", GuardPos[index].transform.position, GuardPos[index].transform.rotation);
        //_guard.MyIndex = index;
        //CreateGuardList.Add(_guard);

        //GuardChanger(_guard);
    }

    //オブジェクトを複数表示
    //オブジェクトを三回攻撃すると消える
}
