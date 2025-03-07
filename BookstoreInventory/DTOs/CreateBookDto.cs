namespace BookstoreInventory.DTOs
{
    public class CreateBookDto
    {
        public string Title { get; set; }
        public Guid AuthorId { get; set; }
    }
}
