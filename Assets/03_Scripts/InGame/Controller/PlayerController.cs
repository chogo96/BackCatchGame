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
        /// 점수가 한번만 올라가게 체크하는 용도
        /// </summary>
        public bool scoreUpCheck;
        /// <summary>
        /// 플레이어의 체력카운트
        /// </summary>
        public int hpCount { get => _hpCount; set{ } }
        /// <summary>
        /// 공격 가능여부 > 외부에서 변동해야함
        /// </summary>
        public bool canAttack { get => _canAttack; set => _canAttack = value; }
        /// <summary>
        /// 현재 상태
        /// </summary>
        public State currentState { get => _currentState; set { } }
        /// <summary>
        /// 인스펙터에서 등록할 inputActionAsset
        /// </summary>
        public InputActionAsset inputActionAsset;
        /// <summary>
        /// 중력
        /// </summary>
        [SerializeField] private float _gravity;
        /// <summary>
        /// 플레이어 앞뒤 움직임 속도
        /// </summary>
        [SerializeField] private float _moveSpeed;
        /// <summary>
        /// 플레이어 회전 속도
        /// </summary>
        [SerializeField] private float _rotateSpeed;
        /// <summary>
        /// 플레이어 점프력
        /// </summary>
        [SerializeField] private float _jumpPower;
        /// <summary>
        /// 지면에 닿는지 체크하기 위한 거리
        /// </summary>
        [SerializeField] private float _groundDistance;
        /// <summary>
        /// 레이캐스트로 체크할 대상의 레이어마스크
        /// </summary>
        [SerializeField] private LayerMask groundLayer;
        /// <summary>
        /// 공격 후 공격 불가능한 시간
        /// </summary>
        [SerializeField] private float _damageDelay;
        /// <summary>
        /// 캐릭터의 캐릭터 컨트롤러 스크립트
        /// </summary>
        private CharacterController _characterController;
        /// <summary>
        /// 캐릭터의 수직 속도
        /// </summary>
        private float _verticalVelocity;
        /// <summary>
        /// 입력 유무를 확인하는 용도
        /// </summary>
        private bool _onTouching = false;
        /// <summary>
        /// 첫 입력값을 저장할 포지션을 저장한 변수
        /// </summary>
        private Vector3 _startPosition;
        /// <summary>
        /// 실시간으로 업데이트 받는 현재 입력값을 저장한 변수
        /// </summary>
        private Vector3 _currentPosition;
        /// <summary>
        /// 캐릭터가 살아있는지 체크용
        /// </summary>
         public bool isLive;
        /// <summary>
        /// 캐릭터 컨트롤러에 넣을 벡터값 저장용
        /// </summary>
        private Vector3 _moveVector;
        /// <summary>
        /// 캐릭터 컨트롤러에 넣을 점프용 벡터 값
        /// </summary>
        private Vector3 _jumpVector;
        /// <summary>
        /// 첫 입력과 실시간으로 바뀌는 입력값을 뺀 방향 벡터
        /// </summary>
        private Vector3 _direction;
        /// <summary>
        /// 플레이어의 트랜스폼
        /// </summary>
        private Transform _playerTransform;

        [SerializeField]private int _hpCount;
        /// <summary>
        /// 캐릭터가 땅에 닿았는지 체크하는 용도
        /// </summary>
        private bool isGrounded;
        /// <summary>
        /// 사망 시 부활 포인트 포지션 저장용 배열
        /// </summary>
        [SerializeField] private Transform[] _respawnPoint;

        [SerializeField]private State _currentState;
        public bool invisible = false;
        [SerializeField]private bool _canAttack = false;
        /// <summary>
        /// 데미지를 입었는지 체크 용도
        /// </summary>
        public bool _damaged;
        Animator _animator;
        /// <summary>
        /// 지속시간
        /// </summary>
        [SerializeField]private float _respawnTime;
        /// <summary>
        /// 지속 시간 갱신용
        /// </summary>
        [SerializeField]private float _attackDelay;
        /// <summary>
        /// 승리모션 패배모션 한번만 체크
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
        /// 상태 변경시 업데이트용 함수
        /// </summary>
        /// <param name="state">현재 상태</param>
        public void switchStateUpdate(State state)
        {
            switch (state)
            {
                case State.Idle:
                    //Idle 애니메이션 추가
                    _animator.Play("IdleA");
                    break;
                case State.Move:
                    //Move 애니메이션 추가
                    _animator.Play("Run");
                    break;
                case State.Attack:
                    break;
                case State.Damage:
                    //Damage 애니메이션 추가
                    IsDamaged();
                    break;
                case State.Death:
                    //Death 애니메이션 추가
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
        /// 캐릭터의 점프를 담당하는 기능 함수
        /// </summary>
        private void Jump()
        {
            JumpVector();
            _characterController.Move(_jumpVector * Time.deltaTime);
        }
        /// <summary>
        /// 캐릭터의 전,후진을 담당하는 기능 함수
        /// </summary>
        private void Move()
        {
            MovingVector();
            _characterController.Move(_moveVector * Time.deltaTime);
        }
        /// <summary>
        /// 캐릭터를 회전하는 기능 함수
        /// </summary>
        private void Rotate()
        {
            CheckDirection();
            transform.localRotation *= Quaternion.Euler(0f, -_direction.normalized.x * _rotateSpeed * Time.deltaTime, 0f);
        }

        /// <summary>
        /// 레이어 체크해서 각 팀의 인원이 죽는다면 다른 팀의 점수가 올라간다.
        /// </summary>
        public void ScoreUp()
        {
            if (!GameManager.Instance.isPlaying) { return; } //게임플레이 중이 아닐 때에는 점수가 안올라야함.
            if (LayerMask.LayerToName(gameObject.layer) == "TeamA")
            {
                GameManager.teamBScore++;
                Debug.Log("스코어B 올라가는중~");
                scoreUpCheck = false;
            }
            else if (LayerMask.LayerToName(gameObject.layer) == "TeamB")
            {
                scoreUpCheck = false;
                GameManager.teamAScore++;
                Debug.Log("스코어A 올라가는중~");
            }
        }

        /// <summary>
        /// 리스폰을 담당하는 기능적 함수 따로 적는 이유는 FixedUpdate에서 진행해야 할 함수이기 때문에 따로 잡아둠
        /// </summary>
        private void Respawn()
        {
            RespawnPlayer();
            StartCoroutine(ResetRespawnState());
            ResetHP();
        }
        /// <summary>
        /// 플레이어를 리스폰 지역의 포지션으로 옮기는 함수
        /// </summary>
        private void RespawnPlayer()
        {
            _playerTransform.position = _respawnPoint[0].position;
        }
        /// <summary>
        /// 죽음 상태 이후 체력을 초기화 시켜주는 함수 
        /// </summary>
        private void ResetHP()
        {
            _hpCount = 0;
        }
        /// <summary>
        /// 조건 별 함수 체크용
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
                    }//데미지를 입었을 때
                    else
                    {
                        if (_canAttack)
                        {
                            CanMove();
                        }//공격이 가능할 때
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
                }//살아있을 때
                else
                {
                    _currentState = State.Death;
                    switchStateUpdate(this.currentState);
                }//죽어 있을 때
            }//인게임 플레이 중일때
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
        /// fixedUpdate에서 돌아가야해서 여기다가 넣음 수정 요망 TODO 
        /// </summary>
        private void FixedUpdate()
        {
            if(!isLive)
            {
                Respawn();
            }
        }
        /// <summary>
        /// 데미지를 입었을 때 호출하는 함수
        /// 
        /// state를 duration시간동안 데미지로 변경함
        /// </summary>
        public void IsDamaged()
        {
            if (invisible || _damaged)
            {
                return;
            }//중복 데미지의 예외사항
            StartCoroutine(DamageDelay());
            _currentState = State.Damage;
            switchStateUpdate(this.currentState);
            _hpCount++;
        }
        

        /// <summary>
        /// 승패를 구분해서 모션을 플레이 해줌
        /// </summary>
        private void EndMotionPrint()
        {
            Debug.Log("이 함수 불러짐?");
            if (GameManager.Instance.winningTeam == LayerMask.LayerToName(gameObject.layer))//이겼을 때
            {
                _animator.Play("Victory");
                Debug.Log("승리 모션 취하는 중~");
            }//이겼을 때 모션
            else if(GameManager.Instance.winningTeam == "Draw")
            {
                _animator.Play("Tired");
                Debug.Log("비긴 모션 취하는 중~");

            }//비겼을 때
            else
            {
                _animator.Play("Sit");
                Debug.Log("패배 모션 취하는 중~");

            }//졌을 때
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
        /// 죽음상태 이 후 FixedUpdate에서 Idle 상태로 _durationTime만큼의 딜레이 후 Idle상태로 변환
        /// </summary>
        /// <returns></returns>
        IEnumerator ResetRespawnState()
        {
            yield return new WaitForSeconds(_respawnTime);
            isLive = true;
            IsMove();
        }

        /// <summary>
        /// 데미지 중복 방지용 코루틴
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
        /// 움직임을 체크해서 State를 변경하는 함수 (Move, Idle)
        /// </summary>
        void IsMove()
        {
            if (!GameManager.Instance.isPlaying)
            {
                return;
            }//플레이중이 아닐때는 필요없는 것
            if (_onTouching)
            {
                _currentState = State.Move;
                MoveConditionCheck();
                switchStateUpdate(this.currentState);

            }//입력이 있을 때
            else
            {
                _currentState = State.Idle;
                switchStateUpdate(this.currentState);

            }
        }

        /// <summary>
        /// 공격 및 데미지 입을 때에도 움직일 수 있게 직접 함수를 넣음
        /// </summary>
        void CanMove()
        {
            if (_onTouching)
            {
                Movement();
            }//입력이 있을 때
            else
            {
                Movement();
            }
        }
        /// <summary>
        /// 입력 할 벡터 값을 리턴해주는 함수
        /// </summary>
        private Vector3 InputVector(Vector3 input)
        {
            return new Vector3(input.x, input.y, input.z);
        }
        /// <summary>
        /// 현재 벡터 값을 구하는 함수
        /// </summary>
        private void CurrentPosition()
        {
            Vector3 tempInput = Vector3.zero;

#if UNITY_STANDALONE //PC에서는 마우스로 입력값을 받는다
            tempInput = new Vector3(UnityEngine.Input.mousePosition.x, UnityEngine.Input.mousePosition.y, -Camera.main.transform.position.z);

#elif UNITY_ANDROID //안드로이드 환경에서는 터치로 입력값을 받는다.

#endif
            _currentPosition = InputVector(tempInput);
        }
        /// <summary>
        /// 움직임의 관련된 함수를 한번에 모아놓은 곳
        /// </summary>
        private void Movement()
        {
            Jump();
            Move();
            Rotate();
        }

        /// <summary>
        /// 캐릭터의 전진 후진, 회전 , 점프등 전반적인 움직임을 관리하는 함수
        /// </summary>
        private void MoveConditionCheck()
        {
            Movement();
            switchStateUpdate(this.currentState);
        }
        /// <summary>
        /// 전진 후진에 대입할 벡터값을 구하는 함수
        /// </summary>
        private void MovingVector()
        {
            Vector3 move = Vector3.zero;
            CheckDirection();
            move += (-(transform.right * _direction.x) + -(transform.forward * _direction.y)).normalized * _moveSpeed;
            _moveVector = InputVector(move);
        }
        /// <summary>
        /// 점프할 때 대입할 벡터값을 구하는 함수
        /// </summary>
        private void JumpVector()
        {
            Vector3 move = Vector3.zero;
            if (isGrounded && _verticalVelocity < 0)
            {
                _verticalVelocity = 0;
            }//땅에 캐릭터가 닿아 있을 때 수직 속도를 주지 않음
            else
            {
                _verticalVelocity -= _gravity * Time.deltaTime;
            } //캐릭터가 땅에서 떨어져 있을 때는 수직 속도에 중력을 적용함
            move.y = _verticalVelocity;
            _jumpVector = InputVector(move);
        }
        /// <summary>
        /// 첫 입력의 벡터값과 현재 입력의 벡터값을 비교하는 함수
        /// </summary>
        private Vector3 CheckDirection()
        {
            _direction = _startPosition - _currentPosition;
            return _direction; 
        }
        /// <summary>
        /// 점프를 뛸때 지상에 있는지 판단하는 함수 얘는 Update에서 지속적으로 체크를 해줘야함
        /// </summary>
        private void IsJump()
        {
            isGrounded = Physics.Raycast(transform.position, Vector3.down, _groundDistance, groundLayer);
        }
        /// <summary>
        /// 수직 속도를 대입해주는 함수
        /// </summary>
        private void InputVerticalVelocity()
        {
            if (!isGrounded)
            {
                return;
            } //지상에 없을 때의 예외처리
            _verticalVelocity = _jumpPower;
        }
        /// <summary>
        /// 인풋 시스템의 점프 입력을 받으면 활성화 되는 이벤트 함수
        /// </summary>
        /// <param name="contexts"></param>
        public void OnJump(InputAction.CallbackContext contexts)
        {
            if (!isLive)
            {
                return;
            }//죽었을 때의 점프 예외
            //정확하게 눌렀을 때
            if (contexts.performed)
            {
                InputVerticalVelocity();
            }
        }
        /// <summary>
        /// 인풋시스템의 클릭 입력을 받으면 입력 받는지 체크하는 이벤트 함수
        /// </summary>
        /// <param name="context"></param>
        public void OnClick(InputAction.CallbackContext context)
        {
            //눌렀을 때
            if (context.started)
            {
                _onTouching = true;
                //클릭 시작 시 마우스 포지션의 xyz값을 지정한다.
                _startPosition = _currentPosition;
                //IsMove();

            }
            if (context.performed)
            {
                _onTouching = true;
            }
            //땠을 때
            if (context.canceled)
            {
                _onTouching = false;
                _direction = Vector3.zero;
            }
        }
    }
}

