using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CICEman.Player;

namespace CICEman.Player
{
    public class PlayerClimb : PlayerState
    {
        [SerializeField] float _climbSpeed;

        protected override void Update()
        {
            base.Update();
        }

        protected override void OnFixedUpdate()
        {
            UpdateYVelocity(_climbSpeed);
            UpdateFlipX();
            UpdateXVelocity(_climbSpeed);

            //Moverse en Y
            if (_yInput != 0)
            {
                _animator.enabled = true;
                _animator.SetBool("isClimbing", Mathf.Abs(_rigidbody.velocity.y) > 0.1f);
    
            }

            //Moverse en X
            else if(_xInput != 0)
            {
                _animator.enabled = true;
                _animator.SetBool("isClimbing", Mathf.Abs(_rigidbody.velocity.x) > 0.1f);
                
            }
            //Parar la animación en la escalera si no hay input
            else
            {
                _animator.enabled = false;
            }
            

        }

        private void OnEnable()
        {
            _rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
            _rigidbody.gravityScale = 0;
            _animator.SetBool("isClimbing", true);
        }

        private void OnDisable()
        {
            _animator.SetBool("isClimbing", false);
            _rigidbody.gravityScale = 2;
            
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.CompareTag("Ladder"))
            {
                _player.SetNextState(Player.State.Default);
            }
            
        }


    }
}


