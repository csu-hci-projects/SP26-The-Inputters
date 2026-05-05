using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class GrillManager : MonoBehaviour
{
    [SerializeField] float cookingSpeed;
    [SerializeField] GameObject grillPatty;
    [SerializeField] private TextMeshPro grillStatusText;

    List<string> cutletNames = new List<string>();
    int prevQuotient;
    bool grillOn = false;

    void Start()
    {
        if (grillPatty != null)
            grillPatty.SetActive(false);
        if (grillStatusText != null)
            grillStatusText.gameObject.SetActive(false);
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

        if (grillOn)
        {
            CutletManager cm = grillPatty.GetComponent<CutletManager>();
            if (cm != null) cm.SetCookingState(true);
        }

        Debug.Log($"[Voice] Patty placed on grill. GrillOn:{grillOn}");
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

        foreach (string cutletName in cutletNames)
        {
            GameObject cutlet = GameObject.Find(cutletName);
            if (cutlet != null)
                cutlet.GetComponent<CutletManager>().SetCookingState(false);
        }
        cutletNames.Clear();

        grillOn = false;
        if (grillStatusText != null)
            grillStatusText.gameObject.SetActive(false);

        burger.AddPatty(cookingStatus);
        Debug.Log($"[Voice] Transferred {cookingStatus} patty to burger.");
        return true;
    }

    public void StartCooking()
    {
        grillOn = !grillOn;

        if (grillPatty != null && grillPatty.activeSelf)
        {
            CutletManager cm = grillPatty.GetComponent<CutletManager>();
            if (cm != null) cm.SetCookingState(grillOn);
        }

        foreach (string cutletName in cutletNames)
        {
            GameObject cutlet = GameObject.Find(cutletName);
            if (cutlet != null)
                cutlet.GetComponent<CutletManager>().SetCookingState(grillOn);
        }

        if (grillStatusText != null)
        {
            if (grillOn)
            {
                grillStatusText.text = "Grill: ON";
                grillStatusText.gameObject.SetActive(true);
            }
            else
            {
                grillStatusText.gameObject.SetActive(false);
            }
        }
        Debug.Log($"[Voice] Grill toggled {(grillOn ? "ON" : "OFF")}.");
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Ingredient")
        {
            if (other.gameObject == grillPatty) return;
            if (other.GetComponent<IngredientProperties>().GetPrefabName() == "Cutlet B" && !other.gameObject.GetComponent<ObjectManager>().isGrabbed)
            {
                if (!cutletNames.Contains(other.name))
                {
                    cutletNames.Add(other.name);
                    if (grillOn)
                        other.GetComponent<CutletManager>().SetCookingState(true);
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Ingredient")
        {
            if (other.gameObject == grillPatty) return;
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
