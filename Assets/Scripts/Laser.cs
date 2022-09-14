using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    // speed variable of 8
    [SerializeField]
    private float _Speed = 8.0f;

    // Update is called once per frame
    void Update()
    {
        // translate laser up
        transform.Translate(new Vector3(0, 1, 0) * _Speed * Time.deltaTime);

        // if laser position is greater than 7 on y axis // destroy the object
        if (transform.position.y < 9f) return;
        if (transform.parent) Destroy(transform.parent.gameObject);
        Destroy(gameObject);
    }
}
