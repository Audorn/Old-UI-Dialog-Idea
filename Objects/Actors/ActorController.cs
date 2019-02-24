using UnityEngine;
using System.Collections;

/// <summary>
/// Actor Controller handles input from the user in regards to movement or action commands
/// given to a party member.  It will ask AIPersonality to evaluate the orders and provide
/// aggreable ones as close as possible to the original.  If there is no user input for
/// this party member, AIPersonality will be asked to provide input.  Once approved commands
/// have been prepared they will be passed to ActorMesh for processing.
/// </summary>

[RequireComponent(typeof (AIPersonality))]
[RequireComponent(typeof (ActorMesh))]
public class ActorController : MonoBehaviour {

	private AIPersonality personality;
	private ActorMesh mesh;

	public bool isDisabled = false;

	private void Start() {
		personality = GetComponent<AIPersonality> ();
		mesh = GetComponent<ActorMesh> ();
	}

	// For input and display purposes
	private void Update() {

		// ------------------------------------------------------------------------------
		// TODO: Determine whether actor already has instructions from user and if they
		//       do, evaluate them after each step in the instructions.  If they do not,
		//       ask AIPersonality to provide instructions.
		// ------------------------------------------------------------------------------

	}

	public void Move (Vector3 _dest) {

		// Early out
		if (isDisabled)
			return;

		// ------------------------------------------------------------------------------
		// TODO: Implement pause and allow for different reactions when RT or paused.
		// TODO: Implement actions and a dynamic actions based on the target.
		// TODO: Implement laying textures on ground to signify commands in advance and post.
		// -----------------------------------------------------------------------------

		// -----------------------------------------------------------------------------
		// TODO: Figure out how to send a set of instructions to AIPersonality (Array?).
		// -----------------------------------------------------------------------------

		// Check AIPersonality for approval
		int AIApproval = personality.EvaluateUserInput (_dest);
		if (AIApproval <= 50) {

			// -------------------------------------------------------------------------
			// TODO: Come up with a better way to determine aggreability from AIPersonality.
			// -------------------------------------------------------------------------

			_dest = personality.MakeInputAgreeable (_dest);
		}

		// -----------------------------------------------------------------------------
		// TODO: Learn about the way Unity handles user input vs. NavMeshAgent so that
		// 		 I can combine the two better instead of just using NMA for both.
		// mesh.ProcessPath (destination);
		// -----------------------------------------------------------------------------

		mesh.Move (_dest);
	}
}
