using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CICEman.Player {

    public class PlayerState : MonoBehaviour
    {
        #region variables
        [SerializeField] Player.State _state;

        protected Animator _animator;
        protected Player _player;
        protected Rigidbody2D _rigidbody;
        protected PlayerShoot _playerShoot;
        protected float _xInput;
        protected float _yInput;
        protected bool _jumpInput;
        protected bool _inputDash = false;
        protected bool _canSecondJump = true;
        #endregion


        public Player.State State { get { return _state; } }

        protected virtual void Awake()
        {
            _player = GetComponent<Player>();
            _rigidbody = GetComponent<Rigidbody2D>();
            _animator = GetComponent<Animator>();
            _playerShoot = GetComponent<PlayerShoot>();
        }

        

        protected virtual void Update()
        {
            if (!_player.GetPlayerIsDead())
            {
                _xInput = Input.GetAxisRaw("Horizontal");
                _yInput = Input.GetAxisRaw("Vertical");
                _jumpInput = _jumpInput || Input.GetKeyDown(KeyCode.X);
                _inputDash = Input.GetKeyDown(KeyCode.Z);
            }
            

        }


        void FixedUpdate()
        {
            OnFixedUpdate();
            _jumpInput = false;
        }


        protected virtual void OnFixedUpdate()
        {
            //No implementado
        }


        protected virtual void OnCollisionEnter2D(Collision2D collision)
        {
            //No implementado
        }


        protected virtual void OnTriggerStay2D(Collider2D collision)
        {
            //No implementado
        }


        protected void CheckCollisionWithEnemy(Collision2D collision, Vector2 impactSpeed)
        {
            
                if ((_player._enemiesLayerMask.value & (1 << collision.gameObject.layer)) > 0)
                {

                //El player 2 es más fuerte y resiste el doble a las colisiones con los enemigos
                if(GameManager.singleton.selectedCharacter == 1)
                {
                    _player.TakeDamage(2.5f);
                }
                else
                {
                    _player.TakeDamage(5.0f);
                }
                    

                //Se comprueba que la vida no esté a 0 y el player no esté en estado dañado ya
                    if(_player.GetCurrentLife() > 0 && _player.GetCurrentState() != Player.State.Damaged)
                    {

                        if (collision.collider.transform.position.x > this.transform.position.x)
                        {
                        impactSpeed.x = -impactSpeed.x;

                        }
                        _rigidbody.velocity = impactSpeed;
                        _player.SetNextState(Player.State.Damaged);
                    }
                    

                }

            
            
        }

        //Movimiento en X
        protected void UpdateXVelocity(float speed)
        {

            Vector2 velocity = _rigidbody.velocity;
            velocity.x = _xInput * speed;

            _rigidbody.velocity = velocity;


        }

        //Movimiento en Y
        protected void UpdateYVelocity(float speed)
        {

            Vector2 velocity = _rigidbody.velocity;
            velocity.y = _yInput * speed;

            _rigidbody.velocity = velocity;


        }


        //Constraints del Player
        protected virtual void UpdateConstraints()
        {
            RigidbodyConstraints2D constraints = RigidbodyConstraints2D.FreezeRotation;
            if (_xInput == 0)
            {
                constraints = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionX;

            }
            _rigidbody.constraints = constraints;
        }



        //Orientación
        protected void UpdateFlipX()
        {
            Vector2 velocity = _rigidbody.velocity;
            if (velocity.x < 0)
            {
                this.transform.localScale = new Vector3(-1, 1, 1);

            }
            else if (velocity.x > 0)
            {
                this.transform.localScale = new Vector3(1, 1, 1);
            }
        }

        

    }
}



