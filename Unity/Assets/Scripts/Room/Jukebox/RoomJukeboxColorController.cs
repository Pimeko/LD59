using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class RoomJukeboxColorController : MonoBehaviour
{
    [SerializeField]
    Light2D light2D;
    [SerializeField]
    List<Color> colors;
    [SerializeField]
    float speed;

    int index;
    float t;

    void Update()
    {
        t += Time.deltaTime * speed;

        if (t >= 1f)
        {
            t = 0f;
            index = (index + 1) % colors.Count;
        }

        Color from = colors[index];
        Color to = colors[(index + 1) % colors.Count];

        light2D.color = Color.Lerp(from, to, t);
    }
}