using UnityEngine;
public enum SpawnType
{
    Start,      // 게임 진입 시 최초 스폰
    Return,     // 사망/귀환 시 스폰
    Portal,     // 포탈/맵 이동 시 스폰
    Event,      // 컷씬/이벤트 전용
    Dungeon     // 던전 내부 특정 구역 스폰
}

[DisallowMultipleComponent]
public class SpawnPoint : MonoBehaviour
{
    [Tooltip("씬 전환 시 참조되는 고유 이름입니다.")]
    public string spawnID;

    [Tooltip("이 스폰포인트의 타입입니다.")]
    public SpawnType spawnType;
}
