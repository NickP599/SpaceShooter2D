public class Timer
{
    private float m_Timer;
    private float m_CurrentTime;

    public bool IsFinished => m_CurrentTime <= 0;

    public Timer(float startTime)
    {
        SetStartTime(startTime);     
    }

    public void SetStartTime(float startTime)
    {
        m_Timer = startTime;
        m_CurrentTime = m_Timer;
    }

    public void RemoveTime(float deltaTime)
    { 
        if(m_CurrentTime <= 0) return;

        m_CurrentTime -= deltaTime;
    }

    public void RestartTimer()
    {
        m_CurrentTime = m_Timer;
    }
}
