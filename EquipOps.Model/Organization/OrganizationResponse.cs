namespace EquipOps.Model.Organization
{
    public class OrganizationResponse
    {
        public int TotalNumbers { get; set; }
        public List<OrganizationResponseViewModel> organizationResponseViewModel { get; set; } = new List<OrganizationResponseViewModel>();
    }
}
