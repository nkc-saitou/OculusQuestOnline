using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nakajima.Player;

/// <summary>
/// Playerが持つべきInterface
/// </summary>
namespace Nakajima.Player
{
    public interface IPlayer
    {
        /// <summary>
        /// 自分のPlayerIDを返す
        /// </summary>
        /// <returns>PlayerID</returns>
        int GetMyPlayerID();

        /// <summary>
        /// 自分の手の情報を返す
        /// </summary>
        /// <returns>自分の手(両手)</returns>
        PlayerHand[] GetMyHands();
    }
}


