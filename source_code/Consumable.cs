using UnityEngine;
using System.Collections;

public class Consumable : MonoBehaviour {

    public float hydration = 0f;

    public float remaining = 0f;
    public float stomach = 0f;
    public float damage = 0f;

    public bool canDrink = false;
    public bool canEat = false;

    public bool canRefill = false;
    public bool isPure = false;
    public bool needsCooking = false;
    public int timeToCook = 0;

    public bool requiresHeatingToHarden = false;

    [SerializeField]
    private Material _CookedMat;
    [SerializeField]
    private MeshRenderer[] _CookMeshes;

    // HAVE DRINK PERCENTAGE FOR GETKEY? would be cool, to only drink half

    void Awake()
    {
       // remaining = hydration;
    }

    public void Cook()
    {
        needsCooking = false;

            damage = 0;

        if(_CookedMat != null)
        {
            for(int i = 0; i < _CookMeshes.Length; i++)
            {
                _CookMeshes[i].material = _CookedMat;
            }

        }

        
    }

    public void Drink()
    {
        if (remaining == 0f) return;
        if (!isPure || (canEat && needsCooking)) return;

        remaining -= hydration;
        UIManager.Instance.AdjustBar(hydration, UIManager.Bars.Hydration);
        if(!canRefill) Camera.main.GetComponent<Inventory>().RemoveItem(this.gameObject, 1);
    }

	public void Eat()
    {
        if ((needsCooking && damage == 0) || (canDrink && !isPure)) return;

       

        UIManager.Instance.AdjustBar(stomach, UIManager.Bars.Food);

        if (damage > 0) Damage(damage);
        Camera.main.GetComponent<Inventory>().RemoveItem(this.gameObject, 1);

    }

    public void Damage(float dmg)
    {
        UIManager.Instance.AdjustBar(-dmg, UIManager.Bars.Health);
    }
	
	// Update is called once per frame
	void Update () {
	
        if(Input.GetKeyDown(KeyCode.F))
        {
            if(canDrink)
             Drink();

            if (canEat)
                Eat();
        }
	}
}
