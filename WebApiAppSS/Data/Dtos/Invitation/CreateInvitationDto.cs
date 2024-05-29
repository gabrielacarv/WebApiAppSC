namespace WebApiAppSS.Data.Dtos.Invitation
{
    public class CreateInvitationDto
    {
        public int GroupId { get; set; }
        public int RecipientId { get; set; }
        public int SenderId { get; set; }
        public string? Status { get; set; }
    }
}
