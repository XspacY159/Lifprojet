using UnityEngine;

public class PlayerMvmt : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float speed;
    private float currentSpeedCoeff;
    [SerializeField] private POI testPOI;
    private void Update()
    {
        Vector2 currentPos = new Vector2(transform.position.x, transform.position.z);
        Tile currentTile = TerrainManager.Instance.GetTile(currentPos);

        if(Input.GetKeyDown(KeyCode.E))
        {
            testPOI.Interact(transform);
        }

        if (currentTile == null) return;
        currentSpeedCoeff = currentTile.GetTileType().movingSpeedCoeff;
        //Debug.Log(currentTile.GetTileType().tileName);
    }
    private void FixedUpdate()
    {
        Vector3 velocity = Vector3.zero;
        if (Input.GetKey(KeyCode.W))
        {
            velocity += Time.fixedDeltaTime * speed * currentSpeedCoeff * Vector3.forward;
        }
        if (Input.GetKey(KeyCode.A))
        {
            velocity += Time.fixedDeltaTime * speed * currentSpeedCoeff * Vector3.left;
        }
        if (Input.GetKey(KeyCode.S))
        {
            velocity += Time.fixedDeltaTime * speed * currentSpeedCoeff * Vector3.back;
        }
        if (Input.GetKey(KeyCode.D))
        {
            velocity += Time.fixedDeltaTime * speed * currentSpeedCoeff * Vector3.right;
        }
        rb.velocity = velocity;
    }
}
