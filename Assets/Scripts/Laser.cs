using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    // speed variable of 8
    [SerializeField]
    private float _Speed = 8.0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // translate laser up
        transform.Translate(new Vector3(0, 1, 0) * _Speed * Time.deltaTime);

        // if laser psoition is greater than 7 on y axis 
        // destroy the object
        if (transform.position.y > 7f)
        {
            Object.Destroy(this.gameObject);
        }
    }
}
