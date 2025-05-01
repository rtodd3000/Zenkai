using UnityEngine;
using StarterAssets;

public class ClimbingBehaviour : MonoBehaviour
{
    public Animator animator;
    public Transform leftHandTarget;
    public Transform rightHandTarget;
    public Transform leftFootTarget;
    public Transform rightFootTarget;

    public float climbSpeed = 100f;
    public float detectionDistance = 10.0f;
    public float distanceFromWall = 0.5f;
    public LayerMask climbableLayer;
    [SerializeField] private LayerMask groundLayer;

    public float landingRayDistance = 15f;
    public float landingOffset = 0.1f;
    
    // Add wider wall detection parameters
    public float wallCheckWidth = 0.5f;
    public int wallCheckRays = 3;

    private bool isClimbing = false;
    private Vector3 originalPosition;
    private Vector3 wallNormal;

    private float climbingStartTime;
    private float climbingGraceDuration = 0.2f;

    private Collider playerCollider;
    private ThirdPersonController thirdPersonController;

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

            if (Input.GetKeyDown(KeyCode.G))
            {
                Debug.Log("G key pressed: Ending climbing.");
                EndClimbing();
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                RaycastHit hit;
                Vector3 rayOrigin = leftHandTarget.position;
                Vector3 rayDirection = transform.forward;

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

    void StartClimbing(Vector3 hitPoint, Vector3 hitNormal)
    {
        isClimbing = true;
        climbingStartTime = Time.time;
        originalPosition = transform.position;
        wallNormal = hitNormal;

        if (playerCollider != null)
        {
            playerCollider.enabled = false;
        }

        if (thirdPersonController != null)
        {
            thirdPersonController.isClimbing = true;
        }

        Vector3 newPosition = hitPoint + hitNormal * distanceFromWall;
        newPosition.y = originalPosition.y;
        transform.position = newPosition;

        transform.rotation = Quaternion.LookRotation(-hitNormal);

        animator.SetBool("IsClimbing", true);
        Debug.Log("Started climbing. New starting position: " + transform.position);
    }

    bool IsWallInFront()
    {
        // Cast multiple rays to check for wall contact
        float halfWidth = wallCheckWidth / 2f;
        
        // Start with center ray
        Vector3 frontRayOrigin = transform.position - transform.forward * 0.3f;
        RaycastHit centerHit;
        bool centerHasContact = Physics.Raycast(frontRayOrigin, transform.forward, out centerHit, 1.0f, climbableLayer);
        Debug.DrawRay(frontRayOrigin, transform.forward * 1.0f, centerHasContact ? Color.green : Color.red);
        
        if (centerHasContact)
            return true;
            
        // Check additional rays to the sides
        for (int i = 1; i <= wallCheckRays/2; i++)
        {
            float offset = (halfWidth * i) / (wallCheckRays/2);
            
            // Left check
            Vector3 leftOrigin = frontRayOrigin + transform.right * -offset;
            bool leftContact = Physics.Raycast(leftOrigin, transform.forward, 1.0f, climbableLayer);
            Debug.DrawRay(leftOrigin, transform.forward * 1.0f, leftContact ? Color.green : Color.yellow);
            
            // Right check
            Vector3 rightOrigin = frontRayOrigin + transform.right * offset;
            bool rightContact = Physics.Raycast(rightOrigin, transform.forward, 1.0f, climbableLayer);
            Debug.DrawRay(rightOrigin, transform.forward * 1.0f, rightContact ? Color.green : Color.yellow);
            
            if (leftContact || rightContact)
                return true;
        }
        
        // Also check from hand positions for better detection
        bool leftHandContact = Physics.Raycast(leftHandTarget.position, transform.forward, 1.0f, climbableLayer);
        bool rightHandContact = Physics.Raycast(rightHandTarget.position, transform.forward, 1.0f, climbableLayer);
        
        Debug.DrawRay(leftHandTarget.position, transform.forward * 1.0f, leftHandContact ? Color.green : Color.yellow);
        Debug.DrawRay(rightHandTarget.position, transform.forward * 1.0f, rightHandContact ? Color.green : Color.yellow);
        
        return leftHandContact || rightHandContact;
    }

    void HandleClimbing()
    {
        float climbInputVertical = Input.GetAxis("Vertical");
        float climbInputHorizontal = Input.GetAxis("Horizontal");

        bool isTouchingClimbable = IsWallInFront();

        if (!isTouchingClimbable)
        {
            if (Time.time - climbingStartTime > climbingGraceDuration)
            {
                Debug.Log("No wall in front. Stop climbing.");
                EndClimbing();
                return;
            }
        }

        // Check for ceiling/obstacle above when climbing up
        if (climbInputVertical > 0)
        {
            Vector3 headPosition = transform.position + Vector3.up * 1.8f;
            RaycastHit ceilingHit;
            if (Physics.Raycast(headPosition, Vector3.up, out ceilingHit, 0.5f))
            {
                Debug.Log("Obstacle above detected, limiting upward movement");
                climbInputVertical = 0;
            }
        }

        // Check for ground below when climbing down
        RaycastHit downHit;
        bool isHittingGround = Physics.Raycast(transform.position, Vector3.down, out downHit, 1.0f, groundLayer);
        if (isHittingGround && climbInputVertical < 0)
        {
            Debug.Log("Cannot climb down into ground.");
            climbInputVertical = 0;
        }

        animator.SetFloat("ClimbDirection", climbInputVertical);
        animator.SetFloat("ClimbHorizontal", climbInputHorizontal);

        Vector3 upwardMovement = Vector3.up * climbInputVertical * climbSpeed * Time.deltaTime;
        Vector3 horizontalMovement = transform.right * climbInputHorizontal * climbSpeed * Time.deltaTime;
        Vector3 climbMovement = upwardMovement + horizontalMovement;

        transform.position += climbMovement;
        
        // After moving, check if we need to adjust distance from the wall
        if (isTouchingClimbable)
        {
            RaycastHit wallHit;
            if (Physics.Raycast(transform.position, transform.forward, out wallHit, 1.0f, climbableLayer))
            {
                // Adjust position to maintain proper distance from wall
                float currentDistance = Vector3.Distance(transform.position, wallHit.point);
                if (Mathf.Abs(currentDistance - distanceFromWall) > 0.1f)
                {
                    Vector3 adjustedPosition = wallHit.point + wallHit.normal * distanceFromWall;
                    adjustedPosition.y = transform.position.y; // Keep the same height
                    transform.position = Vector3.Lerp(transform.position, adjustedPosition, 0.5f);
                }
                
                // Update wall normal if it changed significantly
                if (Vector3.Angle(wallNormal, wallHit.normal) > 15f)
                {
                    wallNormal = wallHit.normal;
                    transform.rotation = Quaternion.Lerp(transform.rotation, 
                                                         Quaternion.LookRotation(-wallHit.normal),
                                                         0.5f);
                }
            }
        }
    }

    void EndClimbing()
    {
        isClimbing = false;
        animator.SetBool("IsClimbing", false);

        if (playerCollider != null)
            playerCollider.enabled = true;

        if (thirdPersonController != null)
            thirdPersonController.isClimbing = false;

        // Try to detect if we're at the top of a climbable surface
        RaycastHit upwardHit;
        float upwardRayDistance = 2f;
        bool roofDetected = Physics.Raycast(transform.position, Vector3.up, out upwardHit, upwardRayDistance, climbableLayer);

        if (roofDetected && Vector3.Dot(upwardHit.normal, Vector3.up) > 0.9f)
        {
            // Try multiple forward positions to find a suitable landing spot
            for (float forwardOffset = 0.5f; forwardOffset <= 2.0f; forwardOffset += 0.5f)
            {
                Vector3 topPosition = upwardHit.point + transform.forward * forwardOffset;
                Vector3 downwardOrigin = topPosition + Vector3.up * 1f;
                
                RaycastHit downwardHit;
                Debug.DrawRay(downwardOrigin, Vector3.down * (upwardRayDistance + 1f), Color.blue, 2f);
                
                if (Physics.Raycast(downwardOrigin, Vector3.down, out downwardHit, upwardRayDistance + 1f))
                {
                    Vector3 landingPos = downwardHit.point;
                    landingPos.y += (playerCollider != null ? playerCollider.bounds.extents.y : 0f) + landingOffset;
                    transform.position = landingPos;
                    Debug.Log("Landed on top surface at: " + landingPos);
                    return;
                }
            }
        }

        // Fallback detection for any ground below us
        Vector3 rayOrigin = transform.position + Vector3.up * 2f;
        RaycastHit fallbackHit;
        Debug.DrawRay(rayOrigin, Vector3.down * landingRayDistance, Color.red, 2f);
        if (Physics.Raycast(rayOrigin, Vector3.down, out fallbackHit, landingRayDistance))
        {
            Vector3 landingPos = fallbackHit.point;
            landingPos.y += (playerCollider != null ? playerCollider.bounds.extents.y : 0f) + landingOffset;
            transform.position = landingPos;
            Debug.Log("Landed on surface at: " + landingPos);
            return;
        }

        Debug.LogWarning("No landing surface detected; reverting to original position.");
        transform.position = originalPosition;
    }

    void OnAnimatorIK(int layerIndex)
    {
        if (animator)
        {
            if (isClimbing)
            {
                animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1f);
                animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1f);
                animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandTarget.position);
                animator.SetIKRotation(AvatarIKGoal.LeftHand, leftHandTarget.rotation);

                animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1f);
                animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1f);
                animator.SetIKPosition(AvatarIKGoal.RightHand, rightHandTarget.position);
                animator.SetIKRotation(AvatarIKGoal.RightHand, rightHandTarget.rotation);

                // You can add foot IK positioning here as well if needed
                // animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1f);
                // animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, 1f);
                // animator.SetIKPosition(AvatarIKGoal.LeftFoot, leftFootTarget.position);
                // animator.SetIKRotation(AvatarIKGoal.LeftFoot, leftFootTarget.rotation);
                
                // animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1f);
                // animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, 1f);
                // animator.SetIKPosition(AvatarIKGoal.RightFoot, rightFootTarget.position);
                // animator.SetIKRotation(AvatarIKGoal.RightFoot, rightFootTarget.rotation);
            }
            else
            {
                animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0f);
                animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 0f);
                animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 0f);
                animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 0f);
                // Reset foot IK if implemented
                // animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 0f);
                // animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, 0f);
                // animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 0f);
                // animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, 0f);
            }
        }
    }
}