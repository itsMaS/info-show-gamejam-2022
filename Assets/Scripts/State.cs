using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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
            Initialize();
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            DestroyImmediate(gameObject);
        }
    }

    private void Initialize()
    {
        _instance = this;
        Events = new GameEvents();
    }

    [SerializeField] GameConfigSO configSO;
    [SerializeField] PolygonCollider2D bounds;

    public bool PointWithinBounds(Vector2 point)
    {
        return bounds.OverlapPoint(point);
    }
    public static GameConfigSO config => State.Instance.configSO;
    public static GameEvents Events { get; private set; }
}

public class GameEvents
{
    public UnityEvent<Human, Genome, Genome> onMutation;

    public GameEvents()
    {
        onMutation = new UnityEvent<Human, Genome, Genome>();

        foreach (var human in GameObject.FindObjectsOfType<Human>())
        {
            human.OnMutate.AddListener((oldGenome, newGenome) => onMutation?.Invoke(human, oldGenome, newGenome));
        }
    }
}
