﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup_FeatherFall : PassiveAbility {

    public float maxSpeed = 5f;
    private Rigidbody rb;

    public override void OnAbilityAdd()
    {
        rb = gameObject.GetComponent<Rigidbody>();
    }
    
    // Update is called once per frame
    public override void OnUpdate()
    {
        if (rb.velocity.y < -maxSpeed)  // Check that the y vel is less than neg maxSpeed
        {
            Vector3 ogVelocity = rb.velocity;
            
            Vector3 clamp = Vector3.ClampMagnitude(rb.velocity, maxSpeed);
            Debug.Log("Da bug Check");
            rb.velocity =  new Vector3(ogVelocity.x, clamp.y, ogVelocity.z);
        }
    }
}

