using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

struct ValueType
{
    public int? _int;
    public float? _float;
    public bool? _bool;
    public short? _short;
}

public class PhotonTest : MonoBehaviourPunCallbacks, IPunObservable
{
    List<ValueType> type = new List<ValueType>();

    List<int> _int = new List<int>();
    List<float> _float = new List<float>();
    List<bool> _bool = new List<bool>();
    List<char> _char = new List<char>();
    List<Vector3> _vec = new List<Vector3>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void UpdateValue<T>(ref T _value)
    {
        switch(_value)
        {
            case int n:
                _int.Add(n);
                break;

            case float f:
                _float.Add(f);
                break;

            case bool flg:
                _bool.Add(flg);
                break;

            case char c:
                _char.Add(c);
                break;

            case Vector3 vec:
                _vec.Add(vec);
                break;

            default:
                Debug.LogWarning("規定以外の型が送信されました。");
                break;
        }
    }

    public void StopValue<T>(T value)
    {

    }

    void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            foreach (int n in _int)         stream.SendNext(n);
            foreach (float f in _float)     stream.SendNext(f);
            foreach (bool flg in _bool)     stream.SendNext(flg);
            foreach (char c in _char)       stream.SendNext(c);
            foreach (Vector3 vec in _vec)   stream.SendNext(vec);
        }
        else
        {
            for (int i = 0; i < _int.Count; i++)
            {
                _int[i] = (int)stream.ReceiveNext();
            }
            for(int i = 0; i < _float.Count; i++)
            {
                _float[i] = (float)stream.ReceiveNext();
            }
            for(int i = 0; i < _bool.Count; i++)
            {
                _bool[i] = (bool)stream.ReceiveNext();
            }
            for(int i = 0; i < _char.Count; i++)
            {
                _char[i] = (char)stream.ReceiveNext();
            }
            for(int i=0; i < _vec.Count; i++)
            {
                _vec[i] = (Vector3)stream.ReceiveNext();
            }
        }
    }
}
