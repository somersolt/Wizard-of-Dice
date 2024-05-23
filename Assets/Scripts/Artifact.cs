using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Artifact : MonoBehaviour
{
    public List<ArtifactData> artifacts = new List<ArtifactData>();
    public int[] playersArtifacts = new int[10];
    public int[] playersArtifactsNumber = new int[3];


    public int Value3 = 2;
    public int Value4 = 100;
    public int Value5 = 20;
    public int Value6 = 10;
    public int Value7 = 50;
    public int Value8 = 2;
    public int Value9 = 80;
    private void Awake()
    {
        ArtifactData data1 = new ArtifactData();
        data1.Set(0, "��ȭ��", $"���� ��� ������ '�⺻ ������'({GameMgr.Instance.curruntBonusStat}) ��ŭ�� ������");
        artifacts.Add(data1);

        ArtifactData data2 = new ArtifactData();
        data2.Set(1, "�� �ƴϸ� ��", "�ֻ��� ���ݿ��� 3,4,5 �� ����");
        artifacts.Add(data2);

        ArtifactData data3 = new ArtifactData();
        data3.Set(2, "��Ʈ����Ʈ �ŴϾ�", "ù �ֻ��� ������ 1,2,3�� �ݵ�� ������");
        artifacts.Add(data3);

        ArtifactData data4 = new ArtifactData();
        data4.Set(3, "���� ����", $"������ ���� ������ {Value3}ȸ ����");
        artifacts.Add(data4);

        ArtifactData data5 = new ArtifactData();
        data5.Set(4, "�⺻�� ����", $"�ֻ����� 3�� ���϶�� ���� ���ݷ��� {Value4}% ��ŭ ����");
        artifacts.Add(data5);

        ArtifactData data6 = new ArtifactData();
        data6.Set(5, "ȸ��", $"{Value5}%�� Ȯ���� ���� ������ ������");
        artifacts.Add(data6);

        ArtifactData data7 = new ArtifactData();
        data7.Set(6, "������ ����", $"�ֻ��� ���� 1�� ��� ���� {Value6} ��ŭ ���ݷ� ����");
        artifacts.Add(data7);

        ArtifactData data8 = new ArtifactData();
        data8.Set(7, "������ �߰�", $"����� �̸��� ���ظ� ���� �� ����� 1ȸ ��ȿ�� �ϰ� {Value7} �� ü���� ȸ��");
        artifacts.Add(data8);

        ArtifactData data9 = new ArtifactData();
        data9.Set(8, "���籼��", $"�籼�� Ƚ���� {Value8}ȸ �߰�");
        artifacts.Add(data9);

        ArtifactData data10 = new ArtifactData();
        data10.Set(9, "�����ϻ�", $"�ִ� ü�� {Value9} ����, ü���� ���� ȸ����");
        artifacts.Add(data10);
    }
}
