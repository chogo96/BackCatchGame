using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;
using static StateManagement;
namespace MJ.Player
{
    public class PlayerController : MonoBehaviour
    {
        /// <summary>
        /// �÷��̾��� ü��ī��Ʈ
        /// </summary>
        public int hpCount { get => _hpCount; set{ } }
        /// <summary>
        /// ���� ���ɿ��� > �ܺο��� �����ؾ���
        /// </summary>
        public bool canAttack { get => _canAttack; set => _canAttack = value; }
        /// <summary>
        /// ���� ����
        /// </summary>
        public State currentState { get => _currentState; set { } }
        /// <summary>
        /// �ν����Ϳ��� ����� inputActionAsset
        /// </summary>
        public InputActionAsset inputActionAsset;
        /// <summary>
        /// �߷�
        /// </summary>
        [SerializeField] private float _gravity;
        /// <summary>
        /// �÷��̾� �յ� ������ �ӵ�
        /// </summary>
        [SerializeField] private float _moveSpeed;
        /// <summary>
        /// �÷��̾� ȸ�� �ӵ�
        /// </summary>
        [SerializeField] private float _rotateSpeed;
        /// <summary>
        /// �÷��̾� ������
        /// </summary>
        [SerializeField] private float _jumpPower;
        /// <summary>
        /// ���鿡 ����� üũ�ϱ� ���� �Ÿ�
        /// </summary>
        [SerializeField] private float _groundDistance;
        /// <summary>
        /// ����ĳ��Ʈ�� üũ�� ����� ���̾��ũ
        /// </summary>
        [SerializeField] private LayerMask groundLayer;
        /// <summary>
        /// ���� �� ���� �Ұ����� �ð�
        /// </summary>
        [SerializeField] private float _attackDelay;
        /// <summary>
        /// ĳ������ ĳ���� ��Ʈ�ѷ� ��ũ��Ʈ
        /// </summary>
        private CharacterController _characterController;
        /// <summary>
        /// ĳ������ ���� �ӵ�
        /// </summary>
        private float _verticalVelocity;
        /// <summary>
        /// �Է� ������ Ȯ���ϴ� �뵵
        /// </summary>
        private bool _onTouching = false;
        /// <summary>
        /// ù �Է°��� ������ �������� ������ ����
        /// </summary>
        private Vector3 _startPosition;
        /// <summary>
        /// �ǽð����� ������Ʈ �޴� ���� �Է°��� ������ ����
        /// </summary>
        private Vector3 _currentPosition;
        /// <summary>
        /// ĳ���Ͱ� ����ִ��� üũ��
        /// </summary>
         public bool isLive;
        /// <summary>
        /// ĳ���� ��Ʈ�ѷ��� ���� ���Ͱ� �����
        /// </summary>
        private Vector3 _moveVector;
        /// <summary>
        /// ĳ���� ��Ʈ�ѷ��� ���� ������ ���� ��
        /// </summary>
        private Vector3 _jumpVector;
        /// <summary>
        /// ù �Է°� �ǽð����� �ٲ�� �Է°��� �� ���� ����
        /// </summary>
        private Vector3 _direction;
        /// <summary>
        /// �÷��̾��� Ʈ������
        /// </summary>
        private Transform _playerTransform;

        [SerializeField]private int _hpCount;
        /// <summary>
        /// ĳ���Ͱ� ���� ��Ҵ��� üũ�ϴ� �뵵
        /// </summary>
        private bool isGrounded;
        /// <summary>
        /// ��� �� ��Ȱ ����Ʈ ������ ����� �迭
        /// </summary>
        [SerializeField] private Transform[] _respawnPoint;

