using UnityEngine;
using System.Collections;


public class AIPersonality : MonoBehaviour {


	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	// Evaluate the destination and return approval
	public int EvaluateUserInput(Vector3 _dest) {
		return 100;
	}

	// Evaluate the entire path and try to find a similar but more agreeable path or way
	// to achieve something like what the user wanted.
	public Vector3 MakeInputAgreeable(Vector3 _oldDest) {
		Vector3 newDest = _oldDest;	// Temporarily force AIPersonality to agree to oldHit
		return newDest;
	}
}
