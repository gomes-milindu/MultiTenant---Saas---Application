using System.ComponentModel.DataAnnotations;

namespace WebApplication1.DTO
{
    public class SendAdminTeacherMessageDto
    {
        
        [Required]
        [RegularExpression("^(EMAIL|SMS|BOTH)$", ErrorMessage = "Channel must be EMAIL, SMS, or BOTH.")]
        public string Channel { get; set; } = "EMAIL";

        [Required]
        [RegularExpression("^(ALL|SPECIFIC)$", ErrorMessage = "TargetScope must be ALL or SPECIFIC.")]
        public string TargetScope { get; set; } = "ALL";

        public List<Guid>? TeacherIds { get; set; }

        [MaxLength(200)]
        public string? Subject { get; set; } 

        [Required(ErrorMessage = "Message body cannot be empty.")]
        public string MessageBody { get; set; } = string.Empty;
    }
}
