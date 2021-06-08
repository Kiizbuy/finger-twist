namespace GameFramework.UI
{
    public interface IPresenter<in T0>
    {
        void Present(T0 value);
    }

    public interface IPresenter<in T0, in T1>
    {
        void Present(T0 value, T1 value2);
    }

    public interface IPresenter<in T0, in T1, in T2>
    {
        void Present(T0 value, T1 value2, T2 value3);
    }
}
