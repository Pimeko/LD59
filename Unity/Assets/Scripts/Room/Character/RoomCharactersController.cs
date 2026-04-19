using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomCharactersController : MonoBehaviour
{
    [SerializeField]
    GameObject characterPrefab;
    [SerializeField]
    Transform characterInitialPosition, charactersContainer;
    [SerializeField]
    Vector2 delayBeforeSpawn, delayHelpBubble;
    [SerializeField]
    float maxNbCharacters;

    List<RoomCharacterController> characterControllers;
    float elapsedSpawn, currentDelaySpawn;
    float elapsedHelpBubble, currentDelayHelpBubble;

    void Start()
    {
        characterControllers = new List<RoomCharacterController>();

        elapsedSpawn = 0;
        currentDelaySpawn = Random.Range(delayBeforeSpawn.x, delayBeforeSpawn.y);

        elapsedHelpBubble = 0;
        currentDelayHelpBubble = Random.Range(delayHelpBubble.x, delayHelpBubble.y);
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
        print(characterControllers.Count);
        var randomCharacter = characterControllers.GetRandomItem();
        
        if (!GameManager.Instance.IsEffectOK)
        {
            randomCharacter.ShowBubble(RoomCharacterController.BubbleType.NOISE);
            print("spawn help effect");
        }
        else if (!GameManager.Instance.IsPitchOK)
        {
            if (GameManager.Instance.IsPitchTooFast)
            {
                randomCharacter.ShowBubble(RoomCharacterController.BubbleType.FAST);
                print("spawn help FAST");
            }
            else
            {
                randomCharacter.ShowBubble(RoomCharacterController.BubbleType.SLOW);
                print("spawn help SLOW");
            }
        }
        else if (!GameManager.Instance.IsVolumeOK)
        {
            randomCharacter.ShowBubble(RoomCharacterController.BubbleType.VOLUME);
            print("spawn help VOLUME");
        }

    }

    void Update()
    {
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
    }
}
