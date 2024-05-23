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
        data1.Set(0, "방화광", $"매턴 모든 적에게 '기본 마법력'({GameMgr.Instance.curruntBonusStat}) 만큼의 데미지");
        artifacts.Add(data1);

        ArtifactData data2 = new ArtifactData();
        data2.Set(1, "모 아니면 도", "주사위 눈금에서 3,4,5 를 제거");
        artifacts.Add(data2);

        ArtifactData data3 = new ArtifactData();
        data3.Set(2, "스트레이트 매니아", "첫 주사위 굴림에 1,2,3을 반드시 가져감");
        artifacts.Add(data3);

        ArtifactData data4 = new ArtifactData();
        data4.Set(3, "족보 수집", $"마법서 보상 선택을 {Value3}회 실행");
        artifacts.Add(data4);

        ArtifactData data5 = new ArtifactData();
        data5.Set(4, "기본기 수련", $"주사위가 3개 이하라면 최종 공격력을 {Value4}% 만큼 증폭");
        artifacts.Add(data5);

        ArtifactData data6 = new ArtifactData();
        data6.Set(5, "회피", $"{Value5}%의 확률로 적의 공격을 무시함");
        artifacts.Add(data6);

        ArtifactData data7 = new ArtifactData();
        data7.Set(6, "오히려 좋아", $"주사위 눈이 1인 경우 개당 {Value6} 만큼 공격력 증가");
        artifacts.Add(data7);

        ArtifactData data8 = new ArtifactData();
        data8.Set(7, "원코인 추가", $"사망에 이르는 피해를 입을 시 사망을 1회 무효로 하고 {Value7} 의 체력을 회복");
        artifacts.Add(data8);

        ArtifactData data9 = new ArtifactData();
        data9.Set(8, "재재굴림", $"재굴림 횟수를 {Value8}회 추가");
        artifacts.Add(data9);

        ArtifactData data10 = new ArtifactData();
        data10.Set(9, "구사일생", $"최대 체력 {Value9} 증가, 체력이 전부 회복됨");
        artifacts.Add(data10);
    }
}
