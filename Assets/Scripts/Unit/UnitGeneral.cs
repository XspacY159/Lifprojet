using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitGeneral : MonoBehaviour
{
    public string unitName;
    public int unitHealth;
    public int unitMoveSpeed;
    public int unitAttackDamage;
    public int unitAttackRange;

    // Start is called before the first frame update
    void Start()
    {
        UnitSelectionController.Instance.unitsList.Add(this.gameObject);
    }

    void OnDestroy()
    {
        UnitSelectionController.Instance.unitsList.Remove(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
