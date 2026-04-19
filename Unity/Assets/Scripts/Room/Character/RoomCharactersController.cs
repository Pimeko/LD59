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
    Vector2 delayBeforeInitialSpawn;

    List<RoomCharacterController> characterControllers;
    float elapsedInitialSpawn, currentDelayInitialSpawn;

    void Start()
    {
        characterControllers = new List<RoomCharacterController>();

        elapsedInitialSpawn = 0;
        currentDelayInitialSpawn = Random.Range(delayBeforeInitialSpawn.x, delayBeforeInitialSpawn.y);
    }

    void AddCharacter()
    {
        var instance = Instantiate(characterPrefab, characterInitialPosition.position, Quaternion.identity, charactersContainer);
        var characterController = instance.GetComponent<RoomCharacterController>();
        characterControllers.Add(characterController);
    }

    void Update()
    {
        elapsedInitialSpawn += Time.deltaTime;
        if (elapsedInitialSpawn > currentDelayInitialSpawn)
        {
            AddCharacter();

            elapsedInitialSpawn = 0;
            currentDelayInitialSpawn = Random.Range(delayBeforeInitialSpawn.x, delayBeforeInitialSpawn.y);
        }
    }
}
