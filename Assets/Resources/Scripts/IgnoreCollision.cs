using UnityEngine;
using System.Collections;

public class IgnoreCollision : MonoBehaviour {

	// Use this for initialization
	void Awake () {
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb == null)
            Debug.Log("[IgnoreCollision @ " + gameObject.name + "] This object does't have a RigidBody!");
	}

}
