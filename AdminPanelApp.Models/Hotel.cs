namespace AdminPanelApp.Models
{
    using System.ComponentModel.DataAnnotations;

    public class Hotel
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public string Address { get; set; }

        public string City { get; set; }

        public string PostralCode { get; set; }
    }
}
