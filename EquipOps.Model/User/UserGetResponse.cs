namespace EquipOps.Model.User
{
    public class UserListResponseViewModel
    {
        public int TotalNumbers { get; set; }
        public List<UserGetResponse> UserData { get; set; } = new List<UserGetResponse>();
    }
    public class UserGetResponse
    {
        public string First_name { get; set; } = string.Empty;
        public string Last_name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone_number { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public Guid Organization_id { get; set; }
        public Guid Role_id { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public bool IsEmailVerified { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
