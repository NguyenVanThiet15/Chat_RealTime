namespace Chat_RealTime.Connection
{
    public interface IUserConnection
    {
        void AddUser(string userId, string connectionId);
        void RemoveUser(string connectionId);
        List<string> GetConnectionUser(string userId);
    }
}
