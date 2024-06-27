using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using WebApiAppSS.Data;
using WebApiAppSS.Data.Dtos.Group;
using WebApiAppSS.Data.Dtos.Invitation;
using WebApiAppSS.Models;

namespace WebApiAppSS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvitationController : ControllerBase
    {
        private readonly Context db;

        public InvitationController(Context context)
        {
            db = context;
        }

        [HttpPost]
        [Route("CreateInvitation")]
        public async Task<IActionResult> CreateInvitation([FromForm] CreateInvitationDto invitationDto)
        {
            try
            {
                // Verifica se já existe um convite para o mesmo grupo e destinatário
                var existingInvitation = await db.Invitation.FirstOrDefaultAsync(inv => inv.GroupId == invitationDto.GroupId && inv.RecipientId == invitationDto.RecipientId);

                if (existingInvitation != null)
                {
                    // Convite já existe, retorne um código de status correspondente
                    return StatusCode(409); // Conflito, indicando que o convite já existe
                }

                var invitation = new Invitation
                {
                    GroupId = invitationDto.GroupId,
                    RecipientId = invitationDto.RecipientId,
                    SenderId = invitationDto.SenderId,
                    Status = invitationDto.Status,
                };

                db.Invitation.Add(invitation);
                await db.SaveChangesAsync();

                return StatusCode(201);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Erro ao criar convite: {ex}");
                return StatusCode(500);
            }
        }

        [HttpPut]
        [Route("UpdateInvitationStatus/{groupId}")]
        public async Task<IActionResult> UpdateInvitationStatus(int groupId, [FromForm] UpdateInvitationDto updateDto)
        {
            try
            {
                var invitation = await db.Invitation.FirstOrDefaultAsync(i => i.GroupId == groupId && i.RecipientId == updateDto.RecipientId);

                if (invitation == null)
                {
                    return NotFound("Convite não encontrado.");
                }

                invitation.Status = updateDto.Status;
                db.Invitation.Update(invitation);
                await db.SaveChangesAsync();

                return Ok();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Erro ao atualizar status do convite: {ex}");
                return StatusCode(500, "Erro interno no servidor.");
            }
        }


    }
}
