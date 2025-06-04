namespace CommonMonster.States
{
    public interface IMonsterState
    {
        // ���� ���� �� �� �� ȣ��Ǵ� �޼���
        void Enter();

        // ���°� Ȱ��ȭ�Ǿ� �ִ� ���� �� ������(�Ǵ� FixedUpdate) ȣ��Ǵ� �޼���
        void Execute();

        // ���� ���� �� �� �� ȣ��Ǵ� �޼���
        void Exit();
    }
}