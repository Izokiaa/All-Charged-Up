using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
public class CarController2 : MonoBehaviour
{
    [HideInInspector] public Rigidbody2D rb;
    Animator anim;
    [Header("Camera")]
    CinemachineVirtualCamera cm;
    public GameObject viewTarget;
    public float cameraRotationLerpTime;
    [Header("Car Movement new")]
    public float driftFactor = 0.5f;
    public float accerationFactor = 30f;
    public float turnFactor = 3.5f;
    public float minSteerFactor = 8;
    public float deaccel = 5;
    public bool braking = false;
    [HideInInspector]public float accelInput = 0;
    [HideInInspector] public float steeringInput = 0;
    float rotationAngle = 0;
    [Header("Tire Anims")]
    public GameObject[] frontTires;
    [Header("AudioSources")]
    AudioSource engineAudio;
    public AudioSource driftAudio;
    public static CarController2 ins;
    [Header("GroundTypes")]
    public LayerMask road;
    public bool onRoad;
    [Header("Timers")]
    Timer endBoostTimer;
    public float endBoostTime;
    void Awake()
    {
        ins = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        cm = GameObject.FindGameObjectWithTag("CM").GetComponent<CinemachineVirtualCamera>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        engineAudio = GetComponent<AudioSource>();
        viewTarget = transform.GetChild(0).gameObject;
        transform.GetChild(0).parent = null;

        endBoostTimer = gameObject.AddComponent<Timer>();
        endBoostTimer.Duration = endBoostTime;
        endBoostTimer.AddTimerFinishedListener(() => {
            accerationFactor = .25f;
        });
    }

    // Update is called once per frame
    void Update()
    {
        onRoad = Physics2D.Raycast(transform.position, Vector2.down, 1, road);

        viewTarget.transform.position = transform.position;
        //viewTarget.transform.rotation = Quaternion.Lerp(viewTarget.transform.rotation, Quaternion.Euler(0,0,rb.rotation + 90f), cameraRotationLerpTime * Time.deltaTime);

        Vector2 input = Vector2.zero;
        input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Fire1")); 
        setVectors(input); //gathers input and sets appropriate input vectors 
        rotateTires();
        controlAnimSpeed();
        EnginePitch();
    }

    private void FixedUpdate()
    {
        if (GameManager.ins.canPlay)
        {
            applyEngineForce();
            killPerpVelocity();
            Steer();
        }
       // Debug.Log(rb.velocity);
       // Debug.Log("ForwardVelocity: " + rb.velocity.x + "   Forward Input: " + accelInput);
    }

    void applyEngineForce()
    {
        if (Input.GetAxisRaw("Fire1") > 0)
        {
            Vector2 engineForceVector = -transform.right * accelInput * accerationFactor;
            if (!onRoad)
                engineForceVector = engineForceVector / 2f;
            rb.AddForce(engineForceVector, ForceMode2D.Impulse);
            driftFactor = 1f;
            braking = false;
        }

        if(Input.GetAxisRaw("Fire1") <= 0 && !Input.GetMouseButton(1))
        {
            rb.velocity = Vector2.Lerp(rb.velocity, Vector2.zero, deaccel * Time.deltaTime);
            braking = true;
            driftFactor = .9f;
        }

        if (Input.GetMouseButton(1) && Input.GetAxisRaw("Fire1") <= 0)
        {
            Vector2 engineForceVector = transform.right * 1 * accerationFactor;
            engineForceVector = engineForceVector / 1.5f;
            if (!onRoad)
                engineForceVector = engineForceVector / 1.5f;
            rb.AddForce(engineForceVector, ForceMode2D.Impulse);
            driftFactor = 1f;
            braking = false;
        }
    }

