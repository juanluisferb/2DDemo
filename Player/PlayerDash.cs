using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CICEman.Player
{
    public class PlayerDash : PlayerState{

        #region variables
        [SerializeField] float _dashSpeed = 10;
        [SerializeField] Vector2 _impactSpeed = new Vector2(3, 3);
        [SerializeField] SpriteRenderer _dashEffect;
        [SerializeField] SpriteRenderer _fireDashEffect;
        [SerializeField] AudioClip _dashGroundClip;
        [SerializeField] AudioClip _dashAirClip;
        #endregion

        private void OnEnable()
        {
            _rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
            StartCoroutine(StopDashCoroutine());
            _animator.SetBool("isDashing", true);
            

            //Efectos de sprite de dash en el suelo/Dash aéreo
            if (_player.GetIsGrounded())
            {
                _dashEffect.enabled = true;
                AudioSource.PlayClipAtPoint(_dashGroundClip, this.transform.position);
            }
            else
            {
                _fireDashEffect.enabled = true;
                AudioSource.PlayClipAtPoint(_dashAirClip, this.transform.position);
            }
            
            _rigidbody.gravityScale = 0;
            _rigidbody.angularDrag = 0.5f;
            _rigidbody.drag = 0.8f;

            
        }

        private void OnDisable()
        {
            StopAllCoroutines();
            _animator.SetBool("isDashing", false);
            if (_player.GetIsGrounded())
            {
                _dashEffect.enabled = false;
            }
            else
            {
                _fireDashEffect.enabled = false;
            }
            
            _rigidbody.gravityScale = 3f;
            _rigidbody.angularDrag = 0.05f;
            _rigidbody.drag = 0.6f;

            
        }

        IEnumerator StopDashCoroutine()
        {
            yield return new WaitForSeconds(0.7f);
            _player.SetNextState(Player.State.Default);
        }


        protected override void OnFixedUpdate()
        {
            base.OnFixedUpdate();

            Vector2 velocity = _rigidbody.velocity;
            velocity.x = _dashSpeed * this.transform.localScale.x;
            velocity.y = 0;
            _rigidbody.velocity = velocity;
        }

        protected override void OnCollisionEnter2D(Collision2D collision)
        {
            CheckCollisionWithEnemy(collision, _impactSpeed);

        }

        //De dash se puede subir a la escalera (aire y suelo)
        protected override void OnTriggerStay2D(Collider2D collision)
        {

            if (collision.CompareTag("Ladder"))
            {
                _player.SetNextState(Player.State.Climb);
            }
        }

    }
}


