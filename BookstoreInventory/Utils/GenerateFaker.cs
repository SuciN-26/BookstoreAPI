using BookstoreInventory.Models;
using Faker;

namespace BookstoreInventory.Utils
{
    public static class GenerateFaker
    {
        public static List<Author> GetAuthorList(int len) {

            var authorList = new List<Author>();

            for (int i = 0; i < len; i++) {

                authorList.Add(new Author { Id = Guid.NewGuid(), Name = Name.FullName()});
            }

            return authorList;
        }

    }
}