    void Steer()
    {
        float minSpeedForSteering = (rb.velocity.magnitude / minSteerFactor);
        minSpeedForSteering = Mathf.Clamp01(minSpeedForSteering);
        if (!braking && Mathf.Abs(steeringInput) > 0.97f)
        {
            rotationAngle -= steeringInput * turnFactor * minSpeedForSteering;
            if (!driftAudio.isPlaying)
            {
                driftAudio.pitch = Random.Range(0.4f, 0.6f);
                driftAudio.Play();
            }
        }
        else if (!braking && Mathf.Abs(steeringInput) <= 0.97f) 
        {
            if (driftAudio.isPlaying)
                driftAudio.Stop();
            rotationAngle -= (steeringInput / 2) * turnFactor * minSpeedForSteering; 
        }
        else if (braking && Mathf.Abs(steeringInput) < 0.97f)
        {
            if (driftAudio.isPlaying)
                driftAudio.Stop();
            rotationAngle -= (steeringInput / 2f) * turnFactor * minSpeedForSteering;
        }
        else
        {
            rotationAngle -= (steeringInput / 1.25f) * turnFactor * minSpeedForSteering;
            if (!driftAudio.isPlaying)
            {
                driftAudio.pitch = Random.Range(0.4f, 0.6f);
                driftAudio.Play();
            }
        }
        if (braking && rb.velocity.magnitude < 2 && driftAudio.isPlaying)
        {
                driftAudio.Stop();
        }

        rb.MoveRotation(rotationAngle);
    }

    void setVectors(Vector2 inputVector)
    {
        steeringInput = inputVector.x;
        accelInput = inputVector.y;
    }

    void killPerpVelocity()
    {
        Vector2 forwardVel = -transform.right * Vector2.Dot(rb.velocity, -transform.right);
        Vector2 rightVel = transform.up * Vector2.Dot(rb.velocity, transform.up); //the way the sprites were made, right is up and up is right

        rb.velocity = forwardVel + rightVel * driftFactor;
    }

    void rotateTires()
    {
        if (Input.GetAxisRaw("Horizontal") != 0)
        {
            for (int i = 0; i < frontTires.Length; i++)
            {
                frontTires[i].transform.localRotation = Quaternion.Euler(0, 0, -steeringInput * 40);
            }
        }
        else
        {
            for (int i = 0; i < frontTires.Length; i++)
            {
                frontTires[i].transform.localRotation = Quaternion.Lerp(frontTires[i].transform.localRotation, Quaternion.Euler(0, 0, 0), 6f * Time.deltaTime);
            }
        }

    }

    void controlAnimSpeed()
    {
        anim.speed = accelInput;
        for (int i = 0; i < frontTires.Length; i++)
        {
            frontTires[i].GetComponent<Animator>().speed = accelInput;
        }
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "CheckPoint")
        {
            if (other.gameObject.GetComponent<CheckPoint>().checkPointNumber == (GameManager.ins.checkPointIndex + 1))
            {
                other.GetComponent<Collider2D>().enabled = false;
                other.GetComponent<SpriteRenderer>().enabled = false;
                Debug.Log("Hit Check Point" + GameManager.ins.checkPointIndex);
                GameManager.ins.checkPointIndex++;
                if (GameManager.ins.checkPointIndex == GameManager.ins.checkPointLength)
                {
                    Debug.Log("Lap Completed");
                    GameManager.ins.completedLap();
                }
            }
        }
        if (other.tag == "batOBJ" && other.gameObject.GetComponent<batteryCollectable>().isCollected == false)
        {
            other.gameObject.GetComponent<batteryCollectable>().isCollected = true;
            other.gameObject.GetComponent<Animator>().SetBool("Collected", true);
            GameManager.ins.batteryCount += 20;
            if (GameManager.ins.batteryCount > 100)
                GameManager.ins.batteryCount = 100;
            GameManager.ins.totalPickUps += 1;
            accerationFactor = .35f;
            if (endBoostTimer.Running)
                endBoostTimer.Stop(true);
            endBoostTimer.Run();

            Destroy(other.gameObject, 0.3f);
        }
    }

    public void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "CheckPoint")
        {
            if (other.gameObject.GetComponent<CheckPoint>().checkPointNumber != (GameManager.ins.checkPointLength) && other.GetComponent<Collider2D>().enabled == true)
            {
                Debug.Log("wrongCheckPoint");
                //show turn around sign
            }
        }

    }


    public void EnginePitch()
    {
        engineAudio.pitch = Mathf.Clamp01(rb.velocity.magnitude/12) * 1.75f;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, (Vector2)transform.position + Vector2.up * 1);
    }
}
