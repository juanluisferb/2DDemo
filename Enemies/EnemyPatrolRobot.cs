using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


namespace CICEman.Player
{
    public class EnemyPatrolRobot : EnemyGroundMovement
    {

        [SerializeField] float _attackDistance;

        Player _player;
        Vector2 _minAttackDistance;
        Animator _animator;
        float _nextShootTime;
        float _timeBetweenShoots = 2.0f;


        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _player = FindObjectOfType<Player>();
        }

        //Dispara al player cada 2 segundos si está a distancia de tiro y encarándole
        protected override void FixedUpdate()
        {
            base.FixedUpdate();

            if (Vector2.Distance(_player.transform.position, this.transform.position) <= _attackDistance)
            {   
                if (this.transform.localScale == _player.transform.localScale)
                {
                    if(Time.time >= _nextShootTime)
                    {
                        _animator.SetTrigger("isShooting");
                        FlipX();
                        
                        _nextShootTime = Time.time + _timeBetweenShoots;
                    }
                    
                }
                        
               

            }
        }
    }
}


