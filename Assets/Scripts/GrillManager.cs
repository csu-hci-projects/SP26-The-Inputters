using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GrillManager : MonoBehaviour
{
    [SerializeField] float cookingSpeed;
    [SerializeField] GameObject grillPatty;

    List<string> cutletNames = new List<string>();
    int prevQuotient;


    void Start()
    {
        if (grillPatty != null)
            grillPatty.SetActive(false);
    }

    public void PlacePattyOnGrill()
    {
        if (grillPatty == null)
        {
            Debug.LogWarning("[Voice] Grill patty not assigned.");
            return;
        }
        if (grillPatty.activeSelf)
        {
            Debug.Log("[Voice] Patty already on grill.");
            return;
        }
        grillPatty.SetActive(true);
        Debug.Log("[Voice] Patty placed on grill.");
    }

    public bool TransferPattyToBurger(BurgerAssemblyManager burger)
    {
        if (grillPatty == null || !grillPatty.activeSelf)
        {
            Debug.Log("[Voice] No patty on grill to transfer.");
            return false;
        }
        IngredientProperties ip = grillPatty.GetComponent<IngredientProperties>();
        CutletManager cm = grillPatty.GetComponent<CutletManager>();
        if (ip == null || cm == null)
        {
            Debug.LogWarning("[Voice] Grill Patty is missing IngredientProperties or CutletManager — assign Cutlet B (not Cutlet B Empty).");
            return false;
        }
        string cookingStatus = ip.GetCookingStatus();
        cutletNames.Remove(grillPatty.name);
        cm.SetCookingState(false);
        grillPatty.SetActive(false);
        burger.AddPatty(cookingStatus);
        Debug.Log($"[Voice] Transferred {cookingStatus} patty to burger.");
        return true;
    }

    public void StartCooking()
    {
        if (grillPatty != null && grillPatty.activeSelf)
        {
            CutletManager cm = grillPatty.GetComponent<CutletManager>();
            if (cm != null) cm.SetCookingState(true);
        }
        foreach (string cutletName in cutletNames)
        {
            GameObject cutlet = GameObject.Find(cutletName);
            if (cutlet != null)
                cutlet.GetComponent<CutletManager>().SetCookingState(true);
        }
        Debug.Log($"[Voice] StartCooking called on {cutletNames.Count} cutlet(s) + voice patty.");
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Ingredient")
        {
            if (other.GetComponent<IngredientProperties>().GetPrefabName() == "Cutlet B" && !other.gameObject.GetComponent<ObjectManager>().isGrabbed)
            {
                if (!cutletNames.Contains(other.name))
                    cutletNames.Add(other.name);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Ingredient")
        {
            if (other.GetComponent<IngredientProperties>().GetPrefabName() == "Cutlet B")
            {
                if (cutletNames.Contains(other.name))
                {
                    cutletNames.Remove(other.name);
                    other.GetComponent<CutletManager>().SetCookingState(false);
                }
            }
        }
    }
}
