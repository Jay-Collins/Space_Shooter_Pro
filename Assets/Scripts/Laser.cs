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

        // if laser position is greater than 7 on y axis 
        // destroy the object
        if (transform.position.y > 7f)
        {
            // check if this object has a parent // if it does, destroy the parent
            if (transform.parent != null)
            {
                Object.Destroy(transform.parent.gameObject);
            }
            Object.Destroy(this.gameObject);
        }
    }
}
