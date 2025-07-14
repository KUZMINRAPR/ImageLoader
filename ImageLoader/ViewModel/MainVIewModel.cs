namespace ImageLoader.ViewModel;

public class MainViewModel
{
    public StopViewModel StopViewModel { get; set; } = new();
    public StartViewModel StartViewModel { get; }

    public MainViewModel()
    {
        StartViewModel = new StartViewModel(StopViewModel);
    }
}