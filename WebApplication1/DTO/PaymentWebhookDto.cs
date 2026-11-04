namespace WebApplication1.DTO
{
    public class PaymentWebhookDto
    {
        public Guid TeacherPackageId { get; set; }
        public string PaymentReference { get; set; } = string.Empty;
    }
}
