using UnityEngine;

public class MoveToClickPoint : MonoBehaviour {
	
	UnityEngine.AI.NavMeshAgent agent;
	public bool currentlySelected = false;
	public bool isDisabled = false;

	// GetComponent takes time, use it once.
	void Start () {
		agent = GetComponent<UnityEngine.AI.NavMeshAgent> ();
	}

	void Update () {
		if (Input.GetMouseButtonDown (1) && currentlySelected && !isDisabled) {
			RaycastHit hit;

			if (Physics.Raycast (Camera.main.ScreenPointToRay (Input.mousePosition), out hit, 100)) {
				agent.destination = hit.point;
			}
		}
	}

}