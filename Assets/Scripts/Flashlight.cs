using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

[RequireComponent(typeof(Light))]
public class Flashlight : MonoBehaviour  
{
    public float charge;
    Light flashlight;

    // Start is called before the first frame update
    void Start()
    {
        flashlight = GetComponent<Light>();
        flashlight.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (CrossPlatformInputManager.GetButtonDown("Fire1") && charge > 0)
            flashlight.enabled = !flashlight.enabled;

        if(flashlight.enabled)
        {
            charge -= Time.deltaTime;

            if (charge <= 0)
                flashlight.enabled = false;
        }        
    }
}
