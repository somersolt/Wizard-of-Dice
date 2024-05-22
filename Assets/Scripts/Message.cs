using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Build.Content;
using UnityEngine;
using UnityEngine.UI;


public class Message : MonoBehaviour
{
    TextMeshProUGUI text;
    float time = 0;
    float duration = 1f;
    bool onWorld;

    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
    }
    void Update()
    {
        time += Time.deltaTime;
        if (onWorld)
        {
            gameObject.transform.position += new Vector3(0, 0.002f, 0);
        }
        else
        {
            gameObject.transform.position += new Vector3(0, 0.3f, 0);
        }
        if ( time > duration)
        {
            Destroy(gameObject);
        }
    }

    public void Setup(int damage, Color color, bool setWorld = false)
    {
        onWorld = setWorld;
        text.text = damage.ToString();
        text.color = color;
    }

    public void Setup(string message, Color color, bool setWorld = false)
    {
        onWorld = setWorld;
        text.text = message;
        text.color = color;
    }
}
