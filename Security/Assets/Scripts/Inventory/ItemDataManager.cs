using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemCode
{
    None,

}

public enum ItemSortyBy
{
    Code,   // �ڵ带 �������� ����
    Name,   // ������ �̸��� �������� ����

}

public class ItemDataManager : MonoBehaviour
{
    /// <summary>
    /// ��� ������ ������ ���� �迭
    /// </summary>
    public ItemData[] itemDatas = null;

    /// <summary>
    /// ������ ������ ������ ���� �ε���
    /// </summary>
    /// <param name="code">������ �������� �ڵ�</param>
    /// <returns></returns>
    public ItemData this[ItemCode code] => itemDatas[(int)code];

    /// <summary>
    /// ������ ������ ������ ���� �ε���(�׽�Ʈ��)
    /// </summary>
    /// <param name="index">������ �������� �ε���</param>
    /// <returns></returns>
    public ItemData this[int index] => itemDatas[index];

    /// <summary>
    /// ���� �����ϴ� ������ ������ ��� ����
    /// </summary>
    public int length => itemDatas.Length;
}
