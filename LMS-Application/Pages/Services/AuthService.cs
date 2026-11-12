using SQLite;

namespace LMS_Application;

public class AuthService
{
    private readonly SQLiteConnection _db;
    public AuthService(DatabaseService dbService) => _db = dbService.Connection;

    public User? CurrentUser { get; private set; }
    public bool IsAdmin => CurrentUser?.IsAdmin == true;

    public Task InitAsync()
    {
        _db.CreateTable<User>();   // table only, no default account
        return Task.CompletedTask;
    }

    public Task<string?> SignUpAsync(string username, string password, bool isAdmin)
    {
        username = username.Trim();
        if (username == "" || string.IsNullOrWhiteSpace(password))
            return Task.FromResult<string?>("Username and password are required.");
        if (_db.Table<User>().Where(u => u.Username == username).Count() > 0)
            return Task.FromResult<string?>("Username already exists.");

        _db.Insert(new User
        {
            Username = username,
            Password = PasswordHasher.Hash(password),
            IsAdmin = isAdmin
        });
        return Task.FromResult<string?>(null);
    }
    public Task<bool> LoginAsync(string username, string password)
    {
        var user = _db.Table<User>().FirstOrDefault(u => u.Username == username);
        if (user == null || !PasswordHasher.Verify(password, user.Password))
            return Task.FromResult(false);

        CurrentUser = user;
        return Task.FromResult(true);
    }
    public void Logout() => CurrentUser = null;

    // ---- admin only ----
    public Task<List<User>> GetUsersAsync() =>
        Task.FromResult(IsAdmin ? _db.Table<User>().ToList() : new List<User>());

    public Task<string?> AddUserAsync(string username, string password, bool isAdmin)
    {
        if (!IsAdmin) return Task.FromResult<string?>("Admins only.");
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            return Task.FromResult<string?>("Username and password are required.");
        if (_db.Table<User>().Where(u => u.Username == username).Count() > 0)
            return Task.FromResult<string?>("Username already exists.");

        _db.Insert(new User
        {
            Username = username.Trim(),
            Password = PasswordHasher.Hash(password),
            IsAdmin = isAdmin
        });
        return Task.FromResult<string?>(null);
    }

    public Task<string?> UpdateUserAsync(int id, string username, string? newPassword, bool isAdmin)
    {
        if (!IsAdmin) return Task.FromResult<string?>("Admins only.");
        var user = _db.Find<User>(id);
        if (user == null) return Task.FromResult<string?>("User not found.");
        if (string.IsNullOrWhiteSpace(username)) return Task.FromResult<string?>("Username is required.");

        if (_db.Table<User>().Where(u => u.Username == username && u.Id != id).Count() > 0)
            return Task.FromResult<string?>("Username already exists.");

        if (user.IsAdmin && !isAdmin && AdminCount() <= 1)
            return Task.FromResult<string?>("Can't demote the last admin.");

        user.Username = username.Trim();
        user.IsAdmin = isAdmin;
        if (!string.IsNullOrWhiteSpace(newPassword))
            user.Password = PasswordHasher.Hash(newPassword);

        _db.Update(user);
        if (CurrentUser?.Id == id) CurrentUser = user;
        return Task.FromResult<string?>(null);
    }

    public Task<string?> DeleteUserAsync(int id)
    {
        if (!IsAdmin) return Task.FromResult<string?>("Admins only.");
        if (CurrentUser?.Id == id) return Task.FromResult<string?>("You can't delete your own account.");
        var user = _db.Find<User>(id);
        if (user == null) return Task.FromResult<string?>("User not found.");
        if (user.IsAdmin && AdminCount() <= 1) return Task.FromResult<string?>("Can't delete the last admin.");

        _db.Delete(user);
        return Task.FromResult<string?>(null);
    }

    private int AdminCount() => _db.Table<User>().Where(u => u.IsAdmin).Count();
}