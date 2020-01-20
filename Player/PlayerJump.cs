using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CICEman.Player
{
    public class PlayerJump : PlayerState
    {
        #region variables
        [SerializeField] float _jumpSpeed = 10;
        [SerializeField] Vector2 _impactSpeed = new Vector2(3, 3);
        [SerializeField] SpriteRenderer _doubleJump;
        [SerializeField] SpriteRenderer _fireBuster;
        [SerializeField] AudioClip _jumpClip;
        [SerializeField] AudioClip _landClip;

        private bool _landing = false;
        private bool _secondJumpEnabled = false;
        private bool _dashEnabled = false;
        #endregion

        public bool DashEnabled
        {
            get { return _dashEnabled; }
            set { _dashEnabled = value; }
        }

        public bool SecondJumpEnabled
        {
            get { return _secondJumpEnabled; }
            set { _secondJumpEnabled = value; }
        }

        protected override void Awake()
        {
            base.Awake();
        }


        protected override void Update()
        {
            base.Update();
            UpdateDash();
            UpdateIsShootingInAir();
        }


        protected override void OnFixedUpdate()
        {
            if (!_player.GetPlayerIsDead())
            {
                UpdateIsFalling();
            }

            UpdateXVelocity(_jumpSpeed);
            
            UpdateFlipX();
            UpdateConstraints(); 
            UpdateSecondJump();
            _player.UpdateIsGrounded();
            UpdateEndJump();
        }

        private void UpdateIsFalling()
        {
            if(!_player.GetIsGrounded() && _rigidbody.velocity.y <= 0 && !_player.GetPlayerIsDead())
            {
                _rigidbody.gravityScale = 8;
            }
        }

        
        //Método para disparar en el aire
        private void UpdateIsShootingInAir()
        {
            if (Input.GetKeyDown(KeyCode.C))
            {
                if (!_landing)
                {
                    _animator.SetBool("isShootingWhileJumping", true);
                    _fireBuster.enabled = true;
                    Invoke("ReturnToIdleInJump", 0.15f);
                }
                
            }
        }

        //Habilidad de doble salto
        private void UpdateSecondJump()
        {
            if (_secondJumpEnabled)
            {
                if (!_player.GetIsGrounded() && _jumpInput && _canSecondJump)
                {
                    _player.CanDash = false;
                    _canSecondJump = false;
                    Vector3 velocity = _rigidbody.velocity;
                    velocity.y = _jumpSpeed * 2.0f;
                    _rigidbody.velocity = velocity;
                    _doubleJump.enabled = true;
                }
            }
            
        }

        //Habilidad de Dash Aéreo
        void UpdateDash()
        {

            if (_dashEnabled)
            {
                if (_inputDash && _player.CanDash)
                {
                    _player.CanDash = false;
                    _animator.SetBool("isJumping", false);
                    _player.SetNextState(Player.State.Dash);
                } 
            }
        }


        private void OnEnable()
        {
            _animator.SetBool("isJumping", true);
            AudioSource.PlayClipAtPoint(_jumpClip, this.transform.position);

        }

        private void OnDisable()
        {
            _landing = false;
            _doubleJump.enabled = false;
            _animator.SetBool("isJumping", false);

        }

        void UpdateEndJump()
        {
            if (_player.GetPlayerIsDead()) return;

            Vector3 velocity = _rigidbody.velocity;

            if(velocity.y <= 0)
            {
                if (!_landing && _player.CheckIsLanding())
                {
                    _landing = true;
                    _animator.SetTrigger("jumpEnd");
                }

                if (_player.GetIsGrounded())
                {
                    
                    if (!IsInvoking("EndJump"))
                    {
                        _animator.SetBool("isDashing", false);
                        Invoke("EndJump", 0.2f);
                        _rigidbody.gravityScale = 3;
                        AudioSource.PlayClipAtPoint(_landClip, this.transform.position);
                    }
                    
                }
            }
  
        }

        void EndJump()
        {
            _canSecondJump = true;
            _doubleJump.enabled = false;
            _player.SetNextState(Player.State.Default);
        }

        protected override void OnCollisionEnter2D(Collision2D collision)
        {
            base.OnCollisionEnter2D(collision);
            CheckCollisionWithEnemy(collision, _impactSpeed);

        }

        protected override void OnTriggerStay2D(Collider2D collision)
        {
            if (collision.CompareTag("Ladder"))
            {
                _player.SetNextState(Player.State.Climb);
            }
        }

        void ReturnToIdleInJump()
        {
            _fireBuster.enabled = false;
            _animator.SetBool("isShootingWhileJumping", false);
        }
    }
}


