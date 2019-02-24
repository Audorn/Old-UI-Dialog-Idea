using UnityEngine;
using System.Collections;

/// <summary>
/// Actor mesh controls the behaviour of the mesh in the game world and coordinates the navmesh with the animator.
/// </summary>

[RequireComponent(typeof (Rigidbody))]
[RequireComponent(typeof (CapsuleCollider))]
[RequireComponent(typeof (Animator))]
[RequireComponent(typeof (UnityEngine.AI.NavMeshAgent))]
public class ActorMesh : MonoBehaviour {

	UnityEngine.AI.NavMeshAgent agent;
	Animator animator;

	void Start() {
		agent = GetComponent<UnityEngine.AI.NavMeshAgent> ();
		animator = GetComponent<Animator> ();
	}

	public void Move (Vector3 _dest) {
		agent.SetDestination (_dest);
	}
}
