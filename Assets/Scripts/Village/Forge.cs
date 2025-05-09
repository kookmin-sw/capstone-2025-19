using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.UI;

public class Forge : MonoBehaviour
{
    [SerializeField] GameObject ForgeCanvas;
    [SerializeField] Text ItemName;
    [SerializeField] Slider DurabilitySlider;
    [SerializeField] Text RepairCost;
    [SerializeField] private Image itemInputImage;
    [SerializeField] Button RepairButton;

    Item curItem;
    int cost = 0;

    // Start is called before the first frame update
    void Start()
    {
        ForgeCanvas.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ForgeCanvas.SetActive(true);
            SetWeaponToItemPanel();
        }
    }

    public void SetWeaponToItemPanel()
    {
        ItemIcon currentWeapon = InventoryController.Instance.weaponPanel.GetWeaponItemIcon();
        if (currentWeapon == null) return;

        curItem = currentWeapon.item;
        SetItemIcon(curItem.itemData.name);

        ItemName.text = curItem.itemData.name;
        DurabilitySlider.value = curItem.durability / curItem.itemData.maxItemDurability;
        cost = 100 * (int)(100 - 100*DurabilitySlider.value);
        RepairCost.text = cost + " Gold";

        int curMoney = InventoryController.Instance.money;
        if (curMoney < cost)
        {
            RepairButton.interactable = false;
        }
    }

    public void SetItemIcon(string iconName)
    {

        Sprite icon = Resources.Load<Sprite>($"Sprites/ItemIcon/{iconName}");
        if (icon == null)
        {
            Debug.LogError($"[IconLoader] {iconName} 스프라이트를 찾을 수 없습니다!");
            return;
        }

        itemInputImage.sprite = icon;
    }

    public void Repair()
    {
        curItem.durability = curItem.itemData.maxItemDurability;
        InventoryController.Instance.money -= cost;
    }

    public void Cancel()
    {
        ForgeCanvas.SetActive(false);
    }
}
