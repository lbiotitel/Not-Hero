public interface IEventListener
{
    public void OnEvent(EEventType eventType, object sender, object param = null);
}
