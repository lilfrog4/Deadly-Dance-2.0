using UnityEngine;

public class Attacker : MonoBehaviour
{
    [SerializeField]
    private float _damage = 1; // урон, наносимый нотой
    private float Damage => _damage;
}
