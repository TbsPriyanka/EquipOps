namespace EquipOps.Model.AuthLogin
{
    public class LogoutRequest
    {
        public Guid? UserId { get; set; }
        public string Token { get; set; } = string.Empty;
    }
}
