using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class ID : MonoBehaviour {


    public enum Specification
    {
        NULL, Spear, Stone, Flask, Tuna, Stick, WoodenAxe, RefinedStone, FireWood, FirePlace, Log, BurningStick, Berry, LongStick,
        Fish
            , Clay, ClayBowl, Mushroom, Rabbit, CampfireBase, WoodenTorch, BearMeat, WolfMeat, SmallLight, StoneKnife, WolfFurcoat, SharpWoodWall, ShortWoodWall
    };
    public Specification specification;
    public enum Type { NULL, Weapon, Consumable, Container, Building };
    public Type type;
    public int variation = 0;

    public enum SubMenu { NULL, ToolsWeapons, Building, Misc, LightWarmth };
    public SubMenu subMenu = SubMenu.NULL;

    public Collider[] Colliders;
    public Rigidbody[] Rigidbodies;

    public enum StaticTag { NULL, Tree, Water, Cabin, Boulder, BerryBush, Bush, MushroomPatch, RabbitHabitat, IceLake, BerryBushPoison, BearHabitat, WolfHabitat, CaveWolf, CaveBear, CLAUSE };
    public StaticTag _StaticTag;

    public enum Lifeform { NULL, Human, Bear, Rabbit, Fish, Wolf };
    public Lifeform _LifeForm;

    public bool isBase = true;

    public List<GameObject> Resources = new List<GameObject>();
    public List<ID> ResourceID = new List<ID>();

    public int slotWidth = 1;
    public int slotHeight = 1;

    public bool isRooted = false;

    public bool isFireSurface = false;
    public bool isFlamable = false;
    public bool canPlaceInGround = false;

    public int maxStack = 1;

    public Transform PrimaryObject = null;

    public GameObject Prefab = null;

    public bool Compare(Specification objSpec, Type objType, int objVar)
    {
        if (objSpec == Specification.NULL || objType == Type.NULL) return false;
        if (objSpec != specification) return false;
        if (objType != type) return false;
        if (objVar != variation) return false;

        return true;
    }

}
