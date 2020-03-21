namespace AdminPanelApp.Models
{
    using System.ComponentModel.DataAnnotations;

    public class Products
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(50)]
        public string Name { get; set; }

        [MaxLength(50)]
        public string Packaging { get; set; }

        public string Unit { get; set; }

        public uint Price { get; set; }

        [MaxLength(50)]
        public string Category { get; set; }

        [MaxLength(50)]
        public string Supplier { get; set; }

        public byte[] Picture { get; set; }
    }
}
