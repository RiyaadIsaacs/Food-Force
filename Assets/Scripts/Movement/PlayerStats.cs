using System;
using UnityEngine;

[DisallowMultipleComponent]
public class PlayerStats : MonoBehaviour
{
    [SerializeField] private int level = 1;
    [SerializeField] private int carryWeight = 5;

    public int Level => level;

    // Player movement speed and jump, adjustable in the inspector
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float sprintSpeed = 8f;
    [SerializeField] private float jumpForce = 5f;

    public float BaseMoveSpeed => moveSpeed;
    public float SprintSpeed => sprintSpeed;

}
