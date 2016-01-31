using UnityEngine;
using System.Collections;

public class CamFollow : MonoBehaviour {

    public GameObject follow;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        transform.position = follow.transform.position + new Vector3 ( 0, 6f, -3.6f );
	}

}
