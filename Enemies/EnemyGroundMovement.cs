using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CICEman.Player
{
    public class EnemyGroundMovement : MonoBehaviour
    {
        #region variables
        [SerializeField] protected Transform _groundChecker;
        [SerializeField] protected LayerMask _groundLayerMask;
        [SerializeField] protected Transform _wallChecker;
        [SerializeField] protected float _speed;
        [SerializeField] protected Rigidbody2D _rigidbody;
        #endregion


        protected bool _thereIsGround;
        protected bool _thereIsWall;


        protected virtual void FixedUpdate()
        {

            MoveX();
            CheckGround();
            CheckWall();
            FlipX();

        }


        protected void CheckWall()
        {
            Collider2D wall = Physics2D.OverlapCircle(_wallChecker.position, 0.5f, _groundLayerMask);
            _thereIsWall = (wall != null);
        }


        protected void MoveX()
        {
            if (_thereIsGround || !_thereIsWall)
            {
                Vector2 velocity = _rigidbody.velocity;
                velocity.x = _speed;

                _rigidbody.velocity = velocity;
            }

        }


        protected void CheckGround()
        {
            Collider2D ground = Physics2D.OverlapCircle(_groundChecker.position, 0.1f, _groundLayerMask);
            _thereIsGround = (ground != null);
        }


        protected void FlipX()
        {
            if (!_thereIsGround || _thereIsWall)
            {
                _rigidbody.velocity = Vector2.zero;
                Vector3 scale = this.transform.localScale;
                scale.x *= -1;
                _speed *= -1;
                this.transform.localScale = scale;

            }
        }
    }
}

