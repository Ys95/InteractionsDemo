using System;
using PuzzleDungeon.Character;
using UnityEngine;

namespace Tide
{
    public class ObjectHealth : MonoBehaviour
    {
        [SerializeField] private float maxHealth = 100;
        [SerializeField] private float initialHealth = 100;

        private float        _currentHealth;
        private CharacterHub _characterHub;

        private void Awake()
        {
            Initialize();
        }

        public void Initialize()
        {
            _currentHealth = initialHealth;
            _characterHub  = GetComponent<CharacterHub>();
        }

        public void DealDamage(float amount)
        {
            _currentHealth = Mathf.Max(0, _currentHealth - amount);
            if (_currentHealth == 0)
            {
                Kill();
            }
        }

        public void Kill()
        {
            Destroy(gameObject);
        }
    }
}