using UnityEngine;

public class BurgerAssemblyManager : MonoBehaviour
{
    [SerializeField] private GameObject bottomBunPrefab;
    [SerializeField] private Transform spawnPoint;

    [SerializeField] private GameObject pattyEmptyPrefab;
    [SerializeField] private GameObject cheeseEmptyPrefab;
    [SerializeField] private GameObject tomatoEmptyPrefab;
    [SerializeField] private GameObject onionEmptyPrefab;
    [SerializeField] private GameObject saladEmptyPrefab;
    [SerializeField] private GameObject baconEmptyPrefab;
    [SerializeField] private GameObject topBunEmptyPrefab;

    private GameObject activeBurger;

    public bool HasActiveBurger => activeBurger != null;

    public void PlaceBottomBun()
    {
        if (activeBurger != null)
        {
            Debug.Log("[Voice] Burger already active.");
            return;
        }
        activeBurger = Instantiate(bottomBunPrefab, spawnPoint.position, spawnPoint.rotation);
        activeBurger.GetComponent<ObjectManager>().justSpawned = false;
        Debug.Log("[Voice] Bottom bun placed.");
    }

    public void AddTopping(GameObject emptyPrefab, string toppingName)
    {
        if (activeBurger == null)
        {
            Debug.Log("[Voice] No active burger to add topping to.");
            return;
        }

        ObjectManager burgerOM = activeBurger.GetComponent<ObjectManager>();

        foreach (BaseManager bm in activeBurger.GetComponentsInChildren<BaseManager>())
            bm.gameObject.SetActive(false);

        GameObject topping = Instantiate(emptyPrefab);
        topping.transform.SetParent(activeBurger.transform);

        IngredientProperties ip = topping.transform.GetChild(0).GetComponent<IngredientProperties>();
        ip.SetPrefabName();

        topping.transform.localPosition = new Vector3(
            ip.GetLocation().x,
            burgerOM.topObjectLoc + ip.GetLocation().y,
            ip.GetLocation().z
        );
        topping.transform.localRotation = ip.GetRotation();
        topping.transform.localScale = ip.GetScale();

        ObjectManager toppingChildOM = topping.transform.GetChild(0).GetComponent<ObjectManager>();
        if (toppingChildOM != null)
            toppingChildOM.mode = 1;

        burgerOM.numStackedIngredients++;
        burgerOM.topObjectLoc += ip.GetLocation().y;
        burgerOM.mode = (burgerOM.mode == 1) ? 3 : 2;

        Debug.Log($"[Voice] Added {toppingName} to burger.");
    }

    public void AddPatty(string cookingStatus)
    {
        if (activeBurger == null)
        {
            Debug.Log("[Voice] No active burger to add patty to.");
            return;
        }

        ObjectManager burgerOM = activeBurger.GetComponent<ObjectManager>();

        foreach (BaseManager bm in activeBurger.GetComponentsInChildren<BaseManager>())
            bm.gameObject.SetActive(false);

        GameObject topping = Instantiate(pattyEmptyPrefab);
        topping.transform.SetParent(activeBurger.transform);

        IngredientProperties ip = topping.transform.GetChild(0).GetComponent<IngredientProperties>();
        ip.SetPrefabName();

        topping.transform.localPosition = new Vector3(
            ip.GetLocation().x,
            burgerOM.topObjectLoc + ip.GetLocation().y,
            ip.GetLocation().z
        );
        topping.transform.localRotation = ip.GetRotation();
        topping.transform.localScale = ip.GetScale();

        CutletEmptyManager cem = topping.transform.GetChild(0).GetComponent<CutletEmptyManager>();
        if (cem != null)
            cem.SetCookingStatus(cookingStatus);

        ObjectManager toppingChildOM = topping.transform.GetChild(0).GetComponent<ObjectManager>();
        if (toppingChildOM != null)
            toppingChildOM.mode = 1;

        burgerOM.numStackedIngredients++;
        burgerOM.topObjectLoc += ip.GetLocation().y;
        burgerOM.mode = (burgerOM.mode == 1) ? 3 : 2;

        Debug.Log($"[Voice] Added {cookingStatus} patty to burger.");
    }

    public void AddTopBun()
    {
        if (activeBurger == null)
        {
            Debug.Log("[Voice] No active burger to cap.");
            return;
        }

        ObjectManager burgerOM = activeBurger.GetComponent<ObjectManager>();

        foreach (BaseManager bm in activeBurger.GetComponentsInChildren<BaseManager>())
            bm.gameObject.SetActive(false);

        GameObject topping = Instantiate(topBunEmptyPrefab);
        topping.transform.SetParent(activeBurger.transform);

        IngredientProperties ip = topping.transform.GetChild(0).GetComponent<IngredientProperties>();
        ip.SetPrefabName();

        topping.transform.localPosition = new Vector3(
            ip.GetLocation().x,
            burgerOM.topObjectLoc + ip.GetLocation().y,
            ip.GetLocation().z
        );
        topping.transform.localRotation = ip.GetRotation();
        topping.transform.localScale = ip.GetScale();

        ObjectManager toppingChildOM = topping.transform.GetChild(0).GetComponent<ObjectManager>();
        if (toppingChildOM != null)
            toppingChildOM.mode = 1;

        burgerOM.numStackedIngredients++;
        burgerOM.topObjectLoc += ip.GetLocation().y;
        burgerOM.mode = 4;

        Debug.Log("[Voice] Top bun placed. Burger complete.");
    }

    public void AddCheese()  => AddTopping(cheeseEmptyPrefab,  "cheese");
    public void AddTomato()  => AddTopping(tomatoEmptyPrefab,  "tomato");
    public void AddOnion()   => AddTopping(onionEmptyPrefab,   "onion");
    public void AddSalad()   => AddTopping(saladEmptyPrefab,   "salad");
    public void AddBacon()   => AddTopping(baconEmptyPrefab,   "bacon");
}
