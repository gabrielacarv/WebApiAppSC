namespace WebApiAppSS.Data.Dtos.User
{
    public class CreateUserDto
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public IFormFile Photo { get; set; }
    }
}
