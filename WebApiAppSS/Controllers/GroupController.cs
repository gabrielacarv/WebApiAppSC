using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using WebApiAppSS.Data;
using WebApiAppSS.Data.Dtos.Group;
using WebApiAppSS.Data.Dtos.User;
using WebApiAppSS.Models;

namespace WebApiAppSS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GroupController : ControllerBase
    {
        private readonly Context db;

        public GroupController(Context context)
        {
            db = context;
        }

        [HttpPost]
        [Route("CreateGroup")]
        public async Task<IActionResult> CreateGroup([FromForm] CreateGroupDto groupDto)
        {
            try
            {
                var group = new Models.Group
                {
                    Name = groupDto.Name,
                    MaxPeople = groupDto.MaxPeople,
                    DisclosureDate = groupDto.DisclosureDate,
                    Value = groupDto.Value,
                    Description = groupDto.Description,
                    Administrator = groupDto.Administrator,
                };

                if (groupDto.Icon != null && groupDto.Icon.Length > 0)
                {
                    using (var ms = new MemoryStream())
                    {
                        await groupDto.Icon.CopyToAsync(ms);
                        group.Icon = ms.ToArray();
                    }
                }

                db.Group.Add(group);
                await db.SaveChangesAsync();

                return StatusCode(201);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Erro ao adicionar grupo: {ex}");
                return StatusCode(500);
            }
        }

    

        [HttpGet]
        [Route("GetGroups")]
        public async Task<IActionResult> GetGroups()
        {
            try
            {
                var groups = await db.Group.ToListAsync();
                groups.Sort((g1, g2) => g1.Name.CompareTo(g2.Name));
                return Ok(groups); 
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Erro ao obter grupos: {ex}");
                return StatusCode(500);
            }
        }

        [HttpGet]
        [Route("GetGroupId/{id}")]
        public async Task<IActionResult> GetGroupId(int id)
        {
            try
            {
                var group = await db.Group.FirstOrDefaultAsync(g => g.IdGroup == id);
                return Ok(group);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Erro ao obter grupos: {ex}");
                return StatusCode(500);
            }
        }

        [HttpGet]
        [Route("GetParticipantsByGroup/{groupId}")]
        public async Task<IActionResult> GetParticipantsByGroup(int groupId)
        {
            try
            {
                var participants =  GetParticipants(groupId);
                participants.Sort((p1, p2) => p1.Name.CompareTo(p2.Name));
                return Ok(participants);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Erro ao obter participantes: {ex}");
                return StatusCode(500);
            }
        }

        private List<User> GetParticipants(int groupId)
        {
            return (from convite in db.Invitation
                    join usuario in db.User on convite.RecipientId equals usuario.Id
                    where convite.GroupId == groupId && convite.Status == "aceito"
                    select usuario).ToList();
        }

        [HttpGet]
        [Route("GetGroupsByUser/{userId}")]
        public async Task<IActionResult> GetGroupsByUser(int userId)
        {
            try
            {
                var groups = GetGroupByUser(userId);
                return Ok(groups);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Erro ao obter grupos: {ex}");
                return StatusCode(500);
            }
        }

        private List<Models.Group> GetGroupByUser(int userId)
        {
            //return (from grupo in db.Group
            //        join convite in db.Invitation on grupo.IdGroup equals convite.GroupId
            //        join usuario in db.User on convite.RecipientId equals usuario.Id
            //        where convite.RecipientId == userId && convite.Status == "aceito"
            //        select grupo).ToList();
            return (from grupo in db.Group
                    join convite in db.Invitation on grupo.IdGroup equals convite.GroupId into convites
                    from convite in convites.DefaultIfEmpty() // LEFT JOIN
                    join usuario in db.User on convite.RecipientId equals usuario.Id into usuarios
                    from usuario in usuarios.DefaultIfEmpty() // LEFT JOIN
                    where grupo.Administrator == userId || (convite != null && convite.RecipientId == userId && convite.Status == "Aceito")
                    select grupo).Distinct().ToList();
        }

        [HttpGet]
        [Route("GetGroupsPendingByUser/{userId}")]
        public async Task<IActionResult> GetGroupsPendingByUser(int userId)
        {
            try
            {
                var groups = GetGroupPendingByUser(userId);
                return Ok(groups);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Erro ao obter grupos: {ex}");
                return StatusCode(500);
            }
        }

        [HttpPut("UpdateGroup/{id}")]
        public async Task<IActionResult> UpdateGroup([FromForm] UpdateGroupDto updateGroupDto, int id)
        {
            Models.Group existingGroup = GetGroupById(id);

            if (existingGroup == null)
            {
                return NotFound();
            }

            if (updateGroupDto.Icon != null && updateGroupDto.Icon.Length > 0)
            {
                using (var ms = new MemoryStream())
                {
                    await updateGroupDto.Icon.CopyToAsync(ms);
                    existingGroup.Icon = ms.ToArray();
                }
            }

            existingGroup.Name = updateGroupDto.Name ?? existingGroup.Name;
            existingGroup.MaxPeople = ((int)(updateGroupDto.MaxPeople != null ? updateGroupDto.MaxPeople : existingGroup.MaxPeople));
            existingGroup.DisclosureDate = (DateTime)(updateGroupDto.DisclosureDate != null ? updateGroupDto.DisclosureDate : existingGroup.DisclosureDate);
            existingGroup.Description = updateGroupDto.Description ?? existingGroup.Description;
            existingGroup.Value = (float)(updateGroupDto.Value != null ? updateGroupDto.Value : existingGroup.Value);
            existingGroup.Administrator = (int)(updateGroupDto.Administrator != null ? updateGroupDto.Administrator : existingGroup.Administrator);

            db.Group.Update(existingGroup);

            await db.SaveChangesAsync();

            return StatusCode(200);
        }

        [HttpGet("GetGroupImage/{groupId}")]
        public IActionResult GetGroupImage(int groupId)
        {
            var group = db.Group.Find(groupId);
            if (group == null)
            {
                return NotFound();
            }

            byte[] imageData = group.Icon;

            if (imageData == null || imageData.Length == 0)
            {
                return NotFound();
            }

            string base64Image = Convert.ToBase64String(imageData);

            string imageDataUrl = $"data:image/jpeg;base64,{base64Image}";

            return Ok(new { imageUrl = base64Image });
        }

        [HttpPut]
        [Route("DeleteGroup/{groupId}")]
        public async Task<IActionResult> DeleteGroup(int groupId)
        {
            try
            {
                // Obtém o grupo pelo ID
                var group = await db.Group.FindAsync(groupId);
                if (group == null)
                {
                    return NotFound($"Grupo com ID {groupId} não encontrado.");
                }

                // Obtém todos os convites do grupo especificado
                var invitations = await db.Invitation
                    .Where(i => i.GroupId == groupId)
                    .ToListAsync();

                // Atualiza o status de todos os convites para "recusado"
                invitations.ForEach(i => i.Status = "recusado");

                // Remove o administrador do grupo
                group.Administrator = 2014;

                // Salva as alterações no banco de dados
                await db.SaveChangesAsync();

                return Ok();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Erro ao deletar grupo: {ex}");
                return StatusCode(500, "Erro ao deletar grupo. Tente novamente mais tarde.");
            }
        }

        [HttpPut]
        [Route("LeaveGroup/{groupId}/{participantId}")]
        public async Task<IActionResult> LeaveGroup(int groupId, int participantId)
        {
            try
            {
                // Verifica se o participante está convidado para o grupo
                var invitation = await db.Invitation.FirstOrDefaultAsync(i => i.GroupId == groupId && i.RecipientId == participantId);

                if (invitation == null)
                {
                    return NotFound($"Usuário com ID {participantId} não está convidado para o grupo com ID {groupId}.");
                }

                // Atualiza o status do convite para "recusado"
                invitation.Status = "recusado";

                // Salva as alterações no banco de dados
                await db.SaveChangesAsync();

                return Ok($"Usuário com ID {participantId} saiu do grupo com ID {groupId}.");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Erro ao sair do grupo: {ex}");
                return StatusCode(500, "Erro ao sair do grupo. Tente novamente mais tarde.");
            }
        }



        private Models.Group GetGroupById(int id)
        {
            var group = GetGroup();
            Models.Group existingGroup = group.FirstOrDefault(g => g.IdGroup == id);
            return existingGroup;
        }

        private List<Models.Group> GetGroup()
        {
            return db.Group.ToList();
        }

        private List<Models.Group> GetGroupPendingByUser(int userId)
        {
            return (from grupo in db.Group
                    join convite in db.Invitation on grupo.IdGroup equals convite.GroupId
                    join usuario in db.User on convite.RecipientId equals usuario.Id
                    where convite.RecipientId == userId && convite.Status == "pendente"
                    select grupo).ToList();
        }
    }
}
