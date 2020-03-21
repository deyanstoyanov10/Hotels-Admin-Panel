namespace AdminPanelApp.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class NextScheduleModel
    {
        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Next Schedule Time")]
        public DateTime NextSchedule { get; set; }
    }
}
