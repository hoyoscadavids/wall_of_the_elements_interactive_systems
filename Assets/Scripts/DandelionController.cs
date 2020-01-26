using UnityEngine;


public class DandelionController : MonoBehaviour {
    private ParticleSystem particles;
    private ParticleSystem.Particle[] individualParticles;
    private WindZone attractor;
    private float time = 5f;
    private float time2 = 5f;
    private bool wasTriggered = false;
    private bool wasTriggered2 = false;
    private int windMain = 10;
    // Use this for initialization
    void Awake () {
        particles = GameObject.Find("Dandelion").GetComponent<ParticleSystem>();
        attractor = GameObject.Find("Spring_attractor").GetComponent<WindZone>();
	}

    private void Update()
    {
        print(wasTriggered + "" + wasTriggered2);
       
        if (wasTriggered && !wasTriggered2)
        {
            time -= Time.deltaTime;
            attractor.windMain = 0;
            time2 = 5f;
        }
        if (time <= 0f) {
            wasTriggered2 = true;
            attractor.windMain = windMain;
        }
        if (wasTriggered2) {
            time2 -= Time.deltaTime;
        }
        if (time2 <= 0f) {
            wasTriggered2 = false;
            wasTriggered = false;
            time = 5f;
        }
       
    }

    private void OnCollisionExit(Collision collider)
    {
        if (!wasTriggered)
        {
            wasTriggered = true;
        }
    }
}
