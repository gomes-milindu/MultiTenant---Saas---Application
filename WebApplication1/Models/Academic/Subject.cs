using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApplication1.Models.Auth;

namespace WebApplication1.Models.Academic
{
    [Table("Subjects")]
    public class Subject
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty; 

        [MaxLength(300)]
        public string? Description { get; set; }

        [Required]
        public Guid OwnerUserId { get; set; }

        [Required]
        public Guid CreatedByUserId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;


        [ForeignKey(nameof(OwnerUserId))]
        public User? OwnerUser { get; set; }

        // Navigation Properties
        [ForeignKey(nameof(CreatedByUserId))]
        public User? CreatedByUser { get; set; }
    }
}
