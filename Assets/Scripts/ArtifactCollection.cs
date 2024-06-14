using System.Collections.Generic;
using UnityEngine;

public class ArtifactCollection : MonoBehaviour
{
    private Mediator mediator;

    public ValueData valueData;
    public List<ArtifactData> artifactList = new List<ArtifactData>();
    public int[] playersArtifactsLevel = new int[10];
    public int[] playersArtifactsNumber = new int[3];

    private void Awake()
    {
        mediator = FindObjectOfType<Mediator>();

        ArtifactData data1 = new ArtifactData();
        data1.Set(0, "��ȭ��", $"���� ��� ������ '�⺻ ���ݷ�'(<color=purple>{mediator.gameMgr.curruntBonusStat}</color>) ��ŭ�� ������");
        artifactList.Add(data1);

        ArtifactData data2 = new ArtifactData();
        data2.Set(1, "�� �ƴϸ� ��", "�ֻ��� ���ݿ��� 3,4,5 �� ����");
        artifactList.Add(data2);

        ArtifactData data3 = new ArtifactData();
        data3.Set(2, "��Ʈ����Ʈ �ŴϾ�", "ù �ֻ��� ������ 1,2,3�� �ݵ�� ������");
        artifactList.Add(data3);

        ArtifactData data4 = new ArtifactData();
        data4.Set(3, "���� ����", $"������ ���� ������ {valueData.Value3}ȸ ����");
        artifactList.Add(data4);

        ArtifactData data5 = new ArtifactData();
        data5.Set(4, "�⺻�� ����", $"�ֻ����� 3�� ���϶�� ���� ���ݷ��� {valueData.Value4}% ��ŭ ����");
        artifactList.Add(data5);

        ArtifactData data6 = new ArtifactData();
        data6.Set(5, "ȸ��", $"{valueData.Value5}%�� Ȯ���� ���� ������ ������");
        artifactList.Add(data6);

        ArtifactData data7 = new ArtifactData();
        data7.Set(6, "������ ����", $"�ֻ��� ���� 1�� ��� ���� {valueData.Value6} ��ŭ ���ݷ� ����");
        artifactList.Add(data7);

        ArtifactData data8 = new ArtifactData();
        data8.Set(7, "������ �߰�", $"����� �̸��� ���ظ� ���� �� ����� 1ȸ ��ȿ�� �ϰ� <color=green>{valueData.Value7}</color> �� ü���� ȸ��");
        artifactList.Add(data8);

        ArtifactData data9 = new ArtifactData();
        data9.Set(8, "���籼��", $"�籼�� Ƚ���� {valueData.Value8}ȸ �߰�");
        artifactList.Add(data9);

        ArtifactData data10 = new ArtifactData();
        data10.Set(9, "�����ϻ�", $"�ִ� ü�� <color=green>{valueData.Value9}</color> ����, ü���� ���� ȸ����");
        artifactList.Add(data10);


        for(int i = 0; i < 3;  i++)
        {
            playersArtifactsNumber[i] = -1;
        }
    }

}
