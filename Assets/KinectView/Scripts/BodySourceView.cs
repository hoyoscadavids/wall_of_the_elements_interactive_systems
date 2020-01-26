using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Kinect = Windows.Kinect;

public class BodySourceView : MonoBehaviour
{
    /// <summary>
    /// 
    /// Seasons: 
    /// Winter: 0
    /// Spring: 1
    /// Summer: 2
    /// Fall: 3
    /// 
    /// </summary>
    // My Variables
    public GameObject capsulePrefab;
    public GameObject spherePrefab;
    public GameObject fogPrefab;
    public GameObject firePrefab;
    public int season;
    private List<GameObject> bodyColliders;
    private GameObject fog;
    private GameObject leaves;
    private GameObject fallingLeaves;
    private GameObject fallingSnow;

    private GameObject winter, spring, summer, fall;
    private GameObject snowManObject; 
    public GameObject snowmanPrefab = null;
    public Material BoneMaterial;
    public GameObject BodySourceManager;

    private Dictionary<ulong, GameObject> _Bodies = new Dictionary<ulong, GameObject>();
    private BodySourceManager _BodyManager;
    private Dictionary<ulong, List<GameObject>> physicalBodies = new Dictionary<ulong, List<GameObject>>();
    private Dictionary<ulong, List<GameObject>> fogParticles = new Dictionary<ulong, List<GameObject>>();
    private Dictionary<ulong, List<GameObject>> fireParticles = new Dictionary<ulong, List<GameObject>>();

    public int fullLoop = 120;
    private int loopTimer;

    private Dictionary<Kinect.JointType, Kinect.JointType> _BoneMap = new Dictionary<Kinect.JointType, Kinect.JointType>()
    {
        { Kinect.JointType.FootLeft, Kinect.JointType.AnkleLeft },
        { Kinect.JointType.AnkleLeft, Kinect.JointType.KneeLeft },
        { Kinect.JointType.KneeLeft, Kinect.JointType.HipLeft },
        { Kinect.JointType.HipLeft, Kinect.JointType.SpineBase },

        { Kinect.JointType.FootRight, Kinect.JointType.AnkleRight },
        { Kinect.JointType.AnkleRight, Kinect.JointType.KneeRight },
        { Kinect.JointType.KneeRight, Kinect.JointType.HipRight },
        { Kinect.JointType.HipRight, Kinect.JointType.SpineBase },

        { Kinect.JointType.HandTipLeft, Kinect.JointType.HandLeft },
        { Kinect.JointType.ThumbLeft, Kinect.JointType.HandLeft },
        { Kinect.JointType.HandLeft, Kinect.JointType.WristLeft },
        { Kinect.JointType.WristLeft, Kinect.JointType.ElbowLeft },
        { Kinect.JointType.ElbowLeft, Kinect.JointType.ShoulderLeft },
        { Kinect.JointType.ShoulderLeft, Kinect.JointType.SpineShoulder },

        { Kinect.JointType.HandTipRight, Kinect.JointType.HandRight },
        { Kinect.JointType.ThumbRight, Kinect.JointType.HandRight },
        { Kinect.JointType.HandRight, Kinect.JointType.WristRight },
        { Kinect.JointType.WristRight, Kinect.JointType.ElbowRight },
        { Kinect.JointType.ElbowRight, Kinect.JointType.ShoulderRight },
        { Kinect.JointType.ShoulderRight, Kinect.JointType.SpineShoulder },

        { Kinect.JointType.SpineBase, Kinect.JointType.SpineMid },
        { Kinect.JointType.SpineMid, Kinect.JointType.SpineShoulder },
        { Kinect.JointType.SpineShoulder, Kinect.JointType.Neck },
        { Kinect.JointType.Neck, Kinect.JointType.Head },
    };

