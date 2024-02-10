using UnityEngine;

public class CameraMovements : MonoBehaviour
{
    [Header("Horizontal Position")]
    [SerializeField] private float movementSpeed = 0.1f;
    [SerializeField] private float smoothing = 5;
    [SerializeField] private Vector3 center = Vector3.zero;
    [SerializeField] private Vector2 posRange = new Vector2(100, 100);
    private Vector3 targetPos;

    [Header("Rotation")]
    [SerializeField] private float rotationSpeed = 1;
    private float targetAngle;
    private float currentAngle;
    private Vector3 cameraDir;

    [Header("Zoom")]
    [SerializeField] private float zoomSpeed = 1;
    [SerializeField] private Vector2 zoomRange = new Vector2(1, 15);
    [SerializeField] Transform cameraToMove;
    private Vector3 zoomTarget;

    private PlayerInputActions playerInputActions;

    private void OnEnable()
    {
        playerInputActions = new PlayerInputActions();
        playerInputActions.GameplayInputs.Enable();
        targetPos = transform.position;

        targetAngle = transform.eulerAngles.y;
        currentAngle = targetAngle;

        zoomTarget = cameraToMove.localPosition;
    }

    private void Update()
    {
        Move();
        Rotate();
        Zoom();
    }

    private void Move()
    {
        Vector2 rawInput = playerInputActions.GameplayInputs.CameraMovements.ReadValue<Vector2>();
        Vector3 input = new Vector3(rawInput.x, 0, rawInput.y);
        Vector3 dir = (transform.worldToLocalMatrix.inverse * input).normalized;
        Vector3 movement = new Vector3(dir.x, 0, dir.z);
        Vector3 nextTargetPos = targetPos + movement * movementSpeed;

        if (PosInBound(nextTargetPos))
        targetPos = nextTargetPos;
        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * smoothing);
    }

    private void Rotate()
    {
        if (!Input.GetMouseButton(2)) return;
        targetAngle += Input.GetAxisRaw("Mouse X") * rotationSpeed;

        currentAngle = Mathf.Lerp(currentAngle, targetAngle, smoothing * Time.deltaTime);
        transform.rotation = Quaternion.Euler(transform.eulerAngles.x, currentAngle, transform.eulerAngles.z);
    }

    private void Zoom()
    {
        float input = Input.GetAxisRaw("Mouse ScrollWheel");
        Vector3 dir = (cameraToMove.worldToLocalMatrix.inverse * Vector3.forward).normalized;
        Vector3 nextZoomTarget = zoomTarget + dir * input * zoomSpeed;

        if (ZoomInBound(nextZoomTarget)) zoomTarget = nextZoomTarget;
        cameraToMove.localPosition = Vector3.Lerp(cameraToMove.localPosition, zoomTarget, Time.deltaTime * smoothing);
    }

    private bool PosInBound(Vector3 position)
    {
        return position.x > -posRange.x &&
            position.x < posRange.x &&
            position.z > -posRange.y &&
            position.z < posRange.y;
    }

    private bool ZoomInBound(Vector3 position)
    {
        return position.y > zoomRange.x && position.y < zoomRange.y;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(center, new Vector3(posRange.x, 5, posRange.y));
    }
}
