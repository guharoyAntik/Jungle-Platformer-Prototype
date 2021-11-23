using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lock : MonoBehaviour
{
    [SerializeField] private GameObject _door;
    [SerializeField] private GameObject _attachedKey;

    public void Unlock()
    {
        //add animation
        _attachedKey.SetActive(true);
        Destroy(_door);
    }
}