    private void Awake()
    {
        loopTimer = fullLoop;
        Time.timeScale = 1;
        bodyColliders = new List<GameObject>();
        fallingSnow = GameObject.Find("Snow_on_top");
        leaves = GameObject.Find("leaves_on_top");
        fallingLeaves = GameObject.Find("leaves");

        winter = GameObject.Find("winter");
        spring = GameObject.Find("spring");
        summer = GameObject.Find("summer_lensFlare");
        fall = GameObject.Find("fall");

        winter.SetActive(false);
        spring.SetActive(false);
        summer.SetActive(false);
        fall.SetActive(false);

        StartCoroutine(Timer());
    }
    void Update()
    {
        if(snowManObject != null)
        {
            snowManObject.GetComponent<Animator>().ResetTrigger("fadeIn");
            snowManObject.GetComponent<Animator>().ResetTrigger("fadeOut");
        }

        spring.GetComponent<Animator>().ResetTrigger("fadeIn");
        spring.GetComponent<Animator>().ResetTrigger("fadeOut");

        summer.GetComponent<Animator>().ResetTrigger("fadeIn");
        summer.GetComponent<Animator>().ResetTrigger("fadeOut");

        fall.GetComponent<Animator>().ResetTrigger("fadeIn");
        fall.GetComponent<Animator>().ResetTrigger("fadeOut");
        /// Change seasons and loop every 2 minutes
        if (loopTimer == fullLoop) {
            foreach (var fog in fogParticles)
            {
                for (int j = 0; j < fog.Value.Count; j++)
                {
                    fog.Value[j].SetActive(false);
                }
                foreach (var fire in fireParticles)
                {
                    for (int j = 0; j < fire.Value.Count; j++)
                    {
                        fire.Value[j].SetActive(false);
                    }
                }
            }
            // WINTER
            // Fade out fall
            fall.GetComponent<Animator>().SetTrigger("fadeOut");
            //////////////////////////////

            spring.SetActive(false);
            summer.SetActive(false);

            winter.SetActive(true);
            season = 0;
            if(snowManObject == null)
            {
                snowManObject = Instantiate(snowmanPrefab);
                snowManObject.name = "Snowman";
            }
        }
        else if (loopTimer == fullLoop - (fullLoop / 4) )
        {
            // SPRING
            // Fade out winter
            if(snowManObject != null)
            {
                snowManObject.GetComponent<Animator>().SetTrigger("fadeOut");
            }
            GameObject snowMan = GameObject.Find("Snowman");
            ParticleSystem particlesOnTop = GameObject.Find("Snow_on_top").GetComponent<ParticleSystem>();
            ParticleSystem particles = GameObject.Find("Snow").GetComponent<ParticleSystem>();

            Rigidbody[] snowManChildren = snowMan.GetComponentsInChildren<Rigidbody>();
            foreach (Rigidbody child in snowManChildren) {
                child.AddForce(new Vector3(-1500f, 0f, 0f));
            }
            particles.Stop();
            particlesOnTop.Stop();

            /////////////////////////////////

            // Fade in Spring
            spring.GetComponent<Animator>().SetTrigger("fadeIn");
            /////////////////////////////////
            summer.SetActive(false);
            fall.SetActive(false);

            spring.SetActive(true);
            season = 1;
        }
        else if (loopTimer == fullLoop / 2 )
        {
            // SUMMER
            // Fade Out Spring
            spring.GetComponent<Animator>().SetTrigger("fadeOut");
            //////////////////////////
            summer.GetComponent<Animator>().SetTrigger("fadeIn");

            winter.SetActive(false);
            Destroy(snowManObject);
            snowManObject = null;

            fall.SetActive(false);
            summer.SetActive(true);

            foreach (var fog in fogParticles)
            {
                for (int j = 0; j < fog.Value.Count; j++)
                {
                    fog.Value[j].SetActive(true);
                }
                foreach (var fire in fireParticles)
                {
                    for (int j = 0; j < fire.Value.Count; j++)
                    {
                        fire.Value[j].SetActive(true);
                    }
                }
            }

            season = 2;
        }
        else if (loopTimer == fullLoop / 4)
        {
            // FALL
            // Fade out Summer
            summer.GetComponent<Animator>().SetTrigger("fadeOut");
            foreach (var fog in fogParticles)
            {
                for (int j = 0; j < fog.Value.Count; j++)
                {
                    fog.Value[j].GetComponent<ParticleSystem>().Stop();

                }
                foreach (var fire in fireParticles)
                {
                    for (int j = 0; j < fire.Value.Count; j++)
                    {
                        fire.Value[j].GetComponent<ParticleSystem>().Stop();
                    }
                }
            }
            //////////////////////

            fall.GetComponent<Animator>().SetTrigger("fadeIn");
            winter.SetActive(false);
            spring.SetActive(false);

            fall.SetActive(true);
            season = 3;
        }
        else if(loopTimer <= 0){
 
            // START AGAIN
            season = 0;
            loopTimer = fullLoop;
        }
         
        if (BodySourceManager == null)
        {
            return;
        }

        _BodyManager = BodySourceManager.GetComponent<BodySourceManager>();
        if (_BodyManager == null)
        {
            return;
        }

        Kinect.Body[] data = _BodyManager.GetData();
        if (data == null)
        {
            return;
        }

        List<ulong> trackedIds = new List<ulong>();
        foreach (var body in data)
        {
            if (body == null)
            {
                continue;
            }

            if (body.IsTracked)
            {
                trackedIds.Add(body.TrackingId);
            }
        }

        List<ulong> knownIds = new List<ulong>(_Bodies.Keys);

        // First delete untracked bodies
        foreach (ulong trackingId in knownIds)
        {
            if (!trackedIds.Contains(trackingId))
            {
                Destroy(_Bodies[trackingId]);
                _Bodies.Remove(trackingId);

                foreach (GameObject bodyPart in physicalBodies[trackingId]) {
                    Destroy(bodyPart);
                }
                physicalBodies.Remove(trackingId);

               foreach (GameObject bodyPart in fogParticles[trackingId])
                 {
                    Destroy(bodyPart);
                 }
                  fogParticles.Remove(trackingId);
                    foreach (GameObject bodyPart in fireParticles[trackingId])
                  {
                        Destroy(bodyPart);
                 }
                    fireParticles.Remove(trackingId);
            }
        }

        foreach (var body in data)
        {
           
            if (body == null)
            {
                continue;
            }

            if (body.IsTracked)
            {
                if (!_Bodies.ContainsKey(body.TrackingId))
                {
                    _Bodies[body.TrackingId] = CreateBodyObject(body.TrackingId);
                   
                }

                RefreshBodyObject(body, _Bodies[body.TrackingId], body);
            }
        }
    }

