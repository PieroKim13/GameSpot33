using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    PlayerController controller;
    IEnumerator sprintCorutine;

    public Action<float> onStaminaChange { get; set; }

    bool IsAlive => hp > 0;

    private void Start()
    {
        sprintCorutine = sprintDerease();
    }

    private void Awake()
    {
        controller = GetComponent<PlayerController>();
        controller.onSprinting = () => StaminaChangeCoroutine(true);
        controller.offSprinting = () => StaminaChangeCoroutine(false);

    }

    /// <summary>
    /// 체력
    /// </summary>
    float hp = 100.0f;
    public float HP
    {
        get => hp;
        set
        {
            if (IsAlive)
            {
                hp = value;
                if (hp <= 0)
                {
                    hp = Mathf.Clamp(hp, 0, MaxHP);

                }
            }
        }
    }
    /// <summary>
    /// 최대 체력
    /// </summary>
    float maxHP = 100.0f;
    public float MaxHP => maxHP;

    /// <summary>
    /// 스테미나
    /// </summary>
    float stamina = 100.0f;
    public float Stamina
    {
        get => stamina;
        set
        {
            if (IsAlive)
            {
                stamina = Mathf.Clamp(value, 0, MaxStamina);
                onStaminaChange?.Invoke(stamina / MaxStamina);
            }
        }
    }
    /// <summary>
    /// 최대 스테미나
    /// </summary>
    float maxStamina = 100.0f;
    public float MaxStamina => maxStamina;

    /// <summary>
    /// 스테미나 감소하는 코루틴
    /// </summary>
    /// <returns></returns>
    IEnumerator sprintDerease()
    {
        while (true)
        {
            Stamina -= 20.0f * Time.deltaTime;
            if (Stamina <= 0)
            {
                controller.OffSprinting();
            }
            yield return null;
        }
    }

    private void StaminaChangeCoroutine(bool isSprint)
    {
        if (isSprint)
        {
            StopCoroutine(sprintCorutine);
            sprintCorutine = sprintDerease();
            StartCoroutine(sprintCorutine);
        }
        else
        {
            StopCoroutine(sprintCorutine);
            sprintCorutine = StaminaRegetateCorutine();
            StartCoroutine(sprintCorutine);
        }
    }

    /// <summary>
    /// 스테미나 자동 회복 코루틴(정지 0.75초 후)
    /// </summary>
    /// <returns></returns>
    IEnumerator StaminaRegetateCorutine()
    {
        if (!controller.isStamina)
        {
            controller.isStamina = true;
            // 코루틴 실행 후 0.5초 대기
            yield return new WaitForSeconds(0.75f);

        }
        // 회복량 = 전체 스테미나의 1/10
        float regenpersec = MaxStamina / 10;
        float timeElapsed = 0.0f;
        while (timeElapsed < 10)
        {
            timeElapsed += Time.deltaTime;
            Stamina += Time.deltaTime * regenpersec;

            yield return null;
        }
    }
}
