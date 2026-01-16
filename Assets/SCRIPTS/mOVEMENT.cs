using UnityEngine;

public class Movement : MonoBehaviour
{
    // Update is called once per frame
    public float speedFloat;

    public void Update()
    {
        //moveForward
        if (Input.GetKey(KeyCode.W))
        {
            transform.Translate(transform.forward * speedFloat * Time.deltaTime);
        }
        //move backward
        if (Input.GetKey(KeyCode.S))
        {
            //note we invert the transform.forward
            transform.Translate(-transform.forward * speedFloat * Time.deltaTime);
        }
    }
}