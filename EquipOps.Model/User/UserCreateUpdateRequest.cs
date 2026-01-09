namespace EquipOps.Model.User
{
    public class UserCreateUpdateRequest
    {
        public Guid? Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public Guid RoleId { get; set; }        
        public Guid OrganizationId { get; set; }   
        public string PhoneNumber { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
    }
}
