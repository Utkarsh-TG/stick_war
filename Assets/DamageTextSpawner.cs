using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DamageTextSpawner : MonoBehaviour
{
    public Transform playerHead;
    public GameObject attackText;
    public Transform attackTextCanvas;
    
    public void GetEnemyDamage(int damage)
    {
        attackText.GetComponent<TMP_Text>().text = damage.ToString();
        Instantiate(attackText, playerHead.position, transform.rotation, attackTextCanvas);
    }
}
