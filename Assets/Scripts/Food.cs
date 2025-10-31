using UnityEngine;

public class Food : MonoBehaviour
{
    [SerializeField] private float nutritionValue = 0f;

    public float NutritionValue => nutritionValue;
}
