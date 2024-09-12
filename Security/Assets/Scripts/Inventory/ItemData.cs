using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item Data", menuName = "Scriptable Object/Item Data", order = 1)]
public class ItemData : ScriptableObject
{
    [Header("아이템 기본 데이터")]
    //아이템 코드
    public ItemCode code;

    //아이템 이름
    public string itemName = "";

    //아이템 오브젝트
    public GameObject modelPregab;

    //아이템 아이콘
    public Sprite itemIcon;

    //아이템 소지 최대 갯수
    public uint maxStackCount = 1;

    //아이템 설명
    public string itemDescription = "";
}
