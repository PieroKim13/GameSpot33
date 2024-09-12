using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item Data", menuName = "Scriptable Object/Item Data", order = 1)]
public class ItemData : ScriptableObject
{
    [Header("������ �⺻ ������")]
    //������ �ڵ�
    public ItemCode code;

    //������ �̸�
    public string itemName = "";

    //������ ������Ʈ
    public GameObject modelPregab;

    //������ ������
    public Sprite itemIcon;

    //������ ���� �ִ� ����
    public uint maxStackCount = 1;

    //������ ����
    public string itemDescription = "";
}
