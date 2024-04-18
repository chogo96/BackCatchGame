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
        /// ������ �ѹ��� �ö󰡰� üũ�ϴ� �뵵
        /// </summary>
        public bool scoreUpCheck;
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
        [SerializeField] private float _damageDelay;
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
        public bool invisible = false;
        [SerializeField]private bool _canAttack = false;
        /// <summary>
        /// �������� �Ծ����� üũ �뵵
        /// </summary>
        public bool _damaged;
        Animator _animator;
        /// <summary>
        /// ���ӽð�
        /// </summary>
        [SerializeField]private float _respawnTime;
        /// <summary>
        /// ���� �ð� ���ſ�
        /// </summary>
        [SerializeField]private float _attackDelay;
        /// <summary>
        /// �¸���� �й��� �ѹ��� üũ
        /// </summary>
        private bool _motion = true;
        
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
            StateCheck();

            Debug.Log(_motion);
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
                    break;
                case State.Damage:
                    //Damage �ִϸ��̼� �߰�
                    IsDamaged();
                    break;
                case State.Death:
                    //Death �ִϸ��̼� �߰�
                    ScoreOnceCall();
                    break;
                case State.Win:
                    MotionOnceCall();
                    break;
                case State.Lose:
                    MotionOnceCall();
                    break;
                case State.Draw:
                    MotionOnceCall();
                    break;
            }
        }
        private void ScoreOnceCall()
        {
            if (scoreUpCheck == true)
            {
                _animator.Play("DieA");
                ScoreUp();
                scoreUpCheck = false;
            }
            else
            {
                return;
            }
        }
        private void MotionOnceCall()
        {
            if(_motion == true)
            {
                EndMotionPrint();
                _motion = false;
            }
            else 
            { 
                return; 
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
        /// ���̾� üũ�ؼ� �� ���� �ο��� �״´ٸ� �ٸ� ���� ������ �ö󰣴�.
        /// </summary>
        public void ScoreUp()
        {
            if (!GameManager.Instance.isPlaying) { return; } //�����÷��� ���� �ƴ� ������ ������ �ȿö����.
            if (LayerMask.LayerToName(gameObject.layer) == "TeamA")
            {
                GameManager.teamBScore++;
                Debug.Log("���ھ�B �ö󰡴���~");
                scoreUpCheck = false;
            }
            else if (LayerMask.LayerToName(gameObject.layer) == "TeamB")
            {
                scoreUpCheck = false;
                GameManager.teamAScore++;
                Debug.Log("���ھ�A �ö󰡴���~");
            }
        }

        /// <summary>
        /// �������� ����ϴ� ����� �Լ� ���� ���� ������ FixedUpdate���� �����ؾ� �� �Լ��̱� ������ ���� ��Ƶ�
        /// </summary>
        private void Respawn()
        {
            RespawnPlayer();
            StartCoroutine(ResetRespawnState());
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
        /// ���� ���� ���� ü���� �ʱ�ȭ �����ִ� �Լ� 
        /// </summary>
        private void ResetHP()
        {
            _hpCount = 0;
        }
        /// <summary>
        /// ���� �� �Լ� üũ��
        /// </summary>
        public void StateCheck()
        {
            if(GameManager.Instance.isPlaying) 
            {
                if (isLive)
                {
                    if (_damaged)
                    {
                        CanMove();
                        IsDamaged();
                    }//�������� �Ծ��� ��
                    else
                    {
                        if (_canAttack)
                        {
                            CanMove();
                        }//������ ������ ��
                        else
                        {
                            if (_onTouching)
                            {
                                IsMove();
                            }
                            else
                            {
                                _currentState = State.Idle;
                                switchStateUpdate(this.currentState);
                            }
                        }
                    }
                }//������� ��
                else
                {
                    _currentState = State.Death;
                    switchStateUpdate(this.currentState);
                }//�׾� ���� ��
            }//�ΰ��� �÷��� ���϶�
            else
            {
                if(GameManager.Instance.winningTeam == LayerMask.LayerToName(gameObject.layer))
                {
                    _currentState = State.Win;
                    switchStateUpdate(this.currentState);
                }
                else if(GameManager.Instance.winningTeam == "Draw")
                {
                    _currentState = State.Draw;
                    switchStateUpdate(this.currentState);
                }
                else
                {
                    _currentState = State.Lose;
                    switchStateUpdate(this.currentState);
                }
            }
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
            if (invisible || _damaged)
            {
                return;
            }//�ߺ� �������� ���ܻ���
            StartCoroutine(DamageDelay());
            _currentState = State.Damage;
            switchStateUpdate(this.currentState);
            _hpCount++;
        }
        

        /// <summary>
        /// ���и� �����ؼ� ����� �÷��� ����
        /// </summary>
        private void EndMotionPrint()
        {
            Debug.Log("�� �Լ� �ҷ���?");
            if (GameManager.Instance.winningTeam == LayerMask.LayerToName(gameObject.layer))//�̰��� ��
            {
                _animator.Play("Victory");
                Debug.Log("�¸� ��� ���ϴ� ��~");
            }//�̰��� �� ���
            else if(GameManager.Instance.winningTeam == "Draw")
            {
                _animator.Play("Tired");
                Debug.Log("��� ��� ���ϴ� ��~");

            }//����� ��
            else
            {
                _animator.Play("Sit");
                Debug.Log("�й� ��� ���ϴ� ��~");

            }//���� ��
        }

        public void IsAttack()
        {
            if(_canAttack)
            {
                return;
            }
            StartCoroutine(AttackDelay());
        }
        /// <summary>
        /// �������� �� �� FixedUpdate���� Idle ���·� _durationTime��ŭ�� ������ �� Idle���·� ��ȯ
        /// </summary>
        /// <returns></returns>
        IEnumerator ResetRespawnState()
        {
            yield return new WaitForSeconds(_respawnTime);
            isLive = true;
            IsMove();
        }

        /// <summary>
        /// ������ �ߺ� ������ �ڷ�ƾ
        /// </summary>
        /// <returns></returns>
        IEnumerator DamageDelay()
        {
            if (_damaged)
            {
                yield return null;
            }
            _damaged = true;
            invisible = true;
            _animator.Play("Damage");
            yield return new WaitForSeconds(_damageDelay);
            invisible = false;
            _damaged = false;
        }

        IEnumerator AttackDelay()
        {
            if (_canAttack)
            {
                yield return null;
            }
            _canAttack = true;
            _animator.Play("ATK1");
            _currentState = State.Attack;
            switchStateUpdate(this.currentState);
            yield return new WaitForSeconds(_attackDelay);
            _canAttack = false;
            CanMove();
        }
        /// <summary>
        /// �������� üũ�ؼ� State�� �����ϴ� �Լ� (Move, Idle)
        /// </summary>
        void IsMove()
        {
            if (!GameManager.Instance.isPlaying)
            {
                return;
            }//�÷������� �ƴҶ��� �ʿ���� ��
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
        }

        /// <summary>
        /// ���� �� ������ ���� ������ ������ �� �ְ� ���� �Լ��� ����
        /// </summary>
        void CanMove()
        {
            if (_onTouching)
            {
                Movement();
            }//�Է��� ���� ��
            else
            {
                Movement();
            }
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
        /// �������� ���õ� �Լ��� �ѹ��� ��Ƴ��� ��
        /// </summary>
        private void Movement()
        {
            Jump();
            Move();
            Rotate();
        }

        /// <summary>
        /// ĳ������ ���� ����, ȸ�� , ������ �������� �������� �����ϴ� �Լ�
        /// </summary>
        private void MoveConditionCheck()
        {
            Movement();
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

