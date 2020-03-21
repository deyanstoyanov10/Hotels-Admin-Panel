namespace AdminPanelApp.Models
{
    using System.ComponentModel.DataAnnotations;

    public class UserInfo
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string Hotel { get; set; }

        [Required]
        public string UserName { get; set; }
    }
}
