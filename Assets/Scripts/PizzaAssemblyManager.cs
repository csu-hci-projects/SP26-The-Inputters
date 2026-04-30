using UnityEngine;

public class PizzaAssemblyManager : MonoBehaviour
{
    [SerializeField] private GameObject doughKetchupPrefab;
    [SerializeField] private Transform spawnPoint;

    [SerializeField] private GameObject mushroomEmptyPrefab;
    [SerializeField] private GameObject pepperoniEmptyPrefab;
    [SerializeField] private GameObject oliveEmptyPrefab;
    [SerializeField] private GameObject pepperEmptyPrefab;
    [SerializeField] private GameObject basilEmptyPrefab;

    private GameObject activePizza;

    public bool HasActivePizza => activePizza != null;

    public void PlacePizza()
    {
        if (activePizza != null)
        {
            Debug.Log("[Voice] Pizza already active.");
            return;
        }
        activePizza = Instantiate(doughKetchupPrefab, spawnPoint.position, spawnPoint.rotation);
        activePizza.GetComponent<ObjectManager>().justSpawned = false;
        Debug.Log($"[Voice] Pizza placed. SpawnPoint world pos: {spawnPoint.position}, Pizza world pos: {activePizza.transform.position}, Pizza scale: {activePizza.transform.localScale}");
    }

    public void AddTopping(GameObject emptyPrefab, string toppingName)
    {
        if (activePizza == null)
        {
            Debug.Log("[Voice] No active pizza to add topping to.");
            return;
        }

        ObjectManager pizzaOM = activePizza.GetComponent<ObjectManager>();

        // Disable the current active drop zone before the new topping brings its own
        foreach (BaseManager bm in activePizza.GetComponentsInChildren<BaseManager>())
            bm.gameObject.SetActive(false);

        GameObject topping = Instantiate(emptyPrefab);
        topping.transform.SetParent(activePizza.transform);

        // Mirrors BaseManager.OnTriggerStay: read from child 0's IngredientProperties
        IngredientProperties ip = topping.transform.GetChild(0).GetComponent<IngredientProperties>();
        ip.SetPrefabName();

        topping.transform.localPosition = new Vector3(
            ip.GetLocation().x,
            pizzaOM.topObjectLoc + ip.GetLocation().y,
            ip.GetLocation().z
        );
        topping.transform.localRotation = ip.GetRotation();
        topping.transform.localScale = ip.GetScale();

        ObjectManager toppingChildOM = topping.transform.GetChild(0).GetComponent<ObjectManager>();
        if (toppingChildOM != null)
            toppingChildOM.mode = 1;

        pizzaOM.numStackedIngredients++;
        pizzaOM.topObjectLoc += ip.GetLocation().y;
        pizzaOM.mode = (pizzaOM.mode == 1) ? 3 : 2;

        Debug.Log($"[Voice] Added {toppingName} to pizza.");
    }

    public void AddMushroom()  => AddTopping(mushroomEmptyPrefab,  "mushroom");
    public void AddPepperoni() => AddTopping(pepperoniEmptyPrefab, "pepperoni");
    public void AddOlive()     => AddTopping(oliveEmptyPrefab,     "olive");
    public void AddPepper()    => AddTopping(pepperEmptyPrefab,    "pepper");
    public void AddBasil()     => AddTopping(basilEmptyPrefab,     "basil");
}
