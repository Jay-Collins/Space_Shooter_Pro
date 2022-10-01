using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    // speed variable of 8
    [SerializeField] private float _speed = 8.0f;
    
    // Update is called once per frame
    void Update()
    { 
        MoveUp();
    }

    void MoveUp()
    {
        // translate laser up
        transform.Translate(new Vector3(0, 1, 0) * _speed * Time.deltaTime);

        // if laser position is greater than 7 on y axis // destroy the object
        if (transform.position.y < 9f) return;
        // null check and destroy parent.
        if (transform.parent) Destroy(transform.parent.gameObject);
        Destroy(gameObject);
    }
}
