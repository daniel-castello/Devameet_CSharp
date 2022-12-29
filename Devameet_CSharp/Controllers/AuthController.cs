using Devameet_CSharp.Dtos;
using Devameet_CSharp.Models;
using Devameet_CSharp.Repository;
using Devameet_CSharp.Service;
using Devameet_CSharp.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Devameet_CSharp.Controllers
{
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;
        private readonly IUserRepository _userRepository;

        public AuthController(ILogger<AuthController> logger, IUserRepository userRepository)
        {
            _logger = logger;
            _userRepository = userRepository;
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("api/[controller]/login")]
        public IActionResult ExecuteLogin([FromBody] LoginRequestDto loginrequest)
        {
            try
            {
                if (!String.IsNullOrEmpty(loginrequest.Password) && !String.IsNullOrEmpty(loginrequest.Login) &&
                    !String.IsNullOrWhiteSpace(loginrequest.Password) && !String.IsNullOrWhiteSpace(loginrequest.Login))
                {

                    User user = _userRepository.GetUserByLoginPassword(loginrequest.Login.ToLower(), MD5Utils.GenerateHashMD5(loginrequest.Password));

                    if (user != null)
                    {
                        return Ok(new LoginResponseDto()
                        {
                            Email = user.Email,
                            Name = user.Name,
                            Token = TokenService.CreateToken(user)
                        });
                    }
                    else
                    {
                        return BadRequest(new ErrorResponseDto()
                        {
                            Description = "Email ou sennha inválido!",
                            Status = StatusCodes.Status400BadRequest
                        });
                    }

                }
                else
                {
                    return BadRequest(new ErrorResponseDto()
                    {
                        Description = "Usuário não preencheu os campos de login corretamente",
                        Status = StatusCodes.Status400BadRequest
                    });
                }
            }
            catch (Exception e)
            {
                _logger.LogError("Ocorreu um erro no login: " + e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponseDto()
                {
                    Description = "Ocorreu um erro ao fazer o login",
                    Status = StatusCodes.Status500InternalServerError
                });
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("api/[controller]/register")]
        public IActionResult SaveUser([FromBody] UserRequestDto userdto)
        {
            try
            {

                if (userdto != null)
                {
                    var erros = new List<string>();

                    if (string.IsNullOrEmpty(userdto.Name) || string.IsNullOrWhiteSpace(userdto.Name))
                    {
                        erros.Add("Nome inválido");
                    }
                    if (string.IsNullOrEmpty(userdto.Email) || string.IsNullOrWhiteSpace(userdto.Email) || !userdto.Email.Contains("@"))
                    {
                        erros.Add("E-mail inválido");
                    }
                    if (string.IsNullOrEmpty(userdto.Password) || string.IsNullOrWhiteSpace(userdto.Password))
                    {
                        erros.Add("Senha inválido");
                    }

                    if (erros.Count > 0)
                    {
                        return BadRequest(new ErrorRespostaDto()
                        {
                            Status = StatusCodes.Status400BadRequest,
                            Erros = erros
                        });
                    }

                    User user = new User()
                    {
                        Email = userdto.Email,
                        Password = userdto.Password,
                        Name = userdto.Name,
                        Avatar = userdto.Avatar
                    };



                    user.Password = Utils.MD5Utils.GenerateHashMD5(user.Password);
                    user.Email = user.Email.ToLower();

                    if (!_userRepository.VerifyEmail(user.Email))
                    {
                        _userRepository.Save(user);
                    }
                    else
                    {
                        return BadRequest(new ErrorRespostaDto()
                        {
                            Status = StatusCodes.Status400BadRequest,
                            Descricao = "Usuário já está cadastrado!"
                        });
                    }

                }

                return Ok("Usuário foi salvo com sucesso");
            }
            catch (Exception e)
            {
                _logger.LogError("Ocorreu um erro ao salvar o usuário");
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorRespostaDto()
                {
                    Descricao = "Ocorreu o seguinte erro: " + e.Message,
                    Status = StatusCodes.Status500InternalServerError
                });
            }
        }

    }
}
