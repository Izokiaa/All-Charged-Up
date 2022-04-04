using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmokeController : MonoBehaviour
{
    ParticleSystem smokeParticles;
    ParticleSystem.EmissionModule emissionRate;
    ParticleSystem.EmissionModule minEmissionRate;
    ParticleSystem.EmissionModule maxEmissionRate;
    // Start is called before the first frame update
    void Start()
    {
        smokeParticles = GetComponent<ParticleSystem>();
        emissionRate = smokeParticles.emission;
        emissionRate.rateOverTime = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if ((CarController2.ins.braking && CarController2.ins.rb.velocity.magnitude > 3f) || Mathf.Abs(CarController2.ins.steeringInput) > 0.97f)
        {
            emissionRate.rateOverTime = 75;
        }
        else
            emissionRate.rateOverTime = 0;
    }
}
