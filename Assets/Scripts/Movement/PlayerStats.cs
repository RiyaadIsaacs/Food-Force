using System;
using UnityEngine;

[DisallowMultipleComponent]
public class PlayerStats : MonoBehaviour
{
    [SerializeField] private int level = 1;
    [SerializeField] private int carryWeight = 5;
    [SerializeField] private int money = 500;

    public int Level => level;
    public int CarryWeight => carryWeight;
    public int Money => money;

    // Player movement speed and jump, adjustable in the inspector
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float sprintSpeed = 8f;

    public float BaseMoveSpeed => moveSpeed;
    public float SprintSpeed => sprintSpeed;

    public void DeductMoney( int foodCost)
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
    }

}
