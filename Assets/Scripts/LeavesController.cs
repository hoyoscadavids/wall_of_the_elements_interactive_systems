using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeavesController : MonoBehaviour {

    private ParticleSystem leavesParticle;
    private float time = 1f;
    private bool activateTime = false;


    private void Awake()
    {
        leavesParticle = gameObject.GetComponent<ParticleSystem>();
    }

    private void Update()
    {
        if (activateTime)
        {
            time -= Time.deltaTime;
            print("time");
            if(time <= 0f)
            {
                print("TIMEOUT");
                var force = leavesParticle.forceOverLifetime;
                force.enabled = false;
                activateTime = false;
                time = 1f;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!activateTime)
        {
            var force = leavesParticle.forceOverLifetime;
            force.enabled = true;
            activateTime = true;
            leavesParticle.gravityModifier = 0.7f;
        }
        
    }

}
