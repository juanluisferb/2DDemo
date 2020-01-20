using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CICEman.Player;

public class EnemyTurret : MonoBehaviour {

    #region variables
    Animator _animator;
    Player _player;
    [SerializeField] float _distanceToAct;
    [SerializeField] Transform _shootPoint;
    [SerializeField] FireBall _fireShoot;
    [SerializeField] float _fireBallDamage;
    [SerializeField] float _shootForce;
    AnimatorStateInfo info;
    SpriteRenderer _spriteRenderer;
    Vector3 correction = new Vector3(0, 2.2f, 0);

    bool _hasSpawned = false;

    #endregion

    private void Awake()
    {
        _player = FindObjectOfType<Player>();
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

   
    private void Update()
    {
        
        //Trackea al player con la corrección del pivote de éste
        float distanceToPlayer = Vector2.Distance(_shootPoint.transform.position, _player.transform.position + correction);
        if(distanceToPlayer <= _distanceToAct)
        {
            //Primero se spawnea y luego desde Idle, dispara
            info = _animator.GetCurrentAnimatorStateInfo(0);
            if (_hasSpawned)
            {
                if (info.IsName("Enemy_turret_idle"))
                {

                    _animator.SetTrigger("isFiring");
                }
                
            }
            else
            {
                _animator.SetTrigger("isSpawning");
                _spriteRenderer.enabled = true;
                _hasSpawned = true;
            }



        }
    }

    void Fire()
    {
        FireBall newFireBall = Instantiate(_fireShoot);
        newFireBall.transform.position = _shootPoint.transform.position;
        Vector2 _playerPosition = (_player.transform.position + correction) - _shootPoint.transform.position;

        newFireBall.GetComponent<Rigidbody2D>().AddForce(_playerPosition * _shootForce, ForceMode2D.Impulse);
        newFireBall.SetDamage(_fireBallDamage);
    }




}
