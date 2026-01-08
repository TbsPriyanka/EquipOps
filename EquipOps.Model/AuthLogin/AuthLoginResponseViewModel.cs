namespace EquipOps.Model.AuthLogin
{
    public class AuthLoginResponseViewModel
    {
        public Guid? UserId { get; set; }
        public Guid? RoleId { get; set; }
        public string? RoleName { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string HashPassword { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Token { get; set; }
    }
}
