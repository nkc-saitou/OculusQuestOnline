using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class TestOnlineData : MonoBehaviour
{
    public static int PlayerID { get; set; } = 0;

    public static ReactiveProperty<int> PlayerCount { get; set; } = new ReactiveProperty<int>();
}
