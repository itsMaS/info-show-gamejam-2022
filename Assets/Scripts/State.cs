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
            return _instance;
        }
    }


    private void Awake()
    {
        if(!_instance)
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

        SpawnInitialHumans();
    }

    private void SpawnInitialHumans()
    {
        int populationCount = config.gameplay.startingPopulationCount;
        for (int i = 0; i < populationCount; i++)
        {
            float offset = (float)i / populationCount;
            Vector2 direction = Vector2.up * 0.5f * Mathf.Sin(Mathf.PI * 2 * offset) + Vector2.right * Mathf.Cos(Mathf.PI * 2 * offset);
            SpawnHuman(direction * config.gameplay.rangeFromReactor, new Genome(true), config.gameplay.startingPopulationAge.PickRandom());
        }
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

    public void SpawnHuman(Vector2 position, Genome genome, float age = 0)
    {
        Transform editorParent = null;
#if UNITY_EDITOR
        editorParent = GameObject.Find("Humans").transform;
#endif
        Human human = Instantiate(humanPrefab.gameObject, position, Quaternion.identity, editorParent).GetComponent<Human>();

        human.Spawn(genome, age);
        Events.SubscribeToHumanEvents(human);
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
            SubscribeToHumanEvents(human);
        }
    }

    public void SubscribeToHumanEvents(Human human)
    {
            human.OnMutate.AddListener((oldGenome, newGenome) => onMutation?.Invoke(human, oldGenome, newGenome));
            human.onDragBegin.AddListener(() => onStartDraggingHuman.Invoke(human));
            human.onDragEnd.AddListener(() => onStopDraggingHuman.Invoke(human));

            human.onBreedHover.AddListener((h1, h2) => onCrossBreedHover.Invoke(h2,h1));
            human.onBreedUnhover.AddListener((h1, h2) => onCrossBreedUnhover.Invoke(h2,h1));
    }
}
