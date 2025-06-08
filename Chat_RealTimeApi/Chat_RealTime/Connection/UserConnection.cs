
namespace Chat_RealTime.Connection
{
    public class UserConnection : IUserConnection
    {
        private readonly Dictionary<string, List<string>> _userConnection = new();
        private readonly object _lock = new ();

        public void AddUser(string userId, string connectionId)
        {
            lock (_lock) {
                if (!_userConnection.ContainsKey(userId)) {
                    _userConnection[userId] = new List<string>();
                        };
                 _userConnection[userId].Add(connectionId);
            }
        }

        public List<string> GetConnectionUser(string userId)
        {
            lock (_lock) {
               return _userConnection.ContainsKey(userId) ? _userConnection[userId] : new List<string>();   
            }
        }

        public void RemoveUser(string connectionId)
        {
            lock (_lock)
            {
                foreach (var user in _userConnection.Keys.ToList())
                {
                    _userConnection[user].Remove(connectionId);
                    if (_userConnection[user].Count == 0) { _userConnection.Remove(user); }
                    
                }
            }
        }
    }
}
