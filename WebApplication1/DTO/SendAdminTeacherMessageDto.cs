using System.ComponentModel.DataAnnotations;
using WebApplication1.Models.Enums;

namespace WebApplication1.DTO
{
    public class SendAdminTeacherMessageDto
    {

        [Required]
        public MessageChannel Channel { get; set; } = MessageChannel.EMAIL;

        [Required]
        public TargetScopeType TargetScope { get; set; } = TargetScopeType.ALL;

        public List<Guid>? TeacherIds { get; set; }

        [MaxLength(200)]
        public string? Subject { get; set; } 

        [Required(ErrorMessage = "Message body cannot be empty.")]
        public string MessageBody { get; set; } = string.Empty;
    }
}
