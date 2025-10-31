using System;
using UnityEngine;

[DisallowMultipleComponent]
public class PlayerStats : MonoBehaviour
{
    [SerializeField] private int level = 1;
    [SerializeField] private int carryWeight = 5;
    [SerializeField] private int money = 500;
    [SerializeField] private GameObject[] inventory;

    [SerializeField] private TMPro.TMP_Text moneyText;

    public int Level => level;
    public int CarryWeight => carryWeight;
    public int Money => money;
    public GameObject[] Inventory => inventory;

    // Player movement speed and jump, adjustable in the inspector
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float sprintSpeed = 8f;

    public float BaseMoveSpeed => moveSpeed;
    public float SprintSpeed => sprintSpeed;

    private void Awake()
    {
        inventory = new GameObject[carryWeight];
    }

    public void DeductMoney(int foodCost)
    {
        if (foodCost < 0)
            throw new ArgumentOutOfRangeException(nameof(foodCost), "Cost cannot be negative.");
        if (money >= foodCost)
        {
            money -= foodCost;
        }
        else
        {
            Debug.LogWarning("Not enough money to deduct.");
        }
        moneyText.text = "Money available:" + money.ToString();
    }

    public void AddMoney(int amount)
    {
        if (amount < 0)
            throw new ArgumentOutOfRangeException(nameof(amount), "Amount to add cannot be negative.");
        money += amount;
        moneyText.text = "Money available:" + money.ToString();
    }

    public bool IsInventoryFull()
    {
        for (int i = 0; i < inventory.Length; i++)
            if (inventory[i] == null)
                return false;

        return inventory.Length > 0; // if length == 0 treat as full
    }

    public void AddFood(GameObject food) 
    {
        for (int i = 0; i < inventory.Length; i++)
        {
            if (inventory[i] == null)
            {
                inventory[i] = food;
                break;
            }
        }
    }

    // Removes and returns the first inventory GameObject that has a Food component.
    public GameObject RemoveFirstFoodItem()
    {
        if (inventory == null || inventory.Length == 0)
            return null;

        for (int i = 0; i < inventory.Length; i++)
        {
            var item = inventory[i];
            if (item != null && item.GetComponent<Food>() != null)
            {
                inventory[i] = null; // remove from inventory
                return item;
            }
        }

        return null;
    }
}
