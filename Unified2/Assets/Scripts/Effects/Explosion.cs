using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    private ParticleSystem ps;

    void Start()
    {
        ps = GetComponent<ParticleSystem>();
        ps.Play();
        GetComponent<AudioSource>().Play();
    }

    void Update()
    {
        Destroy(this.gameObject, ps.main.duration);
    }

}
