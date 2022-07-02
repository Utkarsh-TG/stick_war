using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [System.Serializable]
    public class Enemy
    {
        public float fireRate;
        public float speed;
        public float health;
        public int minDamage;
        public int maxDamage;
    }
    
    [SerializeField]
    public Enemy[] enemyProperties;
}
