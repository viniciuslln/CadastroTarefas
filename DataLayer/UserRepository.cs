using LiteDB;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer
{
    public class UserRepository
    {
        private readonly LiteDatabase _db;

        public UserRepository(LiteDatabase db)
        {
            _db = db;
        }

        public Task<User> GetRegisteredUser(string username, string plainPassword)
        {
            var encripitedPassword = SHA1.Create().ComputeHash(Encoding.Default.GetBytes(plainPassword));
            return Task.Run(() => _db.GetCollection<User>().FindOne(u => u.Username == username && u.Password == Encoding.Default.GetString(encripitedPassword)));
        }

        public async Task<User> SearchRegisteredUser(string username)
        {
            return await Task.Run(() => _db.GetCollection<User>().FindOne(u => u.Username == username));
        }

        public async Task<User> SaveNewUser(string username, string plainPassword)
        {
            var encripitedPassword = System.Security.Cryptography.SHA1.Create().ComputeHash(Encoding.Default.GetBytes(plainPassword));
            var user = new User { Username = username, Password = Encoding.Default.GetString(encripitedPassword) };
            user.Id = await Task.Run(() => _db.GetCollection<User>().Insert(user));

            return user;
        }


    }
}
