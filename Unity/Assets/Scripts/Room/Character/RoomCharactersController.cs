using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomCharactersController : MonoBehaviour
{
    #region SINGLETON
    public static RoomCharactersController Instance;

    void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
        {
            Instance = this;

            DontDestroyOnLoad(gameObject);
        }
    }
    #endregion

    [SerializeField]
    GameObject characterPrefab;
    [SerializeField]
    Transform characterInitialPosition, charactersContainer;
    [SerializeField]
    Vector2 delayBeforeSpawn, delayHelpBubble, delayLeave;
    [SerializeField]
    float maxNbCharacters;

    public List<RoomCharacterController> characterControllers;
    float elapsedSpawn, currentDelaySpawn;
    float elapsedHelpBubble, currentDelayHelpBubble;
    float elapsedLeave, currentDelayLeave;

    bool isFinished;

    void Start()
    {
        characterControllers = new List<RoomCharacterController>();

        elapsedSpawn = 0;
        currentDelaySpawn = Random.Range(delayBeforeSpawn.x, delayBeforeSpawn.y);

        elapsedHelpBubble = 0;
        currentDelayHelpBubble = Random.Range(delayHelpBubble.x, delayHelpBubble.y);

        elapsedLeave = 0;
        currentDelayLeave = Random.Range(delayLeave.x, delayLeave.y);

        GameManager.Instance.OnAllKnobsOK += SpawnWinBubble;
        GameManager.Instance.OnNextMusic += OnNextMusic;
        GameManager.Instance.OnFinish += OnFinish;
    }

    void OnNextMusic()
    {
        elapsedLeave = 0;
    }

    void OnFinish()
    {
        isFinished = true;
    }

    void AddCharacter()
    {
        if (characterControllers.Count >= maxNbCharacters)
            return;

        var instance = Instantiate(characterPrefab, characterInitialPosition.position, Quaternion.identity, charactersContainer);
        var characterController = instance.GetComponent<RoomCharacterController>();
        characterControllers.Add(characterController);
    }

    void SpawnHelpBubble()
    {
        if (characterControllers.Count == 0)
            return;

        var randomCharacter = characterControllers.GetRandomItem();

        if (!GameManager.Instance.IsEffectOK)
        {
            randomCharacter.ShowBubble(RoomCharacterController.BubbleType.NOISE);
        }
        else if (!GameManager.Instance.IsPitchOK)
        {
            if (GameManager.Instance.IsPitchTooFast)
            {
                randomCharacter.ShowBubble(RoomCharacterController.BubbleType.FAST);
            }
            else
            {
                randomCharacter.ShowBubble(RoomCharacterController.BubbleType.SLOW);
            }
        }
        else if (!GameManager.Instance.IsVolumeOK)
        {
            randomCharacter.ShowBubble(RoomCharacterController.BubbleType.VOLUME);
        }
    }

    void SpawnWinBubble()
    {
        var randomCharacter = characterControllers.GetRandomItem();
        for (int i = 0; i < 3; i++)
            randomCharacter.ShowBubble(RoomCharacterController.BubbleType.WIN);
    }

    void MakeOneLeave()
    {
        if (!GameManager.Instance.IsAllOK)
        {
            if (characterControllers.Count == 0)
                return;

            var randomCharacterIndex = Random.Range(0, characterControllers.Count);
            var randomCharacter = characterControllers[randomCharacterIndex];
            randomCharacter.Leave();
            characterControllers.RemoveAt(randomCharacterIndex);
        }
    }

    void Update()
    {
        if (isFinished)
            return;

        elapsedSpawn += Time.deltaTime;
        if (elapsedSpawn > currentDelaySpawn)
        {
            AddCharacter();

            elapsedSpawn = 0;
            currentDelaySpawn = Random.Range(delayBeforeSpawn.x, delayBeforeSpawn.y);
        }

        elapsedHelpBubble += Time.deltaTime;
        if (elapsedHelpBubble > currentDelayHelpBubble)
        {
            SpawnHelpBubble();

            elapsedHelpBubble = 0;
            currentDelayHelpBubble = Random.Range(delayHelpBubble.x, delayHelpBubble.y);
        }

        elapsedLeave += Time.deltaTime;
        if (elapsedLeave > currentDelayLeave)
        {
            MakeOneLeave();

            elapsedLeave = 0;
            currentDelayHelpBubble = Random.Range(delayLeave.x, delayLeave.y);
        }
    }

    void OnDestroy()
    {
        GameManager.Instance.OnAllKnobsOK -= SpawnWinBubble;
        GameManager.Instance.OnNextMusic -= OnNextMusic;
        GameManager.Instance.OnFinish -= OnFinish;
    }
}
