using UnityEngine;

public class ScreenTransition : MonoBehaviour
{
    public Transform targetWorld; // Сюда перетащите World2
    public GameObject player;      // Сюда перетащите игрока

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Телепортируем игрока (камера телепортируется автоматически, так как она дочерняя)
            player.transform.position = targetWorld.position;
        }
    }
}