using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace WebApplication1.DTO
{
    public class CreateEnrollmentDto
    {
        [JsonRequired]
        [Required(ErrorMessage = "Class ID is required.")]
        public Guid ClassId { get; set; }

        [MaxLength(500, ErrorMessage = "Notes cannot exceed 500 characters.")]
        public string? Notes { get; set; }
    }
}
