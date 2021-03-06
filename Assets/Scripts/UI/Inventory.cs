﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;
using DG.Tweening;

public class Inventory : MonoBehaviour {
    [SerializeField] private GameObject UI = null;
    [SerializeField] private DragDrop SwapDragDrop = null;
    private CanvasGroup playerCanvasGroup;
    [SerializeField] GameObject player = null;
    [SerializeField] public List<DragDrop> abilityItems;
    private Camera mainCamera;

    private void Awake() {
        abilityItems = GetComponentsInChildren<DragDrop>().ToList();
        mainCamera = Camera.main;
        playerCanvasGroup = GetComponentInParent<CanvasGroup>();
        UI.GetComponent<Canvas>().worldCamera = Camera.main;
    }

    private void OnEnable() {
        InitInventorySlots();
    }

    public void InitInventorySlots() {
        var abilitiesSlots = GameManager.Instance.MaxAmountOfAbilities;
        int occ = 0;

        for (int i = 0; i < abilityItems.Count; i++) {
            if (occ < abilitiesSlots)
                abilityItems[i].transform.parent.gameObject.SetActive(true);
            else
                abilityItems[i].transform.parent.gameObject.SetActive(false);
            occ++;
        }
    }

    public void ShowHideSwapUI(bool isShowing, Ability swapableAbility)
    {
        if (swapableAbility != null)
        {
            SwapDragDrop.SetAbility(swapableAbility);
        }

        StartCoroutine(ShowHidePlayerInterface(isShowing));
    }

    private IEnumerator ShowHidePlayerInterface(bool isShowing) {
        if (isShowing) {
            UI.transform.DOMoveY(2.20f, .25f).WaitForCompletion();
            UI.transform.DOScale(Vector3.one, .5f);    
        }
        else {
            UI.transform.DOScale(Vector3.zero, .25f).WaitForCompletion();
            UI.transform.DOMoveY(1.5f, .5f);
        }
        yield return null;
    }

    public void AddAbility(Ability ability)
    {
        for (int i = 0; i < abilityItems.Count; i++)
        {
            if (abilityItems[i].ability == null)
            {
                abilityItems[i].SetAbility(ability);
                break;
            }
        }
    }

    public void RemoveAbility(Ability ability)
    {
        for (int i = 0; i < abilityItems.Count; i++)
        {
            if (abilityItems[i].ability == ability)
            {
                abilityItems[i].SetAbility(null);
                break;
            }
        }
    }

    public void FlushUI()
    {
        for (int i = 0; i < abilityItems.Count; i++)
        {
            abilityItems[i].SetAbility(null);
        }
    }
}
