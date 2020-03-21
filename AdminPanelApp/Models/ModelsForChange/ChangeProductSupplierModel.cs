namespace AdminPanelApp.Models
{
    using System.ComponentModel.DataAnnotations;

    public class ChangeProductSupplierModel
    {
        [Required]
        [Display(Name = "Select Product")]
        public int ProductId { get; set; }
        
        [Display(Name = "Current Supplier")]
        public string CurrentSupplier { get; set; }

        [Required]
        [Display(Name = "New Supplier")]
        public int NewSupplierIndex { get; set; }
    }
}
