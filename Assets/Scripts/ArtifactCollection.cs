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
        data1.Set(0, "방화광", $"매턴 모든 적에게 '기본 공격력'(<color=purple>{mediator.gameMgr.curruntBonusStat}</color>) 만큼의 데미지");
        artifactList.Add(data1);

        ArtifactData data2 = new ArtifactData();
        data2.Set(1, "모 아니면 도", "주사위 눈금에서 3,4,5 를 제거");
        artifactList.Add(data2);

        ArtifactData data3 = new ArtifactData();
        data3.Set(2, "스트레이트 매니아", "첫 주사위 굴림에 1,2,3을 반드시 가져감");
        artifactList.Add(data3);

        ArtifactData data4 = new ArtifactData();
        data4.Set(3, "족보 수집", $"마법서 보상 선택을 {valueData.Value3}회 실행");
        artifactList.Add(data4);

        ArtifactData data5 = new ArtifactData();
        data5.Set(4, "기본기 수련", $"주사위가 3개 이하라면 최종 공격력을 {valueData.Value4}% 만큼 증폭");
        artifactList.Add(data5);

        ArtifactData data6 = new ArtifactData();
        data6.Set(5, "회피", $"{valueData.Value5}%의 확률로 적의 공격을 무시함");
        artifactList.Add(data6);

        ArtifactData data7 = new ArtifactData();
        data7.Set(6, "오히려 좋아", $"주사위 눈이 1인 경우 개당 {valueData.Value6} 만큼 공격력 증가");
        artifactList.Add(data7);

        ArtifactData data8 = new ArtifactData();
        data8.Set(7, "원코인 추가", $"사망에 이르는 피해를 입을 시 사망을 1회 무효로 하고 <color=green>{valueData.Value7}</color> 의 체력을 회복");
        artifactList.Add(data8);

        ArtifactData data9 = new ArtifactData();
        data9.Set(8, "재재굴림", $"재굴림 횟수를 {valueData.Value8}회 추가");
        artifactList.Add(data9);

        ArtifactData data10 = new ArtifactData();
        data10.Set(9, "구사일생", $"최대 체력 <color=green>{valueData.Value9}</color> 증가, 체력이 전부 회복됨");
        artifactList.Add(data10);


        for(int i = 0; i < 3;  i++)
        {
            playersArtifactsNumber[i] = -1;
        }
    }

}
