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
    [Route("api/[controller]/login")]
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
        public IActionResult ExecuteLogin([FromBody] LoginRequestDto loginrequest)
        {
            try
            {
                if (!String.IsNullOrEmpty(loginrequest.Password) && !String.IsNullOrEmpty(loginrequest.Email) &&
                    !String.IsNullOrWhiteSpace(loginrequest.Password) && !String.IsNullOrWhiteSpace(loginrequest.Email))
                {

                    User user = _userRepository.GetUserByLoginPassword(loginrequest.Email.ToLower(), MD5Utils.GenerateHashMD5(loginrequest.Password));

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

    }
}
