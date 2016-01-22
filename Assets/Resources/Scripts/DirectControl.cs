using UnityEngine;
using System.Collections;

public class DirectControl : MonoBehaviour {

    Rigidbody rb;
	void Start () {
        rb = GetComponent<Rigidbody>();
	}
	
	void FixedUpdate () {
        if (transform.position.z < 13) {
            rb.AddForce(new Vector3(0f, 0f, 150f)); // * Input.GetAxis("Vertical") );
                                                     // rb.AddForce(new Vector3(10000f, 0f, 0f) * Input.GetAxis("Horizontal"));
        }
    }

}
