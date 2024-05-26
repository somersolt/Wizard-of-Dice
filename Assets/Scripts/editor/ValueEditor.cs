using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ValueData))]
public class ValueEditor : Editor
{
    SerializedProperty value3Prop;
    SerializedProperty value4Prop;
    SerializedProperty value5Prop;
    SerializedProperty value6Prop;
    SerializedProperty value7Prop;
    SerializedProperty value8Prop;
    SerializedProperty value9Prop;
    SerializedProperty Stat1Prop;
    SerializedProperty Stat2Prop;

    void OnEnable()
    {
        // SerializedProperty 초기화
        value3Prop = serializedObject.FindProperty("Value3");
        value4Prop = serializedObject.FindProperty("Value4");
        value5Prop = serializedObject.FindProperty("Value5");
        value6Prop = serializedObject.FindProperty("Value6");
        value7Prop = serializedObject.FindProperty("Value7");
        value8Prop = serializedObject.FindProperty("Value8");
        value9Prop = serializedObject.FindProperty("Value9");
        Stat1Prop = serializedObject.FindProperty("Stat1");
        Stat2Prop = serializedObject.FindProperty("Stat2");
    }
    public override void OnInspectorGUI()
    {
        // 설명 텍스트
        EditorGUILayout.HelpBox("이것은 유물, 스탯 실적용 값을 조정하는 스크립트입니다.", MessageType.Info);

        serializedObject.Update();


        // 각 필드에 대한 설명을 개별적으로 렌더링
        EditorGUILayout.BeginVertical();

        EditorGUILayout.PropertyField(value3Prop, new GUIContent("족보 수집"));
        DrawDescriptionLabel("마법서 보상 선택을 n회 실행");

        EditorGUILayout.PropertyField(value4Prop, new GUIContent("기본기 수련"));
        DrawDescriptionLabel("주사위가 3개 이하라면 최종 공격력을 n% 만큼 증폭");

        EditorGUILayout.PropertyField(value5Prop, new GUIContent("회피"));
        DrawDescriptionLabel("n% 의 확률로 적의 공격을 무시함.");

        EditorGUILayout.PropertyField(value6Prop, new GUIContent("오히려 좋아"));
        DrawDescriptionLabel("주사위 눈이 1인 경우 개당 n 만큼 공격력 증가");

        EditorGUILayout.PropertyField(value7Prop, new GUIContent("원코인 추가"));
        DrawDescriptionLabel("사망에 이르는 피해를 입을 시 사망을 1회 무효로 하고 n 의 체력을 회복");

        EditorGUILayout.PropertyField(value8Prop, new GUIContent("재재굴림"));
        DrawDescriptionLabel("재굴림 횟수를 n 회 추가");

        EditorGUILayout.PropertyField(value9Prop, new GUIContent("구사일생"));
        DrawDescriptionLabel("최대 체력 n 증가, 체력이 전부 회복됨");

        EditorGUILayout.PropertyField(Stat1Prop, new GUIContent("마법서 스탯 보상"));
        DrawDescriptionLabel("공격력 n 증가");

        EditorGUILayout.PropertyField(Stat2Prop, new GUIContent("주사위 스탯 보상"));
        DrawDescriptionLabel("공격력 n 증가");
        EditorGUILayout.EndVertical();


        // 변경 사항 적용
        serializedObject.ApplyModifiedProperties();
    }

    void DrawDescriptionLabel(string description)
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(EditorGUIUtility.labelWidth);
        EditorGUILayout.LabelField(description);
        EditorGUILayout.EndHorizontal();
    }
}
