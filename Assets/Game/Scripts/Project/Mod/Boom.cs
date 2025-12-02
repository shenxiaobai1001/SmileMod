using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boom : MonoBehaviour
{
    public ParticleSystem particleSystem;


    private void OnEnable()
    {
        particleSystem.Play();
    }
}
