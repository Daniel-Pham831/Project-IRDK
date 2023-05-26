using System;
using Cysharp.Threading.Tasks;
using Game.Interfaces;
using Maniac.CoolDownSystem;
using Maniac.Utils;
using ToolBox.Tags;
using UnityEngine;

namespace Game.Weapons
{
    public class Bullet : MonoBehaviour
    {
        private readonly float _lifeTimeDefault = 4f;
        
        [SerializeField] private Tag obstacleTag;
        
        private BulletData _bulletData;
        private bool _movable = false;
        private Rigidbody2D _rb;
        private Cooldown _aliveCooldown;

        private void Awake()
        {
            _aliveCooldown = new Cooldown();
            _rb = GetComponent<Rigidbody2D>();
            if(_rb == null)
                _rb = gameObject.AddComponent<Rigidbody2D>();
        }

        public async UniTask Setup(BulletData bulletData)
        {
            _bulletData = bulletData;
            _aliveCooldown.UpdateTotalDuration(_lifeTimeDefault);
            StartMoving();
        }

        private void StartMoving()
        {
            _movable = true;
            var currentData = _bulletData;
            _aliveCooldown.StartCooldown(() =>
            {
                if (_bulletData != currentData) return;

                _movable = false;
                Destroy(gameObject);
            });
        }

        private void FixedUpdate()
        {
            if (!_movable) return;
            
            _rb.velocity = _bulletData.Direction * _bulletData.MoveSpeed;
        }

        private async void OnTriggerEnter2D(Collider2D col)
        {
            if (col.HasTag(obstacleTag))
            {
                Destroy(gameObject);
            }

            if (col.TryGetComponent<IKnockBackable>(out var knockBackable))
            {
                if(_bulletData != null)
                {
                    await knockBackable.KnockBack(_bulletData.Direction, _bulletData.KnockBackDistance);
                }
            }
            
            if (col.TryGetComponent<IDamageable>(out var damageable))
            {
                if(_bulletData != null)
                {
                    var isCritical = Helper.IsPercentTrigger(_bulletData.CriticalChance);
                    var damageAmount = isCritical ? _bulletData.Damage * _bulletData.CriticalDamageMultiplier : _bulletData.Damage;

                    await damageable.TakeDamage(damageAmount);
                    Destroy(gameObject);
                }
            }
        }
    }

    [Serializable]
    public class BulletData
    {
        public Vector2 Direction;
        public float Damage;
        public float MoveSpeed;
        public float CriticalChance;
        public float CriticalDamageMultiplier;
        public float KnockBackDistance;
    }
}