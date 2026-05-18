namespace LineControlCenter.UI.Services;

public class RefreshStateService
{
    public double Progress { get; private set; } = 100;
    public int SecondsRemaining { get; private set; } = 120;
    public bool IsDashboardActive { get; private set; }
    public string CurrentTime { get; private set; } = "";
    public string CurrentDate { get; private set; } = "";

    public event Action? OnChange;
    public event Func<Task>? OnReconfigure;
    public event Func<Task>? OnRefreshRequested;
    public event Action<int>? OnDpmTargetChanged;

    public int DpmTarget { get; private set; } = 800;

    public double SomsZoom { get; private set; } = 0.9;
    public double RiskZoom { get; private set; } = 0.9;

    public void Update(double progress, int secondsRemaining, bool isActive, string time, string date)
    {
        Progress = progress;
        SecondsRemaining = secondsRemaining;
        IsDashboardActive = isActive;
        CurrentTime = time;
        CurrentDate = date;
        OnChange?.Invoke();
    }

    public void SetDashboardActive(bool isActive)
    {
        IsDashboardActive = isActive;
        OnChange?.Invoke();
    }

    public async Task RequestReconfigure()
    {
        if (OnReconfigure != null)
            await OnReconfigure.Invoke();
    }

    public async Task RequestRefresh()
    {
        if (OnRefreshRequested != null)
            await OnRefreshRequested.Invoke();
    }

    public void SetDpmTarget(int target)
    {
        DpmTarget = target;
        OnDpmTargetChanged?.Invoke(target);
        OnChange?.Invoke();
    }

    public void SetSomsZoom(double zoom)
    {
        SomsZoom = Math.Clamp(zoom, 0.3, 2.0);
        OnChange?.Invoke();
    }

    public void SetRiskZoom(double zoom)
    {
        RiskZoom = Math.Clamp(zoom, 0.3, 2.0);
        OnChange?.Invoke();
    }
}
