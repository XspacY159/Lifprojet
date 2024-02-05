using UnityEngine;

public class UnitClickSelect : MonoBehaviour
{
    private Camera myCam;

    [SerializeField] private LayerMask clickable;
    [SerializeField] private LayerMask ground;

    // Start is called before the first frame update
    void Start()
    {
        myCam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = myCam.ScreenPointToRay(Input.mousePosition);
                //traces a ray from the camera to the scene, according to the position of the mouse cursor

            if(Physics.Raycast(ray, out hit, Mathf.Infinity, clickable))    //the ray hits a clickable objet
            {
                if (Input.GetKey(KeyCode.LeftShift))    //if the player holds shift while clicking, he selects multiple units
                {
                    UnitSelectionController.Instance.ShiftSelect(hit.collider.gameObject);
                }
                else
                {
                    UnitSelectionController.Instance.ClickSelect(hit.collider.gameObject);
                }
            }
            else                                                           //the ray doesn't hit
            {
                if (!Input.GetKey(KeyCode.LeftShift))   //if the player holds shift, he might not want to deselect all, so we don't
                {
                    UnitSelectionController.Instance.DeselectAll();
                }
            }
        }
    }
}
