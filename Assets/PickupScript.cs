using UnityEngine;
using System.Collections;

public class PickupScript : MonoBehaviour {

    Camera cam;
    Collider tmpCollider;

    public Rigidbody rb;

    bool isCarrying = false;
    bool axisInUse = false;

    public float grabRange = 3f;

	// Use this for initialization
	void Start () {
        cam = GetComponentInChildren<Camera>();
	}
	
	// Update is called once per frame
	void LateUpdate () {

        if ( !axisInUse && Input.GetAxis("Use") != 0) {
            axisInUse = true;
            if (!isCarrying && tmpCollider != null)
                Pickup();
            else if (isCarrying)
                Drop();
        }

        else
            axisInUse = false;

        if ( !isCarrying ) {
            RaycastHit hit;
            if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, grabRange)) {
                if (hit.collider.CompareTag("Interactable")) {
                    if ( tmpCollider != null )
                        tmpCollider.gameObject.GetComponent<Renderer>().material.color = Color.white;
                    tmpCollider = hit.collider;
                    hit.collider.gameObject.GetComponent<Renderer>().material.color = Color.yellow;
                }
            }
            else if (tmpCollider != null) {
                tmpCollider.gameObject.GetComponent<Renderer>().material.color = Color.white;
                tmpCollider = null;
            }
        }
        
    }
    void Pickup () {
        isCarrying = true;
        tmpCollider.enabled = false;
        tmpCollider.gameObject.GetComponent<Renderer>().material.color = Color.red;
        tmpCollider.GetComponent<Rigidbody>().isKinematic = true;
        tmpCollider.gameObject.transform.SetParent(transform);
        tmpCollider.transform.localPosition = new Vector3(0f, .5f, 1.5f);
    }
    void Drop() {
        isCarrying = false;
        tmpCollider.enabled = true;
        tmpCollider.gameObject.GetComponent<Renderer>().material.color = Color.white;
        tmpCollider.GetComponent<Rigidbody>().isKinematic = false;
        tmpCollider.gameObject.transform.SetParent(null);
    }

}
