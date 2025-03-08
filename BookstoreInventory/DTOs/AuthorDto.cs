namespace BookstoreInventory.DTOs
{
    public class AuthorDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public List<Guid>? BookIds { get; set; }
    }
}
