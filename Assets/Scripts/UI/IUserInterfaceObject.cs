namespace Sunshower.UI
{
    /// <summary>
    /// 모든 ui는 ui object를 상속받습니다.
    /// </summary>
    public interface IUserInterfaceObject
    {
        public void Enter();

        public void Exit();
    }
}
