using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    // Damage is now handled by EnemyAI.OnTriggerEnter2D -> GameManager.LoseLife()
    // This component exists for future expansion (invincibility frames, etc.)
}
