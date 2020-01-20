using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CICEman.Player
{
    public class SpecialDash : PlayerState
    {
        [SerializeField] float _dashSpeed = 15;
        [SerializeField] Transform _invincibleAura;


        private void OnEnable()
        {
            StartCoroutine(StopSpecialDashCoroutine());
            _animator.SetTrigger("specialDash");
            _rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
            _rigidbody.gravityScale = 0.10f;

 
            if (GameManager.singleton.selectedCharacter == 1)
            {
                
                //Aura on
                _invincibleAura.gameObject.SetActive(true);
            }
        }

        private void OnDisable()
        {
            _rigidbody.gravityScale = 2.0f;

            
            if (GameManager.singleton.selectedCharacter == 1)
            {
                //aura off
                _invincibleAura.gameObject.SetActive(false);
            }
        }

        protected override void OnFixedUpdate()
        {
            base.OnFixedUpdate();

            Vector2 velocity = _rigidbody.velocity;
            velocity.x = _dashSpeed * this.transform.localScale.x;
            _rigidbody.velocity = velocity;
        }

        IEnumerator StopSpecialDashCoroutine()
        {
            yield return new WaitForSeconds(0.9f);
            _player.SetNextState(Player.State.Default);
        }

        protected override void OnTriggerStay2D(Collider2D collision)
        {

            if (collision.CompareTag("Ladder"))
            {
                _player.SetNextState(Player.State.Climb);
            }
        }
    }
}

