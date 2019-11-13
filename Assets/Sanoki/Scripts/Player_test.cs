using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player_test : MonoBehaviourPunCallbacks
{
    List<GameObject> primitivelist =new List<GameObject>();

    public Text testText;
    Sanoki.Online.GameStartSystem gs;

    private void Start()
    {
        gs = FindObjectOfType<Sanoki.Online.GameStartSystem>();
    }

    // 部屋に入室した時
    public override void OnJoinedRoom()
    {
        //SpawnSphere();
        // ランダムな位置にキューブを生成
        int x = Random.Range(-5, 6);
        int y = Random.Range(-4, 3);

        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.Instantiate("Prefabs/Sphere", new Vector3(x, y, 0), Quaternion.identity);
        }
    }

    // Update is called once per frame
    void Update()
    {
        testText.text = "入室人数: " + PhotonNetwork.PlayerList.Length.ToString() + " / 2";

        if (Input.GetKeyDown(KeyCode.G))
        {
            gs.Entry();
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            SpawnSphere();

        }
    }

    IEnumerator Obj_Destroy(GameObject obj)
    {

        yield return new WaitForSeconds(5.0f);
        Destroy(obj);
    }

    void SpawnSphere()
    {
        primitivelist.Add(GameObject.CreatePrimitive(PrimitiveType.Sphere));
        primitivelist[primitivelist.Count - 1].AddComponent<PhotonView>();
        primitivelist[primitivelist.Count - 1].transform.position = new Vector3(Random.Range(-1.0f, 1.0f), 0, Random.Range(-1.0f, 1.0f));
        primitivelist[primitivelist.Count - 1].AddComponent<Rigidbody>();
        primitivelist[primitivelist.Count - 1].AddComponent<Renderer>();
        Renderer ren = primitivelist[primitivelist.Count - 1].GetComponent<Renderer>();
        ren.material.color = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));

        StartCoroutine(Obj_Destroy(primitivelist[primitivelist.Count - 1]));

    }
}
