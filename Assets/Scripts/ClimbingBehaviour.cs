using UnityEngine;
using StarterAssets;  // Adjust if your ThirdPersonController is in a different namespace

public class ClimbingBehaviour : MonoBehaviour
{
    public Animator animator;                // Reference to the Animator component
    public Transform leftHandTarget;         // IK target for the left hand
    public Transform rightHandTarget;        // IK target for the right hand
    public Transform leftFootTarget;         // (Optional) IK target for the left foot
    public Transform rightFootTarget;        // (Optional) IK target for the right foot

    public float climbSpeed = 100f;           // Speed of climbing movement
    public float detectionDistance = 10.0f;   // Distance for raycasting to detect surfaces
    public float distanceFromWall = 0.5f;      // How far from the wall to position the player when climbing
    public LayerMask climbableLayer;          // The layer that defines climbable surfaces

    // Toggle for top detection (set false to always allow upward movement for testing)
    public bool useTopDetection = false;
    
    // New variables to adjust landing behavior.
    public float landingRayDistance = 15f;    // How far down to look for a landing spot
    public float landingOffset = 0.1f;        // Extra offset to adjust landing height if necessary

    private bool isClimbing = false;          // Tracks if the player is currently climbing
    private Vector3 originalPosition;         // Stores the player's position before climbing
    private Vector3 wallNormal;               // Stores the normal of the wall surface

    private Collider playerCollider;          // Reference to the player's collider
    private ThirdPersonController thirdPersonController;  // Reference to the movement controller

    void Start()
    {
        playerCollider = GetComponent<Collider>();
        if (playerCollider == null)
        {
            Debug.LogWarning("No collider found on the player!");
        }

        thirdPersonController = GetComponent<ThirdPersonController>();
        if (thirdPersonController == null)
        {
            Debug.LogWarning("No ThirdPersonController found on the player!");
        }
    }

    void Update()
    {
        if (isClimbing)
        {
            HandleClimbing();

            // Press E to exit climbing mode.x
            if (Input.GetKeyDown(KeyCode.G))
            {
                Debug.Log("E key pressed: Ending climbing.");
                EndClimbing();
            }
        }
        else
        {
            // Press F to attempt to start climbing.
            if (Input.GetKeyDown(KeyCode.F))
            {
                // Raycast from the left-hand target position.
                RaycastHit hit;
                Vector3 rayOrigin = leftHandTarget.position;
                Vector3 rayDirection = transform.forward;  // Adjust if needed

                Debug.DrawRay(rayOrigin, rayDirection * detectionDistance, Color.green);

                if (Physics.Raycast(rayOrigin, rayDirection, out hit, detectionDistance, climbableLayer))
                {
                    Debug.Log("Climbable surface detected at: " + hit.point + ". Starting climbing.");
                    StartClimbing(hit.point, hit.normal);
                }
                else
                {
                    Debug.Log("No climbable surface in range to start climbing.");
                }
            }
        }
    }

    // Called when the player starts climbing.
    // This version repositions the player using the raycast hit but preserves the original Y value.
    void StartClimbing(Vector3 hitPoint, Vector3 hitNormal)
    {
        isClimbing = true;
        originalPosition = transform.position;
        wallNormal = hitNormal;

        // Disable the player's collider to avoid collision interference.
        if (playerCollider != null)
        {
            playerCollider.enabled = false;
        }

        // Set climbing flag in the ThirdPersonController (if applicable).
        if (thirdPersonController != null)
        {
            thirdPersonController.isClimbing = true;
        }

        // Compute a new position:
        // Position the player at the hit point plus an offset along the wall normal.
        // Preserve the original Y value so that the climb starts at ground level.
        Vector3 newPosition = hitPoint + hitNormal * distanceFromWall;
        newPosition.y = originalPosition.y;
        transform.position = newPosition;

        // Rotate the player to face the wall.
        transform.rotation = Quaternion.LookRotation(-hitNormal);

        // Set the climbing animation state.
        animator.SetBool("IsClimbing", true);
        Debug.Log("Started climbing. New starting position: " + transform.position);
    }

