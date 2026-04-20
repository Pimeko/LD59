using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroController : MonoBehaviour
{
    [SerializeField]
    List<GameObject> titles;

    int index;

    private void Start()
    {
        index = 0;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (index == titles.Count - 1)
            {
                SceneManager.LoadScene(1);
            }
            else
            {
                titles[index].SetActive(false);
                index++;
                titles[index].SetActive(true);
            }
        }
    }
}
