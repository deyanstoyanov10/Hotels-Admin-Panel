namespace AdminPanelApp.Models
{
    using System.ComponentModel.DataAnnotations;

    public class Product
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(50)]
        public string Name { get; set; }

        public string Unit { get; set; }

        public uint Price { get; set; }

        [MaxLength(50)]
        public string Category { get; set; }

        [MaxLength(50)]
        public string Supplier { get; set; }

        public uint Quantity { get; set; }

        public string Packaging { get; set; }

        public ushort Status { get; set; }

        public string RequisitionId { get; set; }

        public virtual Requisitions Requisition { get; set; }
    }
}
