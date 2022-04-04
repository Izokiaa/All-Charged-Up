using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class trailRenderControler : MonoBehaviour
{
    [Header("Trail Renderer")]
    TrailRenderer skidTrailRend;
    // Start is called before the first frame update
    void Start()
    {
        skidTrailRend = GetComponent<TrailRenderer>();
        skidTrailRend.emitting = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Mathf.Abs(CarController2.ins.steeringInput) > 0.97f || CarController2.ins.braking)
        {
            skidTrailRend.emitting = true;
        }
        else
            skidTrailRend.emitting = false;
    }
}
