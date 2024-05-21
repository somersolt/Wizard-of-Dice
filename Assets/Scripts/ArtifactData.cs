using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtifactData
{

    //public static readonly string FormatIconPath = "Icon/{0}";
    //아이콘 같은거 있으면 포함할 주소 (Resources 폴더 내 포함)

    public int ID { get; set; }
    public string NAME { get; set; }
    public string DESC { get; set; }
    public bool ONARTIFACT { get; set; }

    //public string Icon { get; set; } 아이콘 path


    //public Sprite GetSprite
    //{
    //    get
    //    {
    //        return Resources.Load<Sprite>(string.Format(FormatIconPath, Icon));
    //    }
    //} icon 주소 반환 프로퍼티

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

