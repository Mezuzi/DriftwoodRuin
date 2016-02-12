using UnityEngine;
using System.Collections;

public class DirectControl : MonoBehaviour {

    public Rigidbody rb;

	void Start () {
        if ( rb == null )
            rb = GetComponent<Rigidbody>();
	}
	
	void FixedUpdate () {
        if (Input.GetAxis("BoatForward") != 0)
            rb.AddRelativeForce ( Vector3.forward * 5000f * Input.GetAxis("BoatForward") );
        if (Input.GetAxis("BoatRotate") != 0)
            rb.AddForceAtPosition (Vector3.right * Input.GetAxis("BoatRotate") * 10f, rb.centerOfMass);
        
    }

}
