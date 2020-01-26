using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireController : MonoBehaviour {
    private ParticleSystem particles;
    private float initialThreshold = -0.1f;
    private float threshold;
    private bool hasActivated; 
    private Light fireLight;
    // Use this for initialization
    void Start()
    {
        threshold = initialThreshold;
        particles = gameObject.GetComponent<ParticleSystem>();
        particles.Stop();
        fireLight = gameObject.GetComponentInChildren<Light>();
        hasActivated = false;
    }

    private void OnEnable()
    {
        threshold = initialThreshold;
        particles = gameObject.GetComponent<ParticleSystem>();
        particles.Stop();
        fireLight = gameObject.GetComponentInChildren<Light>();
        hasActivated = false;

    }

    private void OnDisable()
    {
        fireLight.intensity = 0;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        threshold -= 0.0025f;
        if (threshold <= -0.98) {
            fireLight.intensity = 100;
        }
        if (threshold <= -1.0f && !hasActivated)
        {
            fireLight.intensity = 0;
            particles.Play();
            threshold = initialThreshold;
            hasActivated = true;
        }
    }
}