    private GameObject CreateBodyObject(ulong id)
    {
        GameObject body = new GameObject("Body:" + id);
        List<GameObject> physicalBody = new List<GameObject>();
        List<GameObject> fogParticleSystem = new List<GameObject>();
        List<GameObject> fireParticleSystem = new List<GameObject>();
        physicalBodies[id] = physicalBody;
        
        int counter = 0;
        for (Kinect.JointType jt = Kinect.JointType.SpineBase; jt <= Kinect.JointType.ThumbRight; jt++)
        {
            // Capsules and Sphere for the colliders

             GameObject jointObj;

              if(jt == Kinect.JointType.Head)
              {
                jointObj = Instantiate(spherePrefab);
              }
              else
              {
                jointObj = Instantiate(capsulePrefab);
            }
            

            if ((int)jt <= 11 && (int)jt != 6 && (int)jt != 10 && (int)jt != 3)
                {
                    fogParticleSystem.Add(Instantiate(fogPrefab));
                    fireParticleSystem.Add(Instantiate(firePrefab));
                }

            for (int i = 0; i < fogParticleSystem.Count; i++)
            {
                fogParticleSystem[i].SetActive(false);
                fireParticleSystem[i].SetActive(false);
            }

            physicalBody.Add(jointObj);
            jointObj.name = jt.ToString();
            jointObj.transform.parent = body.transform;
            counter++;  
        }

        fogParticles[id] = fogParticleSystem;
        fireParticles[id] = fireParticleSystem;
        return body;

    }

