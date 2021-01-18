using System.Collections.Generic;

namespace DataLayer
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public virtual ICollection<UserTask> UserTasks { get; set; }
    }
}