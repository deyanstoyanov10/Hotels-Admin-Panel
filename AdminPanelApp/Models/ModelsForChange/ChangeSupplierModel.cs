namespace AdminPanelApp.Models
{
    using System.ComponentModel.DataAnnotations;

    public class ChangeSupplierModel
    {
        [Required]
        [Display(Name = "Current Supplier")]
        public int CurrentSupplierIndex { get; set; }

        [Required]
        [Display(Name = "New Supplier")]
        public int NewSupplierIndex { get; set; }
    }
}
