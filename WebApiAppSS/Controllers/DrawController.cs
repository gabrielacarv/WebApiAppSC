using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using WebApiAppSS.Data;
using WebApiAppSS.Data.Dtos.Draw;
using WebApiAppSS.Models;

namespace WebApiAppSS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DrawController : ControllerBase
    {
        private readonly Context db;

        public DrawController(Context context)
        {
            db = context;
        }

        [HttpPost]
        [Route("ConductDraw")]
        public IActionResult ConductDraw([FromBody] DrawDto drawDto)
        {
            try
            {
                PerformDraw(drawDto.GroupId);
                return Ok("Sorteio realizado com sucesso!");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Erro ao realizar sorteio: {ex}");
                return StatusCode(500, $"Erro ao realizar sorteio: {ex.Message}");
            }
        }

        [HttpGet]
        [Route("GetDrawResult/{groupId}/{participantId}")]
        public IActionResult GetDrawResult(int groupId, int participantId)
        {
            try
            {
                var result = from draw in db.Draw
                             join user in db.User on draw.SelectedId equals user.Id
                             where draw.GroupId == groupId && draw.ParticipantId == participantId
                             select new
                             {
                                 ParticipantId = draw.ParticipantId,
                                 SelectedName = user.Name
                             };

                var drawResult = result.FirstOrDefault();

                if (drawResult == null)
                {
                    return NotFound("Resultado do sorteio não encontrado.");
                }

                return Ok(drawResult);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Erro ao buscar resultado do sorteio: {ex}");
                return StatusCode(500, $"Erro ao buscar resultado do sorteio: {ex.Message}");
            }
        }

        private void PerformDraw(int groupId)
        {
            var participants = GetParticipants(groupId);

            if (participants.Count < 2)
            {
                throw new InvalidOperationException("O grupo deve ter pelo menos 2 participantes.");
            }

            var drawResults = Shuffle(participants);

            SaveDraw(groupId, drawResults);
        }

        private Dictionary<int, int> Shuffle(List<User> participants)
        {
            var rand = new Random();
            var drawResults = new Dictionary<int, int>();
            var recipientIds = participants.Select(p => p.Id).ToList();

            foreach (var participant in participants)
            {
                var possibleRecipients = recipientIds.Where(r => r != participant.Id).ToList();

                if (possibleRecipients.Count == 0)
                {
                    return Shuffle(participants);
                }

                var selected = possibleRecipients[rand.Next(possibleRecipients.Count)];
                drawResults[participant.Id] = selected;
                recipientIds.Remove(selected);
            }

            return drawResults;
        }

        private void SaveDraw(int groupId, Dictionary<int, int> drawResults)
        {
            foreach (var entry in drawResults)
            {
                var draw = new Draw
                {
                    GroupId = groupId,
                    ParticipantId = entry.Key,
                    SelectedId = entry.Value
                };

                db.Draw.Add(draw);
            }

            db.SaveChanges();
        }

        private List<User> GetParticipants(int groupId)
        {
            return (from invite in db.Invitation
                    join user in db.User on invite.RecipientId equals user.Id
                    where invite.GroupId == groupId && invite.Status == "aceito"
                    select user).ToList();
        }
    }
}
