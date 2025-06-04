using CommonMonster.Controller; // CommonMonsterController ����

namespace CommonMonster.States
{
    public abstract class BaseMonsterState : IMonsterState
    {
        // ��� ���� ���°� ������ �� �ִ� ���� ��Ʈ�ѷ� �ν��Ͻ�
        protected CommonMonsterController controller;

        // ������: ���°� ������ �� ��Ʈ�ѷ� �ν��Ͻ��� �޾Ƽ� ����
        public BaseMonsterState(CommonMonsterController controller)
        {
            this.controller = controller;
        }

        // IMonsterState �������̽��� �޼������ �߻����� ����
        // �� Ŭ������ ��ӹ޴� ��� ��ü���� ���� Ŭ�������� �� �޼������ �ݵ�� �����ؾ� ��
        public abstract void Enter();
        public abstract void Execute();
        public abstract void Exit();
    }
}