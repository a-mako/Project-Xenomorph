using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemInteraction : MonoBehaviour
{
    [SerializeField] private ItemData requiredItem;
    private new Renderer renderer;

    private void Awake() {
        renderer = GetComponent<Renderer>();
    }

    private void OnEnable() {
        EventBus.Instance.onItemUsed += OnItemUsed;
    }
    private void OnDisable() {
        EventBus.Instance.onItemUsed -= OnItemUsed;
    }

    private void OnItemUsed(ItemData item)
    {
        if (item.Type == ItemData.ItemType.KeyItem) {
            if (Vector3.Distance(TankController.Instance.transform.position, transform.position) < 3) {
                if (item == requiredItem) {
                    renderer.material.color = new Color(255, 140, 0, 1);
                }
            }
        }
        else if (item.Type == ItemData.ItemType.Consumable) {
            if (item == requiredItem) {
                    renderer.material.color = new Color(232, 0, 254, 1);
                }
        }
    }
}
