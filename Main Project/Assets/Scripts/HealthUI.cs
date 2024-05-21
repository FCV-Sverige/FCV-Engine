using UnityEngine;
using TMPro; // Use UnityEngine.UI if using UI Text instead

public class HealthUI : MonoBehaviour
{
    public PlayerController player; // Reference to the PlayerController script
    public TextMeshProUGUI healthText; // Use Text if using UI Text instead

    void Start()
    {
        if (player == null)
        {
            Debug.LogError("Player reference is missing!");
        }
        if (healthText == null)
        {
            Debug.LogError("Health Text reference is missing!");
        }
    }

    void Update()
    {
        if (player != null && healthText != null)
        {
            healthText.text = "Health: " + player.Health;
        }
    }
}
