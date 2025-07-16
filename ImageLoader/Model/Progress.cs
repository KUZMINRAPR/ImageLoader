using System.Diagnostics;

namespace ImageLoader.Model;

public static class Progress
{
    public static double ActiveCount {get; set;}
    public static double CompletedCount {get; set;}
    public static double GetProgress(ProgressState[] states)
    {
        ActiveCount = states.Where(x => x != ProgressState.NotStarted).Count();
        CompletedCount = states.Where(x => x == ProgressState.Completed).Count();
        
        if (ActiveCount == 0)
            return 0;
        
        Console.WriteLine($"ActiveCount: {ActiveCount}, CompletedCount: {CompletedCount}");
        
        return (double)CompletedCount / ActiveCount * 100;
    }
}
