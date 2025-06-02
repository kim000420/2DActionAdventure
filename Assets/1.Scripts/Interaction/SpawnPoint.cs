using UnityEngine;
public enum SpawnType
{
    Start,      // ���� ���� �� ���� ����
    Return,     // ���/��ȯ �� ����
    Portal,     // ��Ż/�� �̵� �� ����
    Event,      // �ƾ�/�̺�Ʈ ����
    Dungeon     // ���� ���� Ư�� ���� ����
}

[DisallowMultipleComponent]
public class SpawnPoint : MonoBehaviour
{
    [Tooltip("�� ��ȯ �� �����Ǵ� ���� �̸��Դϴ�.")]
    public string spawnID;

    [Tooltip("�� ��������Ʈ�� Ÿ���Դϴ�.")]
    public SpawnType spawnType;
}