    // Handles climbing movement each frame.
    void HandleClimbing()
    {
        // Get climbing input.
        float climbInputVertical = Input.GetAxis("Vertical");    // Positive for up, negative for down.
        float climbInputHorizontal = Input.GetAxis("Horizontal");  // Positive for right, negative for left.

        // Update animation parameters.
        animator.SetFloat("ClimbDirection", climbInputVertical);

        // Optional top detection logic.
        if (useTopDetection)
        {
            Vector3 headPosition = transform.position + Vector3.up * 1.5f;
            RaycastHit topHit;
            bool hasSurfaceAbove = Physics.Raycast(headPosition, Vector3.up, out topHit, 0.5f, climbableLayer);
            if (!hasSurfaceAbove && climbInputVertical > 0)
            {
                Debug.Log("Top of climb reached. Stopping upward movement.");
                climbInputVertical = 0;
            }
        }

        // Calculate movement vectors.
        Vector3 upwardMovement = Vector3.up * climbInputVertical * climbSpeed * Time.deltaTime;
        Vector3 horizontalMovement = transform.right * climbInputHorizontal * climbSpeed * Time.deltaTime;
        Vector3 climbMovement = upwardMovement + horizontalMovement;

        // Apply the climbing movement.
        transform.position += climbMovement;
        Debug.Log("Climbing. New position: " + transform.position);
    }

    // Revised EndClimbing method to better detect rooftop landing.
    void EndClimbing()
    {
        isClimbing = false;
        animator.SetBool("IsClimbing", false);

        // Re-enable the player's collider before landing.
        if (playerCollider != null)
            playerCollider.enabled = true;

        if (thirdPersonController != null)
            thirdPersonController.isClimbing = false;

        // --- Attempt A: Look upward for a nearly horizontal roof ---
        RaycastHit upwardHit;
        float upwardRayDistance = 10f; // Adjust as needed
        bool roofDetected = Physics.Raycast(transform.position, Vector3.up, out upwardHit, upwardRayDistance, climbableLayer);

        if (roofDetected && Vector3.Dot(upwardHit.normal, Vector3.up) > 0.9f)
        {
            // Found a nearly flat roof.
            Vector3 roofPoint = upwardHit.point;
            // Now cast a downward ray from a little above the detected roof.
            Vector3 downwardOrigin = roofPoint + Vector3.up * 1f;
            RaycastHit downwardHit;
            if (Physics.Raycast(downwardOrigin, Vector3.down, out downwardHit, upwardRayDistance + 1f, climbableLayer))
            {
                Vector3 landingPos = downwardHit.point;
                landingPos.y += (playerCollider != null ? playerCollider.bounds.extents.y : 0f) + landingOffset;
                transform.position = landingPos;
                Debug.Log("Landed on roof at: " + landingPos);
                return;
            }
        }

        // --- Attempt B: Fallback downward raycast from above the player ---
        Vector3 rayOrigin = transform.position + Vector3.up * 2f; // Start higher than current position.
        RaycastHit fallbackHit;
        Debug.DrawRay(rayOrigin, Vector3.down * landingRayDistance, Color.red, 2f);
        if (Physics.Raycast(rayOrigin, Vector3.down, out fallbackHit, landingRayDistance, climbableLayer))
        {
            Vector3 landingPos = fallbackHit.point;
            landingPos.y += (playerCollider != null ? playerCollider.bounds.extents.y : 0f) + landingOffset;
            transform.position = landingPos;
            Debug.Log("Landed on surface at: " + landingPos);
            return;
        }
        
        Debug.LogWarning("No top surface detected; reverting to original position.");
        transform.position = originalPosition;
    }

    // Called after the Animator finishes its evaluation; applies IK to the hands.
    void OnAnimatorIK(int layerIndex)
    {
        if (animator)
        {
            if (isClimbing)
            {
                // Left Hand IK setup.
                animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1f);
                animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1f);
                animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandTarget.position);
                animator.SetIKRotation(AvatarIKGoal.LeftHand, leftHandTarget.rotation);

                // Right Hand IK setup.
                animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1f);
                animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1f);
                animator.SetIKPosition(AvatarIKGoal.RightHand, rightHandTarget.position);
                animator.SetIKRotation(AvatarIKGoal.RightHand, rightHandTarget.rotation);
            }
            else
            {
                // Reset IK weights when not climbing.
                animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0f);
                animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 0f);
                animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 0f);
                animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 0f);
            }
        }
    }
}
