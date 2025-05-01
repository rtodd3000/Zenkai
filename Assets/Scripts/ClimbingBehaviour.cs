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

    void HandleClimbing()
    {
        float climbInputVertical = Input.GetAxis("Vertical");
        float climbInputHorizontal = Input.GetAxis("Horizontal");

        Vector3 frontRayOrigin = transform.position - transform.forward * 0.3f;
        RaycastHit frontHit;
        bool isTouchingClimbable = Physics.Raycast(frontRayOrigin, transform.forward, out frontHit, 1.0f, climbableLayer);
        Debug.DrawRay(frontRayOrigin, transform.forward * 1.0f, Color.cyan);

        if (!isTouchingClimbable)
        {
            if (Time.time - climbingStartTime > climbingGraceDuration)
            {
                Debug.Log("No wall in front. Stop climbing.");
                EndClimbing();
                return;
            }
        }

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
        Debug.Log("Climbing. New position: " + transform.position);
    }

    void EndClimbing()
    {
        isClimbing = false;
        animator.SetBool("IsClimbing", false);

        if (playerCollider != null)
            playerCollider.enabled = true;

        if (thirdPersonController != null)
            thirdPersonController.isClimbing = false;

        RaycastHit upwardHit;
        float upwardRayDistance = 2f;
        bool roofDetected = Physics.Raycast(transform.position, Vector3.up, out upwardHit, upwardRayDistance, climbableLayer);

        if (roofDetected && Vector3.Dot(upwardHit.normal, Vector3.up) > 0.9f)
        {
            Vector3 roofPoint = upwardHit.point;
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

        Vector3 rayOrigin = transform.position + Vector3.up * 2f;
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
            }
            else
            {
                animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0f);
                animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 0f);
                animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 0f);
                animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 0f);
            }
        }
    }
}
