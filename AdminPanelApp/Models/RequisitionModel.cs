namespace AdminPanelApp.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class RequisitionModel
    {
        [Required]
        public DateTime Date { get; set; }

        [Required]
        public string Location { get; set; }

        [Required]
        public ushort Status { get; set; }

        public string RequisitionId { get; set; }

        [Required]
        public string AddedBy { get; set; }
    }
}
