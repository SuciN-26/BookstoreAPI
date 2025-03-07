using System.ComponentModel.DataAnnotations.Schema;

namespace BookstoreInventory.Models
{
    public class Book
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public string Title { get; set; }
        public Guid? AuthorId { get; set; }
        public virtual Author Author { get; set; }
    }
}
