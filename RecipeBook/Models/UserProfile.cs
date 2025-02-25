namespace RecipeBook.Models
{
    public class UserProfile
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string BirthDate { get; set; } // Дата рождения как строка "yyyy-MM-dd"
    }
}
