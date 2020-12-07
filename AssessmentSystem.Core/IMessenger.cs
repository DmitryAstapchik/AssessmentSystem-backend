namespace AssessmentSystem.Core
{
    public interface IMessenger
    {
        void SendMessage(string message); //отправить сообщение
        string GetLastMessage(); //получить последнее сообщение
        string GetFirstMessage(); //получить первое сообщение
    }
}