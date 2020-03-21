namespace AdminPanelApp.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class Requisitions
    {
        public Requisitions()
        {
            this.Products = new HashSet<Product>();
        }

        [Key]
        public string Id { get; set; }

        public DateTime Date { get; set; }

        public string Location { get; set; }

        public ushort Status { get; set; }

        public uint Total { get; set; }

        public DateTime ScheduleFor { get; set; }

        public string AddedBy { get; set; }

        public ICollection<Product> Products { get; set; }
    }
}
