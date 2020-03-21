namespace AdminPanelApp.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class NextProductsSchedule
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime NextSchedule { get; set; }
    }
}
