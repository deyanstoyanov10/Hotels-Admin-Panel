namespace AdminPanelApp.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class PreviousSchedule
    {
        [Key]
        public int Id { get; set; }

        public DateTime LastSchedules { get; set; }
    }
}
