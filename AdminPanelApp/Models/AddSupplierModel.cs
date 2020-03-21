namespace AdminPanelApp.Models
{
    using System.ComponentModel.DataAnnotations;

    public class AddSupplierModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
