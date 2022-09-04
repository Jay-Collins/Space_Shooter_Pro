using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Player : MonoBehaviour 
{
    [SerializeField] private float _speed = 3.0f;    

    // Start is called before the first frame update
    void Start()
    {
        //take the current position = new position (0,0,0)
        transform.position = new Vector3(0, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {   
        //new Vector3(1, 0, 0) * 5 * real time
        transform.Translate(new Vector3(1, 0, 0) * _speed * Time.deltaTime);
    }
}
