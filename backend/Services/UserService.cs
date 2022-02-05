public class UserService
{
    private List<UserDto> _users => new()
    {
        new UserDto("Vladimir", "123")
    };

    public UserDto GetUser(User user) =>
      _users.FirstOrDefault(u => 
            string.Equals(u.UserName, user.UserName) &&
            string.Equals(u.Password, user.Password)) ??
            throw new Exception();  
}