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
        // SerializedProperty �ʱ�ȭ
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
        // ���� �ؽ�Ʈ
        EditorGUILayout.HelpBox("�̰��� ����, ���� ������ ���� �����ϴ� ��ũ��Ʈ�Դϴ�.", MessageType.Info);

        serializedObject.Update();


        // �� �ʵ忡 ���� ������ ���������� ������
        EditorGUILayout.BeginVertical();

        EditorGUILayout.PropertyField(value3Prop, new GUIContent("���� ����"));
        DrawDescriptionLabel("������ ���� ������ nȸ ����");

        EditorGUILayout.PropertyField(value4Prop, new GUIContent("�⺻�� ����"));
        DrawDescriptionLabel("�ֻ����� 3�� ���϶�� ���� ���ݷ��� n% ��ŭ ����");

        EditorGUILayout.PropertyField(value5Prop, new GUIContent("ȸ��"));
        DrawDescriptionLabel("n% �� Ȯ���� ���� ������ ������.");

        EditorGUILayout.PropertyField(value6Prop, new GUIContent("������ ����"));
        DrawDescriptionLabel("�ֻ��� ���� 1�� ��� ���� n ��ŭ ���ݷ� ����");

        EditorGUILayout.PropertyField(value7Prop, new GUIContent("������ �߰�"));
        DrawDescriptionLabel("����� �̸��� ���ظ� ���� �� ����� 1ȸ ��ȿ�� �ϰ� n �� ü���� ȸ��");

        EditorGUILayout.PropertyField(value8Prop, new GUIContent("���籼��"));
        DrawDescriptionLabel("�籼�� Ƚ���� n ȸ �߰�");

        EditorGUILayout.PropertyField(value9Prop, new GUIContent("�����ϻ�"));
        DrawDescriptionLabel("�ִ� ü�� n ����, ü���� ���� ȸ����");

        EditorGUILayout.PropertyField(Stat1Prop, new GUIContent("������ ���� ����"));
        DrawDescriptionLabel("���ݷ� n ����");

        EditorGUILayout.PropertyField(Stat2Prop, new GUIContent("�ֻ��� ���� ����"));
        DrawDescriptionLabel("���ݷ� n ����");
        EditorGUILayout.EndVertical();


        // ���� ���� ����
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
