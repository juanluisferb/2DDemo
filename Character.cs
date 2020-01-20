using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace CICEman.Player
{
    public class Character : MonoBehaviour
    {

        #region Variables
        [SerializeField] protected float _maxLife = 100;
        [SerializeField] bool _destroyOnDie = true;
        [SerializeField] Animator _explosionAnimatorPrefab;
        AnimatorStateInfo info;
        Vector3 correction = new Vector3(0, 2.0f, 0);

        protected float _currentLife;
        #endregion



        protected virtual void Start()
        {
            _currentLife = _maxLife;
        }

        public float GetMaxLife() { return _maxLife; }
        public float GetCurrentLife() { return _currentLife; }

        //Método de infligir daño a personajes tanto el player como el enemigo
        public void TakeDamage(float damage)
        {
            Debug.Assert(damage > 0, "El daño ha de ser positivo");
            damage = Mathf.Abs(damage);
            _currentLife = Mathf.Max(_currentLife - damage, 0);

            
            if (_currentLife == 0)
            {
                Die();
            }
            
        }

        //Método de curar
        public virtual void RestoreLife(float health)
        {
            Debug.Assert(health > 0, "La variable health tiene que ser positiva");
            health = Mathf.Abs(health);
            _currentLife = Mathf.Min(_currentLife + health, _maxLife);

        }

        //Método básico de muerte, el animator es para que los enemigos muestren una explosión
        public virtual void Die()
        {

            if (_destroyOnDie)
            {
                Animator spriteExplosion = Instantiate(_explosionAnimatorPrefab);
                spriteExplosion.transform.position = this.transform.position + correction;

                info = spriteExplosion.GetCurrentAnimatorStateInfo(0);
                if (info.length > info.normalizedTime)
                {
                    Destroy(spriteExplosion.gameObject, 0.4f);
                    Destroy(this.gameObject, 0.42f);
                }

            }   
        }
    }
}


