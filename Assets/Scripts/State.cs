using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State : MonoBehaviour
{
    private static State _instance;

    public static State Instance
    {
        get
        {
#if UNITY_EDITOR
            if(!Application.isPlaying)
                return FindObjectOfType<State>();
            else
#endif
                return _instance;
        }
    }

    private void Awake()
    {
        if(!Instance)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            DestroyImmediate(gameObject);
        }
    }

    [SerializeField] GameConfigSO configSO;


    public static GameConfigSO config => State.Instance.configSO;
}
