using EquipOps.Common.Helper;
using EquipOps.DAL.Interfaces;
using EquipOps.Model.Role;
using Microsoft.AspNetCore.Http;
using System.Data;
using System.Security.Claims;

namespace EquipOps.DAL.Repository
{
    public class UserRoleRepository(IHttpContextAccessor _contextAccessor, PgHelper _pghelper) : IUserRoleRepository
    {
        public async Task<UserRoleResponseViewModel> UserRoleCreateAsync(UserRoleRequest request)
        {
            var userIdClaim = _contextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var updated_by = string.IsNullOrWhiteSpace(userIdClaim) ? (Guid?)null : Guid.Parse(userIdClaim);

            var param = new Dictionary<string, DbParam>
            {
                { "_return_id", new DbParam { Value = null, DbType = DbType.Guid, Direction = ParameterDirection.InputOutput } },
                { "_return_createddate", new DbParam { Value = null, DbType = DbType.DateTime, Direction = ParameterDirection.InputOutput } },
                { "_return_updateddate", new DbParam { Value = null, DbType = DbType.DateTime, Direction = ParameterDirection.InputOutput } },
                { "_id", new DbParam { Value = request.id, DbType = DbType.Guid } },
                { "_name", new DbParam { Value = request.name, DbType = DbType.String } },
                { "_created_by", new DbParam { Value = updated_by, DbType = DbType.Guid } },
                { "_is_delete", new DbParam { Value = false, DbType = DbType.Boolean } },
                { "_is_active", new DbParam { Value = true, DbType = DbType.Boolean } }
            };

            dynamic result = await _pghelper.CreateUpdateAsync("master.sp_user_role_create_update", param);

            var data = new UserRoleResponseViewModel
            {
                id = result._return_id,
                name = request.name,
                is_delete = false,
                is_active = true,
                created_by = updated_by,
                created_date = result._return_createddate,
                updated_date = result._return_updateddate,
            };
            return data;
        }
        public async Task<UserRoleResponse> UserRoleListAsync(string? search, bool? IsActive, int length, int page, string orderColumn, string orderDirection)
        {
            var Params = new Dictionary<string, DbParam>
            {
                { "p_search",         new DbParam { Value = search ?? "", DbType = DbType.String } },
                { "p_is_active",      new DbParam { Value = IsActive, DbType = DbType.Boolean } },
                { "p_length",         new DbParam { Value = length, DbType = DbType.Int32 } },
                { "p_page",           new DbParam { Value = page, DbType = DbType.Int32 } },
                { "p_order_column",   new DbParam { Value = orderColumn, DbType = DbType.String } },
                { "p_order_direction",new DbParam { Value = orderDirection, DbType = DbType.String } },
                { "o_total_records",  new DbParam { Value = 0, DbType = DbType.Int32, Direction = ParameterDirection.InputOutput } },
                { "ref",              new DbParam { Value = "mycursor", DbType = DbType.String, Direction = ParameterDirection.InputOutput } }
            };

            dynamic response = await _pghelper.ListAsync("master.sp_get_user_role_list", Params);

            UserRoleResponse list = new UserRoleResponse();
            list.TotalNumbers = response.o_total_records;

            foreach (var row in response.@ref)
            {
                list.userRoleResponseViewModel.Add(new UserRoleResponseViewModel
                {
                    id = row.id,
                    name = row.name,
                    created_by = row.created_by,
                    created_date = row.created_date,
                    updated_date = row.updated_date,
                    is_delete = row.is_delete,
                    is_active = row.is_active
                });
            }
            return list;
        }

        public async Task<UserRoleDeleteResponseViewModel> UserRoleDeleteAsync(UserRoleDeleteRequestViewModel request)
        {
            var param = new Dictionary<string, DbParam>
            {
                { "_return_id", new DbParam { Value = null, DbType = DbType.Guid, Direction = ParameterDirection.InputOutput } },
                { "_return_updateddate", new DbParam { Value = null, DbType = DbType.DateTime, Direction = ParameterDirection.InputOutput } },
                { "_id", new DbParam { Value = request.id, DbType = DbType.Guid } },
                { "_is_delete", new DbParam { Value = true, DbType = DbType.Boolean } },
                { "_is_active", new DbParam { Value = false, DbType = DbType.Boolean } }
            };

            dynamic result = await _pghelper.CreateUpdateAsync("master.sp_user_role_Delete", param);
            var data = new UserRoleDeleteResponseViewModel
            {
                id = result._return_id
            };
            return data;
        }

        public async Task<UserRoleResponseViewModel> UserRoleByIdAsync(Guid? id)
        {
            var Params = new Dictionary<string, DbParam>
            {
                { "_id", new DbParam { Value = id ?? null, DbType = DbType.Guid } },
                { "ref", new DbParam { Value = "mycursor", DbType = DbType.String, Direction = ParameterDirection.InputOutput } }
            };

            dynamic response = await _pghelper.ListAsync("master.sp_get_user_role_byid", Params);

            UserRoleResponseViewModel userRoleResponseViewModel = new UserRoleResponseViewModel();
            dynamic obj = response.@ref[0];
            userRoleResponseViewModel.id = obj.id;
            userRoleResponseViewModel.name = obj.name;
            userRoleResponseViewModel.created_by = obj.created_by;
            userRoleResponseViewModel.created_date = obj.created_date;
            userRoleResponseViewModel.updated_date = obj.updated_date;
            userRoleResponseViewModel.is_delete = obj.is_delete;
            userRoleResponseViewModel.is_active = obj.is_active;

            return userRoleResponseViewModel;
        }
    }
}
