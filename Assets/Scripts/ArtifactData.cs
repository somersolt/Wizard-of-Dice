using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtifactData
{

    //public static readonly string FormatIconPath = "Icon/{0}";
    //������ ������ ������ ������ �ּ� (Resources ���� �� ����)

    public int ID { get; set; }
    public string NAME { get; set; }
    public string DESC { get; set; }
    public bool ONARTIFACT { get; set; }

    //public string Icon { get; set; } ������ path


    //public Sprite GetSprite
    //{
    //    get
    //    {
    //        return Resources.Load<Sprite>(string.Format(FormatIconPath, Icon));
    //    }
    //} icon �ּ� ��ȯ ������Ƽ

    public void Set(int i, string n, string d)
    {
        ID = i;
        NAME = n;
        DESC = d;
    }

    public override string ToString()
    {
        return $"{ID}: / {NAME} / {DESC} ";
    }

}

