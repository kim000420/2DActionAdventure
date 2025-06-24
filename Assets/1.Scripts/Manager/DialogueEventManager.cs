using UnityEngine;

/// <summary>
/// 대화 내 선택지 트리거 및 연출 이벤트 전용 매니저
/// 게임 전역 상태는 GameEventManager에서 관리
/// </summary>
public class DialogueEventManager : MonoBehaviour
{
    public static DialogueEventManager Instance;

    [SerializeField]
    private string currentTalkContext = "Default"; // ✅ 대화 컨텍스트 상태 (조건 분기에 사용)

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // ✅ 현재 대화 컨텍스트 조회
    public string GetCurrentTalkContext()
    {
        return currentTalkContext;
    }

    // ✅ 대화 컨텍스트 설정
    public void SetTalkContext(string contextKey)
    {
        currentTalkContext = contextKey;
    }

    // ✅ 트리거 실행
    public void Trigger(string eventId)
    {
        Debug.Log($"[DialogueEvent] Triggered: {eventId}");
        
        switch (eventId)
        {
            // eventid가 호출되면 이벤트 시작
            // 씬 전환시 또는 대화 종료시 호출됨

            // 씬 전환시 혼잣말
            case "MS_intro":
                // StartTriggerObl 오브젝트를 찾음
                var target = GameObject.Find("StartTriggerObj");

                //타겟의 대화 트리거 얻고 대화 실행
                var trigger = target.GetComponent<DialogueTrigger>();
                trigger?.TryStartDialogue();
                break;

            // 엄마와 대화 후 - 잘다녀와
            case "MS_001":
                // StoryStage 변수명 변경
                GameEventManager.Instance.SetCurrentStoryStage("ST_001");
                break;



            // 숲에서 무슨 소리가 나는데?   
            case "MS_001_ATZ":

                break;

            // 수상한 외부인 조우 - 주거랏!!. 수상한외부인과 전투  
            case "MS_002":
                GameEventManager.Instance.SetCurrentStoryStage("ST_002");
                break;

            // 수상한 외부인 사망
            case "MS_002_MDT":
                GameEventManager.Instance.SetCurrentStoryStage("ST_002");
                break;

            // 시계 겟도다제
            case "MS_002_ATZ":
                GameEventManager.Instance.SetCurrentStoryStage("ST_002_1");
                break;



            // 교관 대화 이후 - 출첵해.  
            case "MS_003":
                GameEventManager.Instance.SetCurrentStoryStage("ST_003");
                break;



            // 안내맨 대화 이후 - 대련장으로 가라.  
            case "MS_004":
                GameEventManager.Instance.SetCurrentStoryStage("ST_004");
                //대련장 텔포
                break;

            // 씬전환
            case "MS_005_SET":
                //대련장 대화 불러오기
                GameEventManager.Instance.SetCurrentStoryStage("ST_005");
                break;

            // 죠 활성화 이벤트
            case "MS_005":
                //죠 활성화
                break;

            // 죠 사망
            case "MS_006_MDT":
                //7조 승자 조준용
                break;

            // 죠 이김  -   7조의 승자는 조준용
            case "MS_006":
                GameEventManager.Instance.SetCurrentStoryStage("ST_006");
                // 길드로 씬이동
                break;




            // 교관 대화 이후 - 다음 시험장으로 가라.  
            case "MS_007":
                GameEventManager.Instance.SetCurrentStoryStage("ST_007");
                break;




            // 쿨눈나와 대화 - 시험장으로 진짜 ㄱㄱ.  
            case "MS_008":
                GameEventManager.Instance.SetCurrentStoryStage("ST_008");
                //텔포
                break;

            // 씬 전환 대화 
            case "MS_009_SET":
                GameEventManager.Instance.SetCurrentStoryStage("ST_008");
                //텔포
                break;

            // 보우 활성화
            case "MS_009":
                GameEventManager.Instance.SetCurrentStoryStage("ST_009");
                break;

            // 보우 사망 
            case "MS_010_MDT":
                //승자는 조준용
                break;

            // 대련 승리 이후 - 4조 승자는 조준용.  
            case "MS_010":
                GameEventManager.Instance.SetCurrentStoryStage("ST_010");
                //텔포
                break;




            // 교관 대화 이후 - 대련장으로 ㄱㄱ.  
            case "MS_011":
                GameEventManager.Instance.SetCurrentStoryStage("ST_011");
                break;




            // 안내맨 대화 이후 - 대련장 입장해라.  
            case "MS_012":
                GameEventManager.Instance.SetCurrentStoryStage("ST_012");
                //텔포
                break;

            // 씬전환
            case "MS_013_SET":
                GameEventManager.Instance.SetCurrentStoryStage("ST_013");
                break;

            // Dok2활성.  
            case "MS_013":
                GameEventManager.Instance.SetCurrentStoryStage("ST_013");
                break;

            // Dok2 사망
            case "MS_014_MDT":
                // ㅊㅊ
                break;

            // 대련 승리! 길마 ㅊㅊ 이후 - 집가서 쉬어.  
            case "MS_014":
                GameEventManager.Instance.SetCurrentStoryStage("ST_014");
                // 길드로 텔포
                break;




            // 교관 대화 이후 - 집가서 쉬어.  
            case "MS_015":
                GameEventManager.Instance.SetCurrentStoryStage("ST_015");
                break;



            // 풀숲에 뭔가가 있는데? 
            case "MS_016_ATZ":
                GameEventManager.Instance.SetCurrentStoryStage("ST_016");
                break;

            // 수상한 외부인 이후 - 시계에 대해서 발설 ㄴㄴ.  
            case "MS_016":
                GameEventManager.Instance.SetCurrentStoryStage("ST_016");
                break;




            // 엄마 대화 이후 - 잘자렴~ 
            case "MS_017":
                GameEventManager.Instance.SetCurrentStoryStage("ST_017");
                break;

            // 얼리버드 기상 엄마 대화 이후 - 계란말이 머겅.  
            case "MS_018":
                GameEventManager.Instance.SetCurrentStoryStage("ST_018");
                break;

            // 수상한 외부인 대화 이후 - 몸조심해~.  
            case "MS_019":
                GameEventManager.Instance.SetCurrentStoryStage("ST_019");
                break;
            // 교관 대화 이후 - 출첵해.  
            case "MS_020":
                break;
            // 교관 대화 이후 - 출첵해.  
            case "MS_021":
                break;

            // 교관 대화 이후 - 출첵해.  
            case "MS_022":
                break;


        }
    }
}
