using EquipOps.Common.Helper;
using EquipOps.DAL.Interfaces;
using EquipOps.Model.EquipmentCategory;
using System.Data;

namespace EquipOps.DAL.Repository
{
    public class EquipmentCategoryRepository(PgHelper _pghelper) : IEquipmentCategoryRepository
    {
        public async Task<EquipmentCategoryResponseViewModel> EquipmentCategoryCreateAsync(EquipmentCategoryRequest request)
        {
            var param = new Dictionary<string, DbParam>
            {
                { "p_return_category_id", new DbParam { DbType = DbType.Int32, Direction = ParameterDirection.InputOutput } },
                { "p_return_updated_at", new DbParam { DbType = DbType.DateTime, Direction = ParameterDirection.InputOutput } },
                { "p_category_id", new DbParam { Value = request.category_id == 0 ? null : request.category_id, DbType = DbType.Int32 } },
                { "p_organization_id", new DbParam { Value = request.organization_id, DbType = DbType.Int32 } },
                { "p_category_name", new DbParam { Value = request.category_name, DbType = DbType.String } },
                { "p_description", new DbParam { Value = request.description, DbType = DbType.String } }
            };

            var result = await _pghelper.CreateUpdateAsync("master.sp_equipment_category_create_update", param);

            return new EquipmentCategoryResponseViewModel
            {
                category_id = result.p_return_category_id,
                organization_id = request.organization_id,
                category_name = request.category_name,
                description = request.description,
                updated_date = result.p_return_updated_at
            };
        }

        public async Task<EquipmentCategoryResponse> EquipmentCategoryListAsync(string? search, int length, int page, string orderColumn, string orderDirection)
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

            dynamic response = await _pghelper.ListAsync("master.sp_equipment_category_list", Params);

            EquipmentCategoryResponse list = new EquipmentCategoryResponse();
            list.TotalNumbers = response.o_total_records;

            foreach (var row in response.@ref)
            {
                list.equipmentCategoryResponseViewModel.Add(new EquipmentCategoryResponseViewModel
                {
                    category_id = row.category_id,
                    organization_id = row.organization_id,
                    category_name = row.category_name,
                    organization_name = row.organization_name,
                    description = row.description,
                    created_date = row.created_at,
                    updated_date = row.updated_at
                });
            }

            return list;
        }

        public async Task<EquipmentCategoryDeleteRequestViewModel> EquipmentCategoryDeleteAsync(EquipmentCategoryDeleteRequestViewModel request)
        {
            var param = new Dictionary<string, DbParam>
            {
                { "p_return_category_id", new DbParam { DbType = DbType.Int32, Direction = ParameterDirection.InputOutput } },
                { "p_return_updated_at", new DbParam { DbType = DbType.DateTime, Direction = ParameterDirection.InputOutput } },
                { "p_category_id", new DbParam { Value = request.category_id, DbType = DbType.Int32 } }
            };

            dynamic result = await _pghelper.CreateUpdateAsync("master.sp_equipment_category_delete", param);

            return new EquipmentCategoryDeleteRequestViewModel
            {
                category_id = result.p_return_category_id
            };
        }

        public async Task<EquipmentCategoryResponseViewModel> EquipmentCategoryByIdAsync(int? category_id)
        {
            var Params = new Dictionary<string, DbParam>
            {
                { "p_category_id", new DbParam { Value = category_id, DbType = DbType.Int32 } },
                { "ref", new DbParam { Value = "mycursor", DbType = DbType.String, Direction = ParameterDirection.InputOutput } }
            };

            dynamic response = await _pghelper.ListAsync("master.sp_equipment_category_getbyid", Params);
            dynamic obj = response.@ref[0];

            return new EquipmentCategoryResponseViewModel
            {
                category_id = obj.category_id,
                organization_id = obj.organization_id,
                category_name = obj.category_name,
                description = obj.description,
                created_date = obj.created_at,
                updated_date = obj.updated_at
            };
        }
    }
}