    private void RefreshBodyObject(Kinect.Body body, GameObject bodyObject, Kinect.Body fullBody)
    {
        int counter = 0;
        for (Kinect.JointType jt = Kinect.JointType.SpineBase; jt <= Kinect.JointType.ThumbRight; jt++)
        {
            Kinect.Joint sourceJoint = body.Joints[jt];
            Kinect.Joint? targetJoint = null;
            

            if (_BoneMap.ContainsKey(jt))
            {
                targetJoint = body.Joints[_BoneMap[jt]];
            }

            Transform jointObj = bodyObject.transform.Find(jt.ToString());
            jointObj.localPosition = GetVector3FromJoint(sourceJoint);

            switch (season)
            {
                case 0:
                    if (jt == Kinect.JointType.SpineBase)
                    {
                        fallingSnow.transform.position = new Vector3(
                            fallingSnow.transform.position.x,
                            fallingSnow.transform.position.y,
                            jointObj.localPosition.z
                            );
                    }
                    break;
                case 1:
                    break;
                case 2:
                    break;
                case 3:
                    if (jt == Kinect.JointType.SpineBase)
                    {
                        leaves.transform.localPosition = new Vector3(leaves.transform.localPosition.x,
                            leaves.transform.localPosition.y, 
                            jointObj.localPosition.z);
                    }
                    break;
            }

            List<GameObject> bodyColliders = physicalBodies[body.TrackingId];
            LineRenderer lr = jointObj.GetComponent<LineRenderer>();
           
            if (targetJoint.HasValue)
            {
                bodyColliders[(int)jt].transform.rotation = new Quaternion(0, 0, 0, 1);

                if (!(jt == Kinect.JointType.Head))
            
                {
                    float rotationX = fullBody.JointOrientations[jt].Orientation.X;
                    float rotationY = fullBody.JointOrientations[jt].Orientation.Y;
                    float rotationZ = fullBody.JointOrientations[jt].Orientation.Z;
                    float rotationW = fullBody.JointOrientations[jt].Orientation.W;

                    Vector3 size = jointObj.localPosition - GetVector3FromJoint(targetJoint.Value);
                    Vector3 scale = new Vector3(1f, Vector3.Magnitude(size) / 1.5f, 1f);

                    bodyColliders[(int)jt].transform.localRotation = new Quaternion(rotationX, rotationY, rotationZ, rotationW);
                    bodyColliders[(int)jt].transform.localScale = scale;


                    if ((int)jt <= 11 && (int)jt != 6 && (int)jt != 10 && (int)jt != 10)
                    {
                        List<GameObject> fogParticleSystem = fogParticles[body.TrackingId];
                        List<GameObject> fireParticleSystem = fireParticles[body.TrackingId];

                        Vector3 rotation = new Quaternion(rotationX, rotationY, rotationZ, rotationW).eulerAngles;
                        fogParticleSystem[counter].transform.localPosition = jointObj.transform.localPosition;
                        fogParticleSystem[counter].GetComponent<ParticleSystem>().shape.rotation.Set(rotation.x, rotation.y, rotation.z);
                        fogParticleSystem[counter].GetComponent<ParticleSystem>().shape.scale.Set(size.x, size.y * 10f, size.z);



                        fireParticleSystem[counter].transform.localPosition = jointObj.transform.localPosition;
                        fireParticleSystem[counter].GetComponent<ParticleSystem>().shape.rotation.Set(rotation.x, rotation.y, rotation.z);

                        counter++;

                    }
                }
             
            }
        }
    }

    private static Color GetColorForState(Kinect.TrackingState state)
    {
        switch (state)
        {
            case Kinect.TrackingState.Tracked:
                return Color.green;

            case Kinect.TrackingState.Inferred:
                return Color.red;

            default:
                return Color.black;
        }
    }

    private static Vector3 GetVector3FromJoint(Kinect.Joint joint)
    {
        int value = 10;
        return new Vector3(joint.Position.X *value, joint.Position.Y * value, joint.Position.Z * value);
    }


    IEnumerator Timer()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            loopTimer--;
        }
    }
}



