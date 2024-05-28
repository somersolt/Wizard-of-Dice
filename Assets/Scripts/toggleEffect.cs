using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class toggleEffect : MonoBehaviour
{
    public ParticleSystem effect;


    private void Update()
    {
        effect.transform.position = transform.position;
    }
}
