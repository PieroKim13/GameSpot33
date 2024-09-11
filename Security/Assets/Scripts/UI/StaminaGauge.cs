using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaminaGauge : GaugeBase
{
    private void Start()
    {
        // 게임 매니저에서 싱글톤 받아오기
        Player player = GameManager.Inst.Player;

        maxValue = player.MaxStamina;               // 최대값은 플레이어의 최대 스테미나
        slider.value = player.Stamina / maxValue;   // 게이지바 값 = 플레이어의 현재 스테미나 / 최대값
        player.onStaminaChange += OnValueChange;

    }
}
