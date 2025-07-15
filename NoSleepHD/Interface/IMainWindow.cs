namespace NoSleepHD.Interface
{
    public interface IMainWindow
    {
        void Minimize();
        void ChangeState(bool show);
        void ShowNotifyMsg();
    }
}
