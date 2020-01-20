using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CICEman.Player
{
    public class PlayerShoot : MonoBehaviour
    {
        #region variables
        [SerializeField] FireBall _fireShoot;
        [SerializeField] float _shootForce;
        [SerializeField] Transform _shootPoint;
        [SerializeField] FireBall _chargedShoot;
        [SerializeField] float _fireBallDamage;
        [SerializeField] float _chargedFireBallDamage;
        [SerializeField] AudioClip _shootClip;
        [SerializeField] AudioClip _chargedShootClip;
        #endregion

        private void ChargedShooting()
        {
            FireBall newChargedFireBall = Instantiate(_chargedShoot);
            newChargedFireBall.transform.position = _shootPoint.transform.position;
            newChargedFireBall.transform.localScale = this.transform.localScale;
            AudioSource.PlayClipAtPoint(_chargedShootClip, this.transform.position);
            newChargedFireBall.GetComponent<Rigidbody2D>().AddForce(Vector2.right * this.transform.localScale.x * _shootForce * 1.5f, ForceMode2D.Impulse);
            newChargedFireBall.SetDamage(_chargedFireBallDamage);
        }

        public void Shooting()
        {

            FireBall newFireBall = Instantiate(_fireShoot);
            newFireBall.transform.position = _shootPoint.transform.position;
            newFireBall.transform.localScale = this.transform.localScale;
            AudioSource.PlayClipAtPoint(_shootClip, this.transform.position);
            newFireBall.GetComponent<Rigidbody2D>().AddForce(Vector2.right * this.transform.localScale.x * _shootForce, ForceMode2D.Impulse);
            newFireBall.SetDamage(_fireBallDamage);
        }
    }
}


