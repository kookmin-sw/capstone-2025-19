using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropItem : MonoBehaviour
{
    public Item item;
    public GameObject go { get; set; }

    private Collider _collider;

    private void Init()
    {
        _collider = GetComponent<Collider>();
        if (item == null)
        {
            item = GetComponent<Item>();
            item.dropItem = this.gameObject;
            go = gameObject;
        }
        item.dropItem = this.gameObject;
    }

    private void OnEnable()
    {
        Init();
        StartCoroutine(EnableColliderAfterDelay(_collider, 0.1f));
    }

    IEnumerator EnableColliderAfterDelay(Collider collider, float delay)
    {
        yield return new WaitForSeconds(delay);
        collider.enabled = true;
    }


    public void SetItem(Item item)
    {
        this.item = item;
        item.dropItem = this.gameObject;
    }
    public void SetModel()
    {
        if (item == null)
        {
            Debug.LogError($"{this.gameObject.name} is null");
            Init();
        }
    }

    /*private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"collider {item.name}");
        if (other.CompareTag("Player"))
        {

            //InventoryController.Instance.CreateDropItemToInventory(item);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!gameObject.activeSelf) { return; }
            //item.itemIcon.GetComponent<ItemIcon>().NullItemGrid();
            //InventoryController.Instance.RemoveItemIcon(item);
        }
    }*/

    public void ActiveCollider(bool value)
    {
        _collider.enabled = value;
    }
}
