namespace EquipOps.BAL.Interfaces
{
    public interface IEmailService
    {
        Task SendForgotPasswordEmailAsync(string email, string userName, string resetLink);
    }
}
