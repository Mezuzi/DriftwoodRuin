using UnityEngine;
using System.Collections;

public class PickupScript : MonoBehaviour {

    Camera cam;
    Collider tmpCollider;

    MeshCollider water;
    public Rigidbody rb;

    bool isCarrying = false;
    bool axisInUse = false;

    public float grabRange = 3f;

	// Use this for initialization
	void Start () {
        cam = GetComponentInChildren<Camera>();
        water = GameObject.Find("Water").GetComponent<MeshCollider>();
	}
	
	// Update is called once per frame
	void Update () {

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

        Row();

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

    void Row() {
        if (Input.GetAxis("Row") != 0) {
            RaycastHit hit;
            if (water.Raycast(new Ray(cam.transform.position, cam.transform.forward), out hit, 10f)) {
                Vector3 F = cam.transform.forward * -3000f;
                F = new Vector3(F.x, 0f, F.z);
                rb.AddForceAtPosition( F, hit.point);
            }
        }
    }

}