        [SerializeField]private State _currentState;
        [SerializeField]private bool _invisible = false;
        [SerializeField]private bool _canAttack = false;
        /// <summary>
        /// �������� �Ծ����� üũ �뵵
        /// </summary>
        public bool _damaged;
        Animator _animator;
        /// <summary>
        /// ���ӽð�
        /// </summary>
        [SerializeField]private float _durationTime;
        /// <summary>
        /// ���� �ð� ���ſ�
        /// </summary>
        private bool _whileDuration = true;
        private void Awake()
        {
            _characterController = GetComponent<CharacterController>();
            
            this.currentState = State.Idle;

            _animator = GetComponent<Animator>();
            
            _hpCount = 0;

            _playerTransform = GetComponent<Transform>();
        }
        private void Update()
        {
            CurrentPosition();
            IsJump();
            Debug.Log(currentState.ToString());
            StateCheck();
        }
        /// <summary>
        /// ���� ����� ������Ʈ�� �Լ�
        /// </summary>
        /// <param name="state">���� ����</param>
        public void switchStateUpdate(State state)
        {
            switch (state)
            {
                case State.Idle:
                    //Idle �ִϸ��̼� �߰�
                    _animator.Play("IdleA");
                    break;
                case State.Move:
                    //Move �ִϸ��̼� �߰�
                    _animator.Play("Run");
                    break;
                case State.Attack:
                    //Attack �ִϸ��̼� �߰�
                    //�Լ� ������ ������ ����, �ִϸ��̼� �߰� (���� �����䱸)
                    _animator.Play("ATK1");
                    Debug.Log("Animation play called");
                    //Attack();
                    break;
                case State.Damage:
                    //Damage �ִϸ��̼� �߰�
                    DamageHP();
                    break;
                case State.Death:
                    //Death �ִϸ��̼� �߰�
                    _animator.Play("DieA");
                    break;
            }
        }
        /// <summary>
        /// ĳ������ ������ ����ϴ� ��� �Լ�
        /// </summary>
        private void Jump()
        {
            JumpVector();
            _characterController.Move(_jumpVector * Time.deltaTime);
        }
        /// <summary>
        /// ĳ������ ��,������ ����ϴ� ��� �Լ�
        /// </summary>
        private void Move()
        {
            MovingVector();
            _characterController.Move(_moveVector * Time.deltaTime);
        }
        /// <summary>
        /// ĳ���͸� ȸ���ϴ� ��� �Լ�
        /// </summary>
        private void Rotate()
        {
            CheckDirection();
            transform.localRotation *= Quaternion.Euler(0f, -_direction.normalized.x * _rotateSpeed * Time.deltaTime, 0f);
        }


        /// <summary>
        /// �������� ����ϴ� ����� �Լ� ���� ���� ������ FixedUpdate���� �����ؾ� �� �Լ��̱� ������ ���� ��Ƶ�
        /// </summary>
        private void Respawn()
        {
            RespawnPlayer();
            StartCoroutine(ChangeIdle());
            ResetHP();
        }
        /// <summary>
        /// �÷��̾ ������ ������ ���������� �ű�� �Լ�
        /// </summary>
        private void RespawnPlayer()
        {
            _playerTransform.position = _respawnPoint[0].position;
        }
        /// <summary>
        /// �÷��̾��� ü�¿� ������ �����̸� ������ �������� �ִ� �Լ�
        /// </summary>
        private void DamageHP() 
        {
            if (!_invisible)
            {
                return;
            }//���������� �� ���ܸ� �ִ� �Լ�
            StartCoroutine(AttackDelay());
            _hpCount++;
            _animator.Play("Damage");
        }
        /// <summary>
        /// ���� ���� ���� ü���� �ʱ�ȭ �����ִ� �Լ� 
        /// </summary>
        private void ResetHP()
        {
            _hpCount = 0;
        }
        public void StateCheck()
        {
            if (isLive)
            {
                if (!_damaged)
                {
                    return;
                }

                if (_canAttack)
                {
                    _currentState = State.Attack;
                    switchStateUpdate(currentState);
                }//������ ������ ��
                else
                {
                    IsMove();
                }
                
            }//������� ��
            else
            {
                _currentState = State.Death;
                switchStateUpdate(currentState);
            }//�׾� ���� ��
        }
        /// <summary>
        /// fixedUpdate���� ���ư����ؼ� ����ٰ� ���� ���� ��� TODO 
        /// </summary>
        private void FixedUpdate()
        {
            if(!isLive)
            {
                Respawn();
            }
        }
        /// <summary>
        /// �������� �Ծ��� �� ȣ���ϴ� �Լ�
        /// 
        /// state�� duration�ð����� �������� ������
        /// </summary>
        public void IsDamaged()
        {
            if (!_damaged)
            {
                return;
            }//�ߺ� �������� ���ܻ���

            _currentState = State.Damage;
            switchStateUpdate(this.currentState);
        }
        /// <summary>
        /// �������� �� �� FixedUpdate���� Idle ���·� _durationTime��ŭ�� ������ �� Idle���·� ��ȯ
        /// </summary>
        /// <returns></returns>
        IEnumerator ChangeIdle()
        {
            yield return new WaitForSeconds(_durationTime);
            isLive = true;
            _currentState = State.Idle;
            switchStateUpdate(currentState);
        }

        IEnumerator AttackDelay()
        {
            _invisible = true;
            yield return new WaitForSeconds(_attackDelay);
            _invisible = false;
        }
        /// <summary>
        /// �������� üũ�ؼ� State�� �����ϴ� �Լ� (Move, Idle)
        /// </summary>
        void IsMove()
        {
            if (_onTouching)
            {
                _currentState = State.Move;
                MoveConditionCheck();
                switchStateUpdate(this.currentState);

            }//�Է��� ���� ��
            else
            {
                _currentState = State.Idle;
                switchStateUpdate(this.currentState);

            }
            Debug.Log(_onTouching);
        }
        /// <summary>
        /// ���� �ð� üũ�� �ڷ�ƾ �Լ�
        /// </summary>
        /// <param name="result">�������� ��ٰ� 5�� �ڿ� ������ �ٲ�</param>
        /// <returns></returns>
        IEnumerator CheckTime(Action<bool> result)
        {
            bool check = false;
            result(check);
            yield return new WaitForSeconds(_durationTime);
            bool checklater = true;
            result(checklater);
        }

