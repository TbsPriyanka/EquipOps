using EquipOps.Common.Helper;
using EquipOps.DAL.Interfaces;
using EquipOps.Model.Organization;
using System.Data;

namespace EquipOps.DAL.Repository
{
    public class OrganizationRepository(PgHelper _pghelper) : IOrganizationRepository
    {
        public async Task<OrganizationResponseViewModel> OrganizationCreateAsync(OrganizationRequest request)
        {
            var param = new Dictionary<string, DbParam>
            {
                { "p_return_organization_id", new DbParam { DbType = DbType.Int32, Direction = ParameterDirection.InputOutput } },
                { "p_return_updated_at", new DbParam { DbType = DbType.DateTime, Direction = ParameterDirection.InputOutput } },
                { "p_organization_id", new DbParam { Value = request.organization_id == 0 ? null : request.organization_id, DbType = DbType.Int32 } },
                { "p_name", new DbParam { Value = request.name, DbType = DbType.String } },
                { "p_address", new DbParam { Value = request.address, DbType = DbType.String } },
                { "p_contact_email", new DbParam { Value = request.contact_email, DbType = DbType.String } },
                { "p_contact_phone", new DbParam { Value = request.contact_phone, DbType = DbType.String } }
            };

            var result = await _pghelper.CreateUpdateAsync("master.sp_organization_create_update", param);

            return new OrganizationResponseViewModel
            {
                organization_id = result.p_return_organization_id,
                name = request.name,
                address = request.address,
                contact_email = request.contact_email,
                contact_phone = request.contact_phone,
                updated_date = result.p_return_updated_at
            };
        }

        public async Task<OrganizationResponse> OrganizationListAsync(string? search, int length, int page, string orderColumn, string orderDirection)
        {
            var Params = new Dictionary<string, DbParam>
            {
                { "p_search", new DbParam { Value = search ?? "", DbType = DbType.String } },
                { "p_length", new DbParam { Value = length, DbType = DbType.Int32 } },
                { "p_page", new DbParam { Value = page, DbType = DbType.Int32 } },
                { "p_order_column", new DbParam { Value = orderColumn, DbType = DbType.String } },
                { "p_order_direction", new DbParam { Value = orderDirection, DbType = DbType.String } },
                { "o_total_records", new DbParam { DbType = DbType.Int32, Direction = ParameterDirection.InputOutput } },
                { "ref", new DbParam { Value = "mycursor", DbType = DbType.String, Direction = ParameterDirection.InputOutput } }
            };

            dynamic response = await _pghelper.ListAsync("master.sp_organization_list", Params);

            OrganizationResponse list = new OrganizationResponse();
            list.TotalNumbers = response.o_total_records;

            foreach (var row in response.@ref)
            {
                list.organizationResponseViewModel.Add(new OrganizationResponseViewModel
                {
                    organization_id = row.organization_id,
                    name = row.name,
                    address = row.address,
                    contact_email = row.contact_email,
                    contact_phone = row.contact_phone,
                    created_date = row.created_at,
                    updated_date = row.updated_at
                });
            }

            return list;
        }

        public async Task<OrganizationResponseViewModel> OrganizationByIdAsync(int? organization_id)
        {
            var Params = new Dictionary<string, DbParam>
            {
                { "p_organization_id", new DbParam { Value = organization_id, DbType = DbType.Int32 } },
                { "ref", new DbParam { Value = "mycursor", DbType = DbType.String, Direction = ParameterDirection.InputOutput } }
            };

            dynamic response = await _pghelper.ListAsync("master.sp_organization_getbyid", Params);
            dynamic obj = response.@ref[0];

            return new OrganizationResponseViewModel
            {
                organization_id = obj.organization_id,
                name = obj.name,
                address = obj.address,
                contact_email = obj.contact_email,
                contact_phone = obj.contact_phone,
                created_date = obj.created_at,
                updated_date = obj.updated_at
            };
        }

        public async Task<OrganizationDeleteResponseViewModel> OrganizationDeleteAsync(OrganizationDeleteRequestViewModel request)
        {
            var param = new Dictionary<string, DbParam>
            {
                { "p_return_organization_id", new DbParam { DbType = DbType.Int32, Direction = ParameterDirection.InputOutput } },
                { "p_return_updated_at", new DbParam { DbType = DbType.DateTime, Direction = ParameterDirection.InputOutput } },
                { "p_organization_id", new DbParam { Value = request.organization_id, DbType = DbType.Int32 } }
            };

            dynamic result = await _pghelper.CreateUpdateAsync("master.sp_organization_delete", param);

            return new OrganizationDeleteResponseViewModel
            {
                organization_id = result.p_return_organization_id
            };
        }
    }
}
