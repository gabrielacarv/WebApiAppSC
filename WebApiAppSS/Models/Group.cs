namespace WebApiAppSS.Models
{
    public class Group
    {
        public int IdGroup { get; set; }
        public string Name { get; set; }
        public int MaxPeople { get; set; }
        public DateTime DisclosureDate { get; set; }
        public float Value { get; set; }
        public string Description { get; set; }
        public int Administrator { get; set; }
        public byte[] Icon { get; set; }
    }
}
