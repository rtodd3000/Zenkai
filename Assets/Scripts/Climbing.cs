using UnityEngine;
using StarterAssets;  // Adjust if your ThirdPersonController is in a different namespace

public class ClimbingController : MonoBehaviour
{
    public Animator animator;                // Reference to the Animator component
    public Transform leftHandTarget;         // IK target for the left hand
    public Transform rightHandTarget;        // IK target for the right hand
    public Transform leftFootTarget;         // (Optional) IK target for the left foot
    public Transform rightFootTarget;        // (Optional) IK target for the right foot

    public float climbSpeed = 100f;           // Speed of climbing movement
    public float detectionDistance = 10.0f;     // Distance for raycasting to detect surfaces
    public LayerMask climbableLayer;         // The layer that defines climbable surfaces

    // Toggle for top detection (set false to always allow upward movement for testing)
    public bool useTopDetection = false;

    private bool isClimbing = false;         // Tracks if the player is currently climbing
    private Vector3 originalPosition;        // Stores the player's position before climbing

    private Collider playerCollider;         // Reference to the player's collider
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

            // Press E to exit climbing mode.
            if (Input.GetKeyDown(KeyCode.E))
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
                    StartClimbing(hit.point);
                }
                else
                {
                    Debug.Log("No climbable surface in range to start climbing.");
                }
            }
        }
    }

    // Called when the player starts climbing.
    void StartClimbing(Vector3 hitPoint)
    {
        isClimbing = true;
        originalPosition = transform.position;

        // Disable the player's collider to avoid collision interference.
        if (playerCollider != null)
        {
            playerCollider.enabled = false;
        }

        // Instead of disabling the whole ThirdPersonController,
        // set its climbing flag so that it stops movement updates
        // but continues its camera rotation.
        if (thirdPersonController != null)
        {
            thirdPersonController.isClimbing = true;
        }

        animator.SetBool("IsClimbing", true);
        Debug.Log("Started climbing.");
    }

    // Handles climbing movement each frame.
    // Handles climbing movement each frame.
    void HandleClimbing()
    {
        // Read vertical and horizontal input.
        float climbInputVertical = Input.GetAxis("Vertical");    // Positive for up, negative for down.
        float climbInputHorizontal = Input.GetAxis("Horizontal");  // Positive for right, negative for left.

        // Drive the blend tree for climbing animation using vertical input.
        animator.SetFloat("ClimbDirection", climbInputVertical);

        // Optionally, you can create a separate parameter for horizontal blending if needed.
        // e.g., animator.SetFloat("ClimbHorizontal", climbInputHorizontal);

        // If top detection is enabled, ensure upward motion stops if no surface above.
        if (useTopDetection)
        {
            Vector3 headPosition = transform.position + Vector3.up * 1.5f; // Adjust based on character's height.
            RaycastHit topHit;
            bool hasSurfaceAbove = Physics.Raycast(headPosition, Vector3.up, out topHit, 0.5f, climbableLayer);
            if (!hasSurfaceAbove && climbInputVertical > 0)
            {
                Debug.Log("Top of climb reached. Stopping upward movement.");
                climbInputVertical = 0;
            }
        }

        // Calculate movement:
        // Vertical (upwards) movement component.
        Vector3 upwardMovement = Vector3.up * climbInputVertical * climbSpeed * Time.deltaTime;
        // Horizontal (lateral) movement component using the player's right vector.
        Vector3 horizontalMovement = transform.right * climbInputHorizontal * climbSpeed * Time.deltaTime;
        
        // Combine both movement directions.
        Vector3 climbMovement = upwardMovement + horizontalMovement;

        // Apply the movement to the player's position.
        transform.position += climbMovement;
        Debug.Log("New position: " + transform.position);
    }


    // Ends the climbing state and repositions the character.
    void EndClimbing()
    {
        isClimbing = false;
        animator.SetBool("IsClimbing", false);

        // Re-enable the player's collider.
        if (playerCollider != null)
        {
            playerCollider.enabled = true;
        }

        // Tell the ThirdPersonController that climbing has ended.
        if (thirdPersonController != null)
        {
            thirdPersonController.isClimbing = false;
        }

        // Cast a ray downward to reposition the character.
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 5f, climbableLayer))
        {
            Vector3 landingPos = hit.point;
            landingPos.y += 0.1f;  // Slight upward offset to prevent clipping.
            transform.position = landingPos;
            Debug.Log("Landed on top of surface at: " + landingPos);
        }
        else
        {
            Debug.LogWarning("No top surface detected, reverting to original position.");
            transform.position = originalPosition;
        }
    }

    // OnAnimatorIK is called after the Animator completes its evaluation.
    // This sets the IK positions and rotations for the hands while climbing.
    void OnAnimatorIK(int layerIndex)
    {
        if (animator)
        {
            if (isClimbing)
            {
                // Left Hand IK
                animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1f);
                animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1f);
                animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandTarget.position);
                animator.SetIKRotation(AvatarIKGoal.LeftHand, leftHandTarget.rotation);

                // Right Hand IK
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
