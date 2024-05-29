namespace WebApiAppSS.Data.Dtos.Group
{
    public class CreateGroupDto
    {
        public string Name { get; set; }
        public int MaxPeople { get; set; }
        public DateTime DisclosureDate { get; set; }
        public float Value { get; set; }
        public string Description { get; set; }
        public int Administrator { get; set; }
        public IFormFile Icon { get; set; }
    }
}
