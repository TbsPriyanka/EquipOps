namespace EquipOps.API.Services.Interface
{
    public interface IEmailService
    {
        Task SendForgotPasswordEmailAsync(string email, string userName, string resetLink);
    }
}
