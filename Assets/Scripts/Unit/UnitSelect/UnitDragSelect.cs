using UnityEngine;

public class UnitDragSelect : MonoBehaviour
{
    private Camera myCam;

    [SerializeField] private RectTransform visualBox;    //used to display the selection box

    private Rect selectionBox;          //used for the logic of the box

    private Vector2 startPosition;
    private Vector2 endPosition;

    // Start is called before the first frame update
    void Start()
    {
        myCam = Camera.main;
        startPosition = Vector2.zero;
        endPosition = Vector2.zero;
        DrawVisual();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))    //players clicks
        {
            startPosition = Input.mousePosition;    //where the box starts
            selectionBox = new Rect();
        }
        if (Input.GetMouseButton(0))        //players holds clicks and drags
        {
            endPosition = Input.mousePosition;      //where the box currently ends
            DrawVisual();
            DrawSelection();
        }
        if (Input.GetMouseButtonUp(0))    //players realeases the click
        {
            SelectUnits();
            startPosition = Vector2.zero;
            endPosition = Vector2.zero;
            DrawVisual();
        }
    }
    
    //draws the selection box to the screen
    private void DrawVisual()
    {
        Vector2 boxStart = startPosition;
        Vector2 boxEnd = endPosition;

        Vector2 boxCenter = (boxStart + boxEnd) / 2;    //compute the center of the box to be displayed
        visualBox.position = boxCenter;

        Vector2 boxSize = new Vector2(Mathf.Abs(boxStart.x - boxEnd.x), Mathf.Abs(boxStart.y - boxEnd.y));  //computes the size of the box
        visualBox.sizeDelta = boxSize;
    }

    //computes the logic of the selection box
    private void DrawSelection()
    {
        //we need to know in which direction the box is dragged to correctly compute
        if(Input.mousePosition.x < startPosition.x) //dragging left
        {
            selectionBox.xMin = Input.mousePosition.x;
            selectionBox.xMax = startPosition.x;
        }
        else                                        //dragging right
        {
            selectionBox.xMin = startPosition.x;
            selectionBox.xMax = Input.mousePosition.x;
        }
        if (Input.mousePosition.y < startPosition.y) //dragging down
        {
            selectionBox.yMin = Input.mousePosition.y;
            selectionBox.yMax = startPosition.y;
        }
        else                                        //dragging up
        {
            selectionBox.yMin = startPosition.y;
            selectionBox.yMax = Input.mousePosition.y;
        }
    }

    private void SelectUnits()
    {
        foreach (var unit in UnitSelectionController.Instance.unitsList.Values) //we go through all the units that can be selected
        {
            if (selectionBox.Contains(myCam.WorldToScreenPoint(unit.transform.position))) //the position of the unit is project to the camera, and checked if in the box
            {
                UnitSelectionController.Instance.DragSelect(unit.gameObject);
            }
        }
    }
}
