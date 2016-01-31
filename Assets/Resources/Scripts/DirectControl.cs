using UnityEngine;
using System.Collections;

public class DirectControl : MonoBehaviour {

    public Rigidbody rb;

	void Start () {
        if ( rb == null )
            rb = GetComponent<Rigidbody>();
	}
	
    /*
	void FixedUpdate () {
        if (transform.position.z < 35) {
            rb.AddForce(new Vector3(0f, 0f, 750f)); // * Input.GetAxis("Vertical") );
                                                     // rb.AddForce(new Vector3(10000f, 0f, 0f) * Input.GetAxis("Horizontal"));
        }
    } */

}