        /// <summary>
        /// �Է� �� ���� ���� �������ִ� �Լ�
        /// </summary>
        private Vector3 InputVector(Vector3 input)
        {
            return new Vector3(input.x, input.y, input.z);
        }
        /// <summary>
        /// ���� ���� ���� ���ϴ� �Լ�
        /// </summary>
        private void CurrentPosition()
        {
            Vector3 tempInput = Vector3.zero;

#if UNITY_STANDALONE //PC������ ���콺�� �Է°��� �޴´�
            tempInput = new Vector3(UnityEngine.Input.mousePosition.x, UnityEngine.Input.mousePosition.y, -Camera.main.transform.position.z);

#elif UNITY_ANDROID //�ȵ���̵� ȯ�濡���� ��ġ�� �Է°��� �޴´�.

#endif
            _currentPosition = InputVector(tempInput);
        }
        /// <summary>
        /// ĳ������ ���� ����, ȸ�� , ������ �������� �������� �����ϴ� �Լ�
        /// </summary>
        private void MoveConditionCheck()
        {
            Jump();
            Move();
            Rotate();
            switchStateUpdate(this.currentState);
        }
        /// <summary>
        /// ���� ������ ������ ���Ͱ��� ���ϴ� �Լ�
        /// </summary>
        private void MovingVector()
        {
            Vector3 move = Vector3.zero;
            CheckDirection();
            move += (-(transform.right * _direction.x) + -(transform.forward * _direction.y)).normalized * _moveSpeed;
            _moveVector = InputVector(move);
        }
        /// <summary>
        /// ������ �� ������ ���Ͱ��� ���ϴ� �Լ�
        /// </summary>
        private void JumpVector()
        {
            Vector3 move = Vector3.zero;
            if (isGrounded && _verticalVelocity < 0)
            {
                _verticalVelocity = 0;
            }//���� ĳ���Ͱ� ��� ���� �� ���� �ӵ��� ���� ����
            else
            {
                _verticalVelocity -= _gravity * Time.deltaTime;
            } //ĳ���Ͱ� ������ ������ ���� ���� ���� �ӵ��� �߷��� ������
            move.y = _verticalVelocity;
            _jumpVector = InputVector(move);
        }
        /// <summary>
        /// ù �Է��� ���Ͱ��� ���� �Է��� ���Ͱ��� ���ϴ� �Լ�
        /// </summary>
        private Vector3 CheckDirection()
        {
            _direction = _startPosition - _currentPosition;
            return _direction; 
        }
        /// <summary>
        /// ������ �۶� ���� �ִ��� �Ǵ��ϴ� �Լ� ��� Update���� ���������� üũ�� �������
        /// </summary>
        private void IsJump()
        {
            isGrounded = Physics.Raycast(transform.position, Vector3.down, _groundDistance, groundLayer);
        }
        /// <summary>
        /// ���� �ӵ��� �������ִ� �Լ�
        /// </summary>
        private void InputVerticalVelocity()
        {
            if (!isGrounded)
            {
                return;
            } //���� ���� ���� ����ó��
            _verticalVelocity = _jumpPower;
        }
        /// <summary>
        /// ��ǲ �ý����� ���� �Է��� ������ Ȱ��ȭ �Ǵ� �̺�Ʈ �Լ�
        /// </summary>
        /// <param name="contexts"></param>
        public void OnJump(InputAction.CallbackContext contexts)
        {
            if (!isLive)
            {
                return;
            }//�׾��� ���� ���� ����
            //��Ȯ�ϰ� ������ ��
            if (contexts.performed)
            {
                InputVerticalVelocity();
            }
        }
        /// <summary>
        /// ��ǲ�ý����� Ŭ�� �Է��� ������ �Է� �޴��� üũ�ϴ� �̺�Ʈ �Լ�
        /// </summary>
        /// <param name="context"></param>
        public void OnClick(InputAction.CallbackContext context)
        {
            //������ ��
            if (context.started)
            {
                _onTouching = true;
                //Ŭ�� ���� �� ���콺 �������� xyz���� �����Ѵ�.
                _startPosition = _currentPosition;
                //IsMove();

            }
            if (context.performed)
            {
                _onTouching = true;
            }
            //���� ��
            if (context.canceled)
            {
                _onTouching = false;
                _direction = Vector3.zero;
            }
        }
    }
}

