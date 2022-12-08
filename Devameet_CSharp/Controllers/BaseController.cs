using Devameet_CSharp.Models;
using Devameet_CSharp.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Devameet_CSharp.Controllers
{
        [Authorize]
        public class BaseController : ControllerBase
        {
            protected readonly IUserRepository _userRepository;

            public BaseController(IUserRepository userRepository)
            {
                _userRepository = userRepository;
            }

            protected User LerToken()
            {
                var idUsuario = User.Claims.Where(c => c.Type == ClaimTypes.Sid).Select(c => c.Value).FirstOrDefault();

                if (string.IsNullOrEmpty(idUsuario))
                {
                    return null;
                }
                else
                {
                    return _userRepository.GetUserById(int.Parse(idUsuario));
                }

            }
        }
}
