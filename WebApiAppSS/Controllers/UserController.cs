using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using WebApiAppSS.Data;
using WebApiAppSS.Data.Dtos.User;
using WebApiAppSS.Models;
using WebApiAppSS.Service.Authentication;
using static System.Net.Mime.MediaTypeNames;

namespace WebApiAppSS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly Context db;
        private readonly AuthService _authService;


        public UserController(IConfiguration config, Context context, IOptions<TokenSettings> tokenSettings, AuthService authService)
        {
            db = context;
            _authService = authService;
        }


        [HttpPost]
        [Route("Login")]
        public IActionResult Login(LoginDto loginDto)
        {
            try
            {
                var user = db.User.FirstOrDefault(u => u.Email == loginDto.Email && u.Password == loginDto.Password);
                if (user == null)
                {
                    return Unauthorized();
                }

                var token = _authService.GenerateToken(user.Email);
                return Ok(new { Token = token });
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Erro ao logar: {ex}");
                return StatusCode(500);
            }
        }

        [HttpGet]
        [Route("UserByEmail/{email}")]
        public async Task<IActionResult> getUserByEmail(string email)
        {
            try
            {
                var user = await db.User.FirstOrDefaultAsync(u => u.Email == email);
                return Ok(user);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Erro ao obter usuário: {ex}");
                return StatusCode(500);
            }
        }


        [HttpPost]
        [Route("AddUser")]     
        public async Task<IActionResult> AddUser([FromForm] CreateUserDto userDto)
        {
            try
            {
                if (!CheckEmail(userDto.Email))
                {
                    return StatusCode(409);
                }

                var user = new User
                {
                    Name = userDto.Name,
                    Email = userDto.Email,
                    Password = userDto.Password
                };

                if (userDto.Photo != null && userDto.Photo.Length > 0)
                {
                    using (var ms = new MemoryStream())
                    {
                        await userDto.Photo.CopyToAsync(ms);
                        user.Photo = ms.ToArray();
                    }
                }

                db.User.Add(user);
                await db.SaveChangesAsync();

                return StatusCode(201);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Erro ao adicionar usuário: {ex}");
                return StatusCode(500);
            }
        }


        [HttpPut("UpdateUser/{id}")]
        public async Task<IActionResult> UpdateUser([FromForm] UpdateUserDto updateUserDto, int id)
        {
            User existingUser = GetUserById(id);

            if (existingUser == null)
            {
                return NotFound();
            }

            if (updateUserDto.Photo != null && updateUserDto.Photo.Length > 0)
            {
                using (var ms = new MemoryStream())
                {
                    await updateUserDto.Photo.CopyToAsync(ms);
                    existingUser.Photo = ms.ToArray();
                }
            }

            existingUser.Name = updateUserDto.Name ?? existingUser.Name;
            existingUser.Email = updateUserDto.Email ?? existingUser.Email;
            existingUser.Password = updateUserDto.Password ?? existingUser.Password;

            db.User.Update(existingUser); // Use Update ao invés de Add para atualizar a entidade

            await db.SaveChangesAsync();

            return StatusCode(200);
        }

        //[HttpGet("{userId}")]
        //public IActionResult GetUserImage(int userId)
        //{
        //    var user = db.User.Find(userId);
        //    if (user == null)
        //    {
        //        return NotFound();
        //    }

        //    // Aqui você precisa recuperar os dados da imagem do usuário do banco de dados
        //    // e retorná-los como um arquivo
        //    // Suponha que a imagem do usuário esteja armazenada como um array de bytes na propriedade "Photo" do objeto User
        //    byte[] imageData = user.Photo;

        //    // Verifique se os dados da imagem não são nulos ou vazios
        //    if (imageData == null || imageData.Length == 0)
        //    {
        //        return NotFound(); // Ou retorne uma imagem padrão ou um código de status NotFound, dependendo do seu caso de uso
        //    }

        //    // Defina o tipo de conteúdo para "imagem/jpeg" ou o tipo apropriado para sua imagem
        //    Response.Headers.Add("Content-Type", "image/jpeg");

        //    // Retorne os dados da imagem como um arquivo
        //    return File(imageData, "image/jpeg");
        //}

        [HttpGet("GetUserImage/{userId}")]
        public IActionResult GetUserImage(int userId)
        {
            var user = db.User.Find(userId);
            if (user == null)
            {
                return NotFound();
            }

            // Aqui você precisa recuperar os dados da imagem do usuário do banco de dados
            // e convertê-los para uma string base64
            byte[] imageData = user.Photo;

            // Verifique se os dados da imagem não são nulos ou vazios
            if (imageData == null || imageData.Length == 0)
            {
                return NotFound(); // Ou retorne uma imagem padrão ou um código de status NotFound, dependendo do seu caso de uso
            }

            // Converta os dados da imagem para uma string base64
            string base64Image = Convert.ToBase64String(imageData);

            // Construa o URL de dados para a imagem base64
            string imageDataUrl = $"data:image/jpeg;base64,{base64Image}";

            // Retorne a URL de dados como resposta
            return Ok(new { imageUrl = base64Image });
        }



        private User GetUserById(int id)
        {
            var user = GetUsers();
            User existingUser = user.FirstOrDefault(u => u.Id == id);
            return existingUser;
        }


        private bool CheckEmail(string email)
        {
            var user = GetUsers();
            var existingUser = user.FirstOrDefault(u => u.Email == email);
            return existingUser == null; 
        }


        private List<User> GetUsers()
        {
            return db.User.ToList();
        }

        //public IActionResult AddUser([FromForm] User user)
        //{
        //    db.User.Add(user);
        //    if (user.Photo != null && user.Photo.Length > 0)
        //    db.SaveChanges();
        //    return Ok(user);
        //}
    }
}
