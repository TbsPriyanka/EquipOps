using EquipOps.Model.ViewModelResponse;

namespace EquipOps.Model.Role
{
    public class UserRoleResponse : CommonParameterList
    {
        public List<UserRoleResponseViewModel> userRoleResponseViewModel { get; set; } = new List<UserRoleResponseViewModel>();
    }
    public class UserRoleResponseViewModel : CommonParameterAllList
    {
        public Guid? id { get; set; }
        public string name { get; set; } = null!;
    }

    public class UserRoleDeleteResponseViewModel
    {
        public Guid? id { get; set; }
    }
}
