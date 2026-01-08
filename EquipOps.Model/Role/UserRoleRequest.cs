namespace EquipOps.Model.Role
{
    public class UserRoleRequest
    {
        public Guid? id { get; set; }
        public string name { get; set; } = null!;
    }

    public class UserRoleDeleteRequestViewModel
    {
        public Guid? id { get; set; }
    }
}