using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CICEman.Player;

    public class EnemyShoot : MonoBehaviour
    {
    #region variables
        [SerializeField] FireBall _fireShoot;
        [SerializeField] float _shootForce;
        [SerializeField] Transform _shootPoint;
        [SerializeField] float _fireBallDamage;
    #endregion

    public void EnemyShooting()
        {
            //Disparo enemigo
            FireBall newFireBall = Instantiate(_fireShoot);
            newFireBall.transform.position = _shootPoint.transform.position;
            newFireBall.GetComponent<Rigidbody2D>().AddForce(Vector2.right * -this.transform.localScale.x * _shootForce, ForceMode2D.Impulse);
            newFireBall.SetDamage(_fireBallDamage);
        }
    }

    


