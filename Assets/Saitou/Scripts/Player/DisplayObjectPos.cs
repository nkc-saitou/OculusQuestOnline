using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Saitou.Player
{
    public enum ObjectPos
    {
        head,
        rightHand,
        leftHand
    }

    public class DisplayObjectPos : MonoBehaviour
    {
        [SerializeField] ObjectPos pos;
        public ObjectPos DisplayPos { get; private set; }
    }
}