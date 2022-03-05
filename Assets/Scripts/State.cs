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
    private void Start()
    {
        Events.Start();
    }

    private void Initialize()
    {
        _instance = this;
        Events = new GameEvents();
    }

    [SerializeField] GameConfigSO configSO;
    [SerializeField] PolygonCollider2D bounds;
    [SerializeField] Human humanPrefab;

    public bool PointWithinBounds(Vector2 point)
    {
        return bounds.OverlapPoint(point);
    }

    public void SpawnHuman(Vector2 position, Genome genome)
    {
        Transform editorParent = null;
#if UNITY_EDITOR
        editorParent = GameObject.Find("Humans").transform;
#endif
        Human human = Instantiate(humanPrefab, position, Quaternion.identity, editorParent).GetComponent<Human>();

        human.Spawn(genome);
    }


    public static GameConfigSO config => State.Instance.configSO;
    public static GameEvents Events { get; private set; }
}

public class GameEvents
{
    public UnityEvent<Human, Genome, Genome> onMutation = new UnityEvent<Human, Genome, Genome>();
    public UnityEvent<Human> onStartDraggingHuman = new UnityEvent<Human>();
    public UnityEvent<Human> onStopDraggingHuman = new UnityEvent<Human>();
    public UnityEvent<Human, Human, Genome> OnCrossBreed = new UnityEvent<Human, Human, Genome>();

    public UnityEvent<Human, Human> onCrossBreedHover = new UnityEvent<Human, Human>();
    public UnityEvent<Human, Human> onCrossBreedUnhover = new UnityEvent<Human, Human>();

    public void Start()
    {
        foreach (var human in GameObject.FindObjectsOfType<Human>())
        {
            human.OnMutate.AddListener((oldGenome, newGenome) => onMutation?.Invoke(human, oldGenome, newGenome));
            human.onDragBegin.AddListener(() => onStartDraggingHuman.Invoke(human));
            human.onDragEnd.AddListener(() => onStopDraggingHuman.Invoke(human));

            human.target.onInteractableDragHover.AddListener(interactable => onCrossBreedHover.Invoke((Human)interactable, human));
            human.target.onInteractableDragUnhover.AddListener(interactable => onCrossBreedUnhover.Invoke((Human)interactable, human));
        }
    }
}
