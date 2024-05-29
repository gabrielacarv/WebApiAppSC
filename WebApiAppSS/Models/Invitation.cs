namespace WebApiAppSS.Models
{
    public class Invitation
    {
        public int IdInvitation { get; set; }
        public int GroupId { get; set; }
        public int RecipientId { get; set; }
        public int SenderId { get; set; }
        public string? Status { get; set; }
    }

}
