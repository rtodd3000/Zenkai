using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MusicHandler : MonoBehaviour 
{
    void Awake() {
        DontDestroyOnLoad(this);
    }
}
