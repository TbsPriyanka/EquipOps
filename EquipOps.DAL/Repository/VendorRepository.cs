using EquipOps.Common.Helper;
using EquipOps.DAL.Interfaces;
using EquipOps.Model.Vendor;
using System.Data;

namespace EquipOps.DAL.Repository
{
    public class VendorRepository(PgHelper _pghelper) : IVendorRepository
    {
        public async Task<VendorResponseViewModel> VendorCreateAsync(VendorRequest request)
        {
            var param = new Dictionary<string, DbParam>
           {
               { "p_return_vendor_id", new DbParam { DbType = DbType.Int32, Direction = ParameterDirection.InputOutput } },
               { "p_return_updated_at", new DbParam { DbType = DbType.DateTime, Direction = ParameterDirection.InputOutput } },
               { "p_vendor_id", new DbParam { Value = request.vendor_id == 0 ? null : request.vendor_id, DbType = DbType.Int32 } },
               { "p_organization_id", new DbParam { Value = request.organization_id, DbType = DbType.Int32 } },
               { "p_name", new DbParam { Value = request.name, DbType = DbType.String } },
               { "p_contact_name", new DbParam { Value = request.contact_name, DbType = DbType.String } },
               { "p_email", new DbParam { Value = request.email, DbType = DbType.String } },
               { "p_phone", new DbParam { Value = request.phone, DbType = DbType.String } },
               { "p_service_type", new DbParam { Value = request.service_type, DbType = DbType.String } }
           };

            var result = await _pghelper.CreateUpdateAsync("master.sp_vendor_create_update", param);

            return new VendorResponseViewModel
            {
                vendor_id = result.p_return_vendor_id,
                organization_id = request.organization_id,
                name = request.name,
                contact_name = request.contact_name,
                email = request.email,
                phone = request.phone,
                service_type = request.service_type,
                updated_date = result.p_return_updated_at
            };
        }

        public async Task<VendorResponse> VendorListAsync(string? search, int length, int page, string orderColumn, string orderDirection)
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

            dynamic response = await _pghelper.ListAsync("master.sp_vendor_list", Params);

            VendorResponse list = new VendorResponse();
            list.TotalNumbers = response.o_total_records;

            foreach (var row in response.@ref)
            {
                list.vendorResponseViewModel.Add(new VendorResponseViewModel
                {
                    vendor_id = row.vendor_id,
                    organization_id = row.organization_id,
                    name = row.name,
                    contact_name = row.contact_name,
                    email = row.email,
                    phone = row.phone,
                    service_type = row.service_type,
                    created_date = row.created_at,
                    updated_date = row.updated_at
                });
            }
            return list;
        }

        public async Task<VendorDeleteResponseViewModel> VendorDeleteAsync(VendorDeleteRequestViewModel request)
        {
            var param = new Dictionary<string, DbParam>
            {
                { "p_return_vendor_id", new DbParam { DbType = DbType.Int32, Direction = ParameterDirection.InputOutput } },
                { "p_vendor_id", new DbParam { Value = request.vendor_id, DbType = DbType.Int32 } }
            };

            dynamic result = await _pghelper.CreateUpdateAsync("master.sp_vendor_delete", param);

            return new VendorDeleteResponseViewModel
            {
                vendor_id = result.p_return_vendor_id
            };
        }

        public async Task<VendorResponseViewModel> VendorByIdAsync(int? vendor_id)
        {
            var Params = new Dictionary<string, DbParam>
            {
                { "p_vendor_id", new DbParam { Value = vendor_id, DbType = DbType.Int32 } },
                { "ref", new DbParam { Value = "mycursor", DbType = DbType.String, Direction = ParameterDirection.InputOutput } }
            };

            dynamic response = await _pghelper.ListAsync("master.sp_vendor_getbyid", Params);
            dynamic obj = response.@ref[0];

            return new VendorResponseViewModel
            {
                vendor_id = obj.vendor_id,
                organization_id = obj.organization_id,
                name = obj.name,
                contact_name = obj.contact_name,
                email = obj.email,
                phone = obj.phone,
                service_type = obj.service_type,
                created_date = obj.created_at,
                updated_date = obj.updated_at
            };
        }
    }
}
