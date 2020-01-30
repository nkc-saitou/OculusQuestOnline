using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nakajima.Main;
public class LobbyManager : MonoBehaviour
{
    MainManager mainManager;
    // Start is called before the first frame update
    void Start()
    {
        mainManager = FindObjectOfType<MainManager>();
        mainManager.playerEntry();
    }
}
