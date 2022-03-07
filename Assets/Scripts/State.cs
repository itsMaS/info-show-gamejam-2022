using System;
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
            Debug.LogError("State already exists, destroying this one");
            DestroyImmediate(gameObject);
        }
    }
    private void Start()
    {
        Events.Start();
#if UNITY_EDITOR
        LoadLevel(startLevel);
#else
        LoadLevel(0);
#endif

        Events.OnPlaceHumanInReactor.AddListener(PlaceInReactor);
    }

    private void PlaceInReactor(Human arg0)
    {
        bool pass = true;
        foreach (var req in CurrentLevelData.Requirements)
        {
            arg0.genome.TryGetGeneValue(req.data, out float value);
            if(value < req.range.x || value > req.range.y)
            {
                pass = false;
            }
        }

        if(pass)
        {
            levelAmount++;

            if(levelAmount >= CurrentLevelData.amount)
            {
                CompleteLevel();
            }
        }
    }

    public int levelAmount { get; private set; }
    public LevelRequirementSO CurrentLevelData => Levels[CurrentLevelIndex];

    [SerializeField] int startLevel = 0;

    public List<Human> Humans = new List<Human>();

    public void LoadLevel(int level)
    {
        levelAmount = 0;
        CurrentLevelIndex = level;
        Events.OnLoadLevel.Invoke(CurrentLevelData);
    }

    public void CompleteLevel()
    {
        Events.OnLevelComplete.Invoke();
        LoadLevel(CurrentLevelIndex + 1);
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

    public Human SpawnHuman(Vector2 position, Genome genome, float age = 0)
    {
        if(Humans.Count >= config.gameplay.maxHumans)
        {
            Human hum = Humans.PickRandom();
            hum.Die();
            Humans.Remove(hum);
        }

        Transform editorParent = null;
#if UNITY_EDITOR
        editorParent = GameObject.Find("Humans").transform;
#endif
        Human human = Instantiate(humanPrefab.gameObject, position, Quaternion.identity, editorParent).GetComponent<Human>();

        human.Spawn(genome, age);

        Humans.Add(human);
        human.OnDeath.AddListener(() => Humans.Remove(human));

        Events.SubscribeToHumanEvents(human);
        Events.OnHumanSpawn.Invoke(human);
        return human;
    }


    public static GameConfigSO config => State.Instance.configSO;
    public static GameEvents Events { get; private set; }

    public List<LevelRequirementSO> Levels;
    public int CurrentLevelIndex { get; private set; }
}

public class GameEvents
{
    public UnityEvent<Human> OnHoverHumanOverReactor = new UnityEvent<Human>();
    public UnityEvent<Human> OnUnhoverHumanOverReactor = new UnityEvent<Human>();
    public UnityEvent OnReactorExplosion = new UnityEvent();


    public UnityEvent<Human> OnPlaceHumanInReactor = new UnityEvent<Human>();
    public UnityEvent<Human> OnHumanDeath = new UnityEvent<Human>();
    public UnityEvent<Human> OnHumanSpawn = new UnityEvent<Human>();
    public UnityEvent OnLevelComplete = new UnityEvent();

    public UnityEvent<LevelRequirementSO> OnLoadLevel = new UnityEvent<LevelRequirementSO>();

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

        SubscribeToReactor(GameObject.FindObjectOfType<Reactor>());
    }

    private void SubscribeToReactor(Reactor reactor)
    {
        reactor.OnHumanHover.AddListener(human => OnHoverHumanOverReactor.Invoke(human));
        reactor.OnHumanUnhover.AddListener(human => OnUnhoverHumanOverReactor.Invoke(human));
        reactor.OnHumanPlacedInside.AddListener(human => OnPlaceHumanInReactor.Invoke(human));
        reactor.onExplode.AddListener(() => OnReactorExplosion.Invoke());
    }
    public void SubscribeToHumanEvents(Human human)
    {
        human.OnMutate.AddListener((oldGenome, newGenome) => onMutation?.Invoke(human, oldGenome, newGenome));
        human.onDragBegin.AddListener(() => onStartDraggingHuman.Invoke(human));
        human.onDragEnd.AddListener(() => onStopDraggingHuman.Invoke(human));

        human.onBreedHover.AddListener((h1, h2) => onCrossBreedHover.Invoke(h2,h1));
        human.onBreedUnhover.AddListener((h1, h2) => onCrossBreedUnhover.Invoke(h2,h1));
        human.OnDeath.AddListener(() => OnHumanDeath.Invoke(human));
    }
}
