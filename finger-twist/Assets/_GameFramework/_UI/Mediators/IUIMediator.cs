namespace GameFramework.UI
{
    public interface IUIMediator<TViewModel> where TViewModel : IViewModel
    {
        void Setup(TViewModel viewModel);
    }
    
    public interface IUIMediator<TViewModel1, TViewModel2> where TViewModel1 : IViewModel where TViewModel2 : IViewModel
    {
        void Setup(TViewModel1 viewModel1, TViewModel2 viewModel2);
    }
    
    public interface IUIMediator<TViewModel1, TViewModel2, TViewModel3> where TViewModel1 : IViewModel where TViewModel2 : IViewModel where  TViewModel3 : IViewModel
    {
        void Setup(TViewModel1 viewModel1, TViewModel2 viewModel2, TViewModel3 viewModel3);
    }

    public interface IScreenUIMediator
    {
        void Start();
        void Stop();
    }
}