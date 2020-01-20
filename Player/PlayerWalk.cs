using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CICEman.Player
{
    public class PlayerWalk : PlayerState
    {

        #region Variables
        [SerializeField] float _walkSpeed = 1;
        [SerializeField] float _jumpSpeed = 5;
        [SerializeField] Vector2 _impactSpeed = new Vector2(3, 3);
        [SerializeField] float _idleCooldown = 0.3f;
        [SerializeField] SpriteRenderer _fireBuster;
        [SerializeField] Animator _auraAnimator;
        [SerializeField] AudioSource _auraClip;
        [SerializeField] Animator _auxBar;

        private bool _canShoot = false;
        private float _chargeTime = 0;
        private float _chargeRate = 1.7f;
        private bool _dashEnabled = false;
        private bool _chargedShootEnabled = false;
        private bool _specialDash;
        private float _nextShootTime;
        private float _nextShootTimeRunning;
        private float _timeBetweenShoots = 0.15f;

        #endregion

        public bool DashEnabled
        {
            get { return _dashEnabled; }
            set { _dashEnabled = value; }
        }

        public bool ChargedShootEnabled
        {
            get { return _chargedShootEnabled; }
            set { _chargedShootEnabled = value; }
        }


        protected override void Awake()
        {
            base.Awake();

        }

        private void OnEnable()
        {
            _animator.SetBool("isWalking", true);
            _player.CanDash = true;
        }

        protected override void Update()
        {
            if (!_player.GetPlayerIsDead())
            {
                base.Update();
                CheckCanChargeShoot();
                UpdateDash();
                _specialDash = Input.GetKeyDown(KeyCode.B);
            }
            
        
        }

        protected override void OnFixedUpdate()
        {
            if (!_player.GetPlayerIsDead())
            {
                UpdateXVelocity(_walkSpeed);
                UpdateFlipX();
                UpdateConstraints();
                _animator.SetBool("isWalking", Mathf.Abs(_rigidbody.velocity.x) > 0.1f);

                UpdateIsShooting();
                _player.UpdateIsGrounded();
                _player.CheckWall();
                UpdateSpecialDash();
                UpdateWall();
                UpdateBeginJump();
            }
           
           

        }
        //Si es el player 2, puede hacer el Dash especial
        private void UpdateSpecialDash()
        {
            if(_specialDash && 
                _player.CanDash && 
                GameManager.singleton.selectedCharacter == 1
                && _player._isGrounded)
            {
                _player.CanDash = false;
                _animator.SetBool("isWalking", false);
                _player.SetNextState(Player.State.SpecialDash);
            }
            
        }
        //Comienzo corrutina disparo cargado en Update
        private void CheckCanChargeShoot()
        {
            if (_chargedShootEnabled)
            {
                if (!_canShoot && _player._isGrounded)
                {
                    StartCoroutine(TimerRoutine());

                } 
            }
        }

        private void OnDisable()
        {
            _animator.SetBool("isWalking", false);
            StopAllCoroutines();
            ReturnToIdle();
            
        }


        private void UpdateIsShooting()
        {
            //Disparo en el suelo quieto
            if (Input.GetKeyDown(KeyCode.C))
            {
                if (Time.time >= _nextShootTime)
                {
                    _animator.SetBool("isShooting", true);
                    Invoke("ReturnToIdle", _idleCooldown);
                    _nextShootTime = Time.time + _timeBetweenShoots;
                }
            }

            //Disparo en el suelo en movimiento
            if (Input.GetKeyDown(KeyCode.C) && _rigidbody.velocity.x != 0)
            {
                if (Time.time >= _nextShootTimeRunning)
                {
                    _fireBuster.enabled = true;
                    _animator.SetBool("isShootingWhileRunning", true);
                    Invoke("ReturnToIdle", _idleCooldown);
                    _nextShootTimeRunning = Time.time + _timeBetweenShoots;
                }
            }
        }

        //Corrutina disparo cargado 2 segundos
        IEnumerator TimerRoutine()
        {
            //Si se pulsa la V, empieza la carga con un aura y sonido
            _canShoot = true;
            if (Input.GetKeyDown(KeyCode.V))
            {
                _auxBar.SetBool("Activate", true);
                _auraAnimator.SetBool("isCharging", true);
                _auraClip.PlayOneShot(_auraClip.clip);
                yield return new WaitForSeconds(1.7f);

                _chargeTime += _chargeRate;
            }
            //Si se mantiene pulsada, el aura cambia a cargado
            if (Input.GetKey(KeyCode.V) && Time.time > _chargeTime)
            {
                _auraAnimator.SetBool("isCharging", false);
                _auraAnimator.SetBool("isCharged", true);
                _auxBar.SetBool("Activate", false);
                _auxBar.SetBool("Activated", true);

                if (_auraClip.isPlaying)
                {
                    _auraClip.Stop();
                }
                
            }
            //Si a partir de los 2 segundos se suelta el botón, se hace el disparo cargado
            if (Input.GetKeyUp(KeyCode.V) && Time.time >= _chargeTime)
            {
                _auraAnimator.SetBool("isCharged", false);
                _auxBar.SetBool("Activated", false);
                if (_auraClip.isPlaying)
                {
                    _auraClip.Stop();
                }
                _animator.SetBool("isChargedShooting", true);
                Invoke("ReturnToIdle", 0.7f);
                _chargeTime = 0;
            }
            else
            {
                //Si se suelta antes, se cancela la carga y las auras
                _auraAnimator.SetBool("isCharging", false);
                _auxBar.SetBool("Activate", false);
                if (_auraClip.isPlaying)
                {
                    _auraClip.Stop();
                }
                _chargeTime = 0;
                StopAllCoroutines();
            }

            _canShoot = false;
        }

        //Comienzo salto
        void UpdateBeginJump()
        {
            if (!_player.GetIsGrounded())
            {
                _player.SetNextState(Player.State.Jump);
            }

            else if (_player.GetIsGrounded() && _jumpInput)
            {
                Vector3 velocity = _rigidbody.velocity;
                velocity.y = _jumpSpeed;
                _rigidbody.velocity = velocity;
                _player.SetNextState(Player.State.Jump);
            }
        }
        //Método para pararse delante de una pared 
        void UpdateWall()
        {
            
            if (_player._thereIsWall)
            {
                _animator.SetBool("isWalking", false);
            }
        }

        //Dash en el suelo
        void UpdateDash()
        {
            
            if (_dashEnabled)
            {
                if (_inputDash && _player.CanDash)
                {
                    _player.CanDash = false;
                    _animator.SetBool("isWalking", false);
                    _player.SetNextState(Player.State.Dash);
                } 
            }
        }

        protected override void OnCollisionEnter2D(Collision2D collision)
        {
            CheckCollisionWithEnemy(collision, _impactSpeed);

        }
        
        //Método para trepar escaleras
        protected override void OnTriggerStay2D(Collider2D collision)
        {
            
            if (collision.CompareTag("Ladder") && _yInput > 0)
            {
                _player.SetNextState(Player.State.Climb);
            }
        }

        //Método para resetear variables de animators y disparos
        void ReturnToIdle()
        {
            
            if (IsInvoking("UpdateIsShooting"))
            {
                CancelInvoke();
            }

            if (!IsInvoking("UpdateIsShooting"))
            {
                _fireBuster.enabled = false;

                if(_animator != null)
                {
                    _animator.SetBool("isShooting", false);
                    _animator.SetBool("isShootingWhileRunning", false);
                    _animator.SetBool("isChargedShooting", false);
                }
                if(_auraAnimator != null)
                {
                    
                    _auraAnimator.SetBool("isCharging", false);
                    _auraAnimator.SetBool("isCharged", false);

                }
                if(_auxBar != null)
                {
                    _auxBar.SetBool("Activate", false);
                    _auxBar.SetBool("Activated", false);
                }
            }
            
            
        }
    }
}



