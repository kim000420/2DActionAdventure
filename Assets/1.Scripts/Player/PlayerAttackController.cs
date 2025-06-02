using UnityEngine;
using UniRx;
using System;
using System.Collections;

public class PlayerAttackController : MonoBehaviour
{
    private PlayerAnimationController anim;
    private PlayerStateController controller;
    private PlayerMotor motor;
    private CompositeDisposable disposables = new CompositeDisposable();

    private enum AttackPhase { None, ComboA, ComboB, ComboC, Strong, Finish }
    private AttackPhase currentPhase = AttackPhase.None;

    private float lastAttackTime = -999f;
    private float attackDirection = 1f;
    private float direction;

    [Header("히트박스")]
    public GameObject[] comboHitboxes;
    public GameObject strongHitbox;
    public GameObject finishHitbox;

    // ComboStep 입력 허용 상태
    private bool comboStep2InputAllowed = false;
    private bool comboStep3InputAllowed = false;
    private bool finishInputQueued = false;

    private void Awake()
    {
        anim = GetComponent<PlayerAnimationController>();
        controller = GetComponent<PlayerStateController>();
        motor = GetComponent<PlayerMotor>();
    }

    private void Start()
    {
        Observable.EveryUpdate()
            .Where(_ => Input.GetKeyDown(KeyCode.A))
            .Subscribe(_ => OnComboInput())
            .AddTo(disposables);

        Observable.EveryUpdate()
            .Where(_ => Input.GetKeyDown(KeyCode.S))
            .Subscribe(_ => OnStrongAttack())
            .AddTo(disposables);
    }

    private void OnComboInput()
    {
        // Idle 또는 Move 상태일 때만 공격 허용
        if (controller.CurrentState != PlayerState.Idle && controller.CurrentState != PlayerState.Moving && controller.CurrentState != PlayerState.Attacking)
        {
            return;
        }
        // 마무리 공격 조건
        if ((currentPhase == AttackPhase.ComboC || currentPhase == AttackPhase.Strong))
        {
            finishInputQueued = true;
            return;
        }

        // 콤보 입력 흐름
        if (currentPhase == AttackPhase.None && controller.CanTransitionTo(PlayerState.Attacking))
        {
            PlayCombo(0);
        }
        else if (comboStep2InputAllowed && currentPhase == AttackPhase.ComboA)
        {
            PlayCombo(1);
        }
        else if (comboStep3InputAllowed && currentPhase == AttackPhase.ComboB)
        {
            PlayCombo(2);
        }
    }


    private void OnStrongAttack()
    {
        if (!controller.CanTransitionTo(PlayerState.Attacking)) return;
        if (currentPhase != AttackPhase.None) return;

        controller.ChangeState(PlayerState.Attacking);
        anim.PlayTrigger("attack_Strong");
        currentPhase = AttackPhase.Strong;
        lastAttackTime = Time.time;
        attackDirection = motor.LastDirection;
        motor.EnableMovementOverride();

        var inputH = controller.GetComponent<PlayerInputHandler>();
        direction = Mathf.Sign(inputH.Horizontal);
        if (direction == 0) direction = controller.GetComponent<SpriteRenderer>().flipX ? -1 : 1;


    }

    private void PlayCombo(int step)
    {
        controller.ChangeState(PlayerState.Attacking);
        anim.PlayTrigger($"attack_Combo_{(char)('A' + step)}");
        currentPhase = (AttackPhase)(step + 1);
        lastAttackTime = Time.time;
        attackDirection = motor.LastDirection;
        motor.EnableMovementOverride();

        var inputH = controller.GetComponent<PlayerInputHandler>();
        direction = Mathf.Sign(inputH.Horizontal);
        if (direction == 0) direction = controller.GetComponent<SpriteRenderer>().flipX ? -1 : 1;

    }

    private void PlayFinishAttack()
    {
        controller.ChangeState(PlayerState.Attacking);
        anim.PlayTrigger("attack_Finish");
        currentPhase = AttackPhase.Finish;

        lastAttackTime = Time.time;
        attackDirection = motor.LastDirection;
        motor.EnableMovementOverride();
        
        var inputH = controller.GetComponent<PlayerInputHandler>();
        direction = Mathf.Sign(inputH.Horizontal);
        if (direction == 0) direction = controller.GetComponent<SpriteRenderer>().flipX ? -1 : 1;


        motor.ForwardImpulse(distance: 1.5f, duration: 0.3f);
    }

    public void EnableHitboxDirect(GameObject hitbox)
    {
        if (hitbox == null) return;

        Vector3 pos = hitbox.transform.localPosition;
        pos.x = Mathf.Abs(pos.x) * attackDirection;
        hitbox.transform.localPosition = pos;

        Vector3 scale = hitbox.transform.localScale;
        scale.x = Mathf.Abs(scale.x) * attackDirection;
        hitbox.transform.localScale = scale;

        hitbox.SetActive(true);
        Invoke(nameof(DisableAllHitboxes), 0.3f);
    }

    private void DisableAllHitboxes()
    {
        foreach (var hit in comboHitboxes)
            hit?.SetActive(false);
        strongHitbox?.SetActive(false);
        finishHitbox?.SetActive(false);
    }

    public void ResetAttackPhase()
    {
        currentPhase = AttackPhase.None;
        comboStep2InputAllowed = false;
        comboStep3InputAllowed = false;
        finishInputQueued = false;
        motor.DisableMovementOverride();
        motor.StopImmediately();
    }

    public void InputStart(int currentStep)
    {
        switch (currentStep)
        {
            case 1:
                comboStep2InputAllowed = true;
                break;
            case 2:
                comboStep3InputAllowed = true;
                break;
            case 3:
                break;
        }
    }

    public void InputStop(int step)
    {
        switch (step)
        {
            case 1:
                comboStep2InputAllowed = false;
                break;
            case 2:
                comboStep3InputAllowed = false;
                break;
            case 3:
                comboStep3InputAllowed = false;
                if (finishInputQueued)
                {
                    PlayFinishAttack();
                }
                else
                {
                    controller.RequestStateChange(PlayerState.Idle);
                    ResetAttackPhase();
                }
                break;
        }
    }
}
