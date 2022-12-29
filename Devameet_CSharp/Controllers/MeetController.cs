using Devameet_CSharp.Dtos;
using Devameet_CSharp.Models;
using Devameet_CSharp.Repository;
using Microsoft.AspNetCore.Mvc;

namespace Devameet_CSharp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MeetController : BaseController
    {
        public readonly ILogger<MeetController> _logger;
        public readonly IMeetRepository _meetRepository;
        public readonly IMeetObjectRepository _meetObjectRepository;

        public MeetController(ILogger<MeetController> logger,
            IMeetRepository meetRepository, IMeetObjectRepository meetObjectRepository, IUserRepository userRepository) : base(userRepository)
        {
            _logger = logger;
            _meetRepository = meetRepository;
            _meetObjectRepository = meetObjectRepository;
        }

        [HttpGet]
        public IActionResult GetMeet()
        {
            try
            {
                User user = LerToken();

                List<Meet> meet = _meetRepository.GetMeetByUser(user.Id);

                return Ok(meet);

            }
            catch (Exception e)
            {
                _logger.LogError("Ocorreu um erro ao obter o a sala de reunião");
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorRespostaDto()
                {
                    Descricao = "Ocorreu o seguinte erro: " + e.Message,
                    Status = StatusCodes.Status500InternalServerError
                });
            }
        }

        [HttpDelete]
        public IActionResult DeleteMeet(int meetId)
        {
            try
            {
                _meetRepository.DeleteMeet(meetId);
                return Ok("Sala de reunião excluída com sucesso");

            }
            catch (Exception e)
            {
                _logger.LogError("Ocorreu um erro ao deletar a sala de reunião");
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorRespostaDto()
                {
                    Descricao = "Ocorreu o seguinte erro: " + e.Message,
                    Status = StatusCodes.Status500InternalServerError
                });
            }
        }

        [HttpPost]
        public IActionResult CreateMeet([FromBody] MeetRequestDto meetRequest)
        {
            try
            {
                Meet meet = new Meet();
                meet.Name = meetRequest.Name;
                meet.Color = meetRequest.Color;
                meet.UserId = LerToken().Id;
                meet.Link = Guid.NewGuid().ToString();

                _meetRepository.CreateMeet(meet);
                return Ok("Sala de reunião salva com sucesso");

            }
            catch (Exception e)
            {
                _logger.LogError("Ocorreu um erro ao inlcuir a sala de reunião");
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorRespostaDto()
                {
                    Descricao = "Ocorreu o seguinte erro: " + e.Message,
                    Status = StatusCodes.Status500InternalServerError
                });
            }
        }

        [HttpPut]
        public IActionResult UpdateMeet([FromBody] MeetUpdateRequestDto meetRequest, int meetId)
        {
            try
            {
                Meet meet = _meetRepository.GetMeetById(meetId);
                meet.Name = meetRequest.Name;
                meet.Color = meetRequest.Color;
                _meetRepository.UpdateMeet(meet);
                List<MeetObjects> meetObjects = new List<MeetObjects>();

                foreach (ObjectMeetRequestDto meetObjectRequest in meetRequest.ObjectsMeet)
                {
                    MeetObjects meetObject = new MeetObjects();
                    meetObject.Name = meetObjectRequest.Name;
                    meetObject.X = meetObjectRequest.X;
                    meetObject.Y = meetObjectRequest.Y;
                    meetObject.Orientation = meetObjectRequest.Orientation;
                    meetObject.MeetId = meetId;
                    meetObject.ZIndex = meetObjectRequest.ZIndex;
                    meetObjects.Add(meetObject);
                }
                _meetObjectRepository.CreateObjectsMeet(meetObjects, meetId);
                return Ok("Sala de reunião atualizada com sucesso");

            }
            catch (Exception e)
            {
                _logger.LogError("Ocorreu um erro ao inlcuir a sala de reunião");
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorRespostaDto()
                {
                    Descricao = "Ocorreu o seguinte erro: " + e.Message,
                    Status = StatusCodes.Status500InternalServerError
                });
            }
        }


    }
}
