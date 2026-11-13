using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models.Subscrition
{
    [Table("PackageReminderTargetTeachers")]
    public class PackageReminderTargetTeacher
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid TeacherUserId { get; set; }
    }
}
