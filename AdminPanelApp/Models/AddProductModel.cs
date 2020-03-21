namespace AdminPanelApp.Models
{
    using System.ComponentModel.DataAnnotations;

    public class AddProductModel
    {
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [Required]
        public string Unit { get; set; }

        [Required]
        [MaxLength(50)]
        public string Packaging { get; set; }

        [Required]
        public uint Price { get; set; }

        [Required]
        public int Category { get; set; }

        [Required]
        [MaxLength(50)]
        public string Supplier { get; set; }
    }
}
