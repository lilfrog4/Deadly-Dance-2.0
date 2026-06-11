using UnityEngine;
using UnityEngine.UI;

public class PlayerBounds : MonoBehaviour
{
    [SerializeField] private float minX = -18f;
    [SerializeField] private float maxX = Mathf.Infinity;
    [SerializeField] private Text boundsText; // Перетащи сюда UI Text для отображения
    
    private Transform player;
    
    private void Start()
    {
        player = GetComponent<Transform>();
        if (player == null)
            player = FindObjectOfType<PlayerController>().transform;
    }
    
    private void LateUpdate()
    {
        if (player == null) return;
        
        Vector3 pos = player.position;
        float oldX = pos.x;
        
        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        player.position = pos;
        
        // Отображаем значения на экране
        if (boundsText != null)
        {
            boundsText.text = $"X: {player.position.x:F2} | Min: {minX} | Max: {maxX}";
        }
    }
}