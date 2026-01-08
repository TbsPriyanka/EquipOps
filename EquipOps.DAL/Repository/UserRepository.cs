using Dapper;
using EquipOps.Common.Helper;
using EquipOps.DAL.Interfaces;
using EquipOps.Model.User;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Npgsql;
using System.Data;
using System.Security.Claims;

namespace EquipOps.DAL.Repository
{
    public class UserRepository(ILogger<UserRepository> _logger, IDbConnectionFactory _dbFactory, IHttpContextAccessor contextAccessor) : IUserRepository
    {
        public async Task<UserCreateUpdateResponse> AddOrUpdateUser(UserCreateUpdateRequest request)
        {
            using var connection = _dbFactory.CreateConnection();

            var userIdClaim = contextAccessor.HttpContext?.User?
                .FindFirst(ClaimTypes.NameIdentifier)?.Value;

            Guid? updatedBy = string.IsNullOrEmpty(userIdClaim)
                ? null
                : Guid.Parse(userIdClaim);

            string? hashedPassword = null;
            if (!string.IsNullOrWhiteSpace(request.Password))
            {
                hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);
            }

            var parameters = new DynamicParameters();
            parameters.Add("p_id", request.Id);
            parameters.Add("p_first_name", request.FirstName);
            parameters.Add("p_last_name", request.LastName);
            parameters.Add("p_email", request.Email);
            parameters.Add("p_phone_number", request.PhoneNumber);
            parameters.Add("p_organization_id", request.OrganizationId);
            parameters.Add("p_role_id", request.RoleId);
            parameters.Add("p_password", hashedPassword); 
            parameters.Add("p_updated_by", updatedBy);
            parameters.Add("o_user_id", dbType: DbType.Guid, direction: ParameterDirection.Output);

            await connection.ExecuteAsync(
                "master.sp_user_create_update",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return new UserCreateUpdateResponse
            {
                Id = parameters.Get<Guid>("o_user_id"),
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                OrganizationId = request.OrganizationId,
                RoleId = request.RoleId
            };
        }

        public async Task<UserGetResponse?> GetByIdAsync(Guid userId)
        {
            if (userId == Guid.Empty)
                return null;

            using var conn = _dbFactory.CreateConnection();
            conn.Open();

            using var tran = conn.BeginTransaction();

            await using var cmd = new NpgsqlCommand(
                "CALL master.sp_user_get_by_id(@p_user_id, @ref)",
                (NpgsqlConnection)conn
            );
            cmd.Transaction = (NpgsqlTransaction)tran;

            cmd.Parameters.AddWithValue("p_user_id", userId);
            cmd.Parameters.Add(new NpgsqlParameter("ref", NpgsqlTypes.NpgsqlDbType.Refcursor)
            {
                Direction = ParameterDirection.InputOutput,
                Value = "my_cursor"
            });

            await cmd.ExecuteNonQueryAsync();

            var result = conn.QueryFirstOrDefault<UserGetResponse>(
                "FETCH ALL IN \"my_cursor\"",
                transaction: tran
            );

            tran.Commit();
            return result;
        }
        public async Task<UserListResponseViewModel> GetUserListAsync(string? Search, int Length, int Page, string OrderColumn, string OrderDirection = "Asc", string role = "", bool? isActive = null)
        {
            using var conn = _dbFactory.CreateConnection();
            conn.Open();

            using var tran = conn.BeginTransaction();

            using var cmd = new NpgsqlCommand(
                $"CALL {StoreProcedure.GetUserList}(" +
                "@p_search, @p_length, @p_page, @p_order_column, @p_order_direction, " +
                "@p_role, @p_is_active, @o_total_numbers, @ref)",
                (NpgsqlConnection)conn);

            cmd.Transaction = (NpgsqlTransaction)tran;

            cmd.Parameters.AddWithValue("p_search", (object?)Search ?? DBNull.Value);
            cmd.Parameters.AddWithValue("p_length", Length);
            cmd.Parameters.AddWithValue("p_page", Page);
            cmd.Parameters.AddWithValue("p_order_column", OrderColumn);
            cmd.Parameters.AddWithValue("p_order_direction", OrderDirection);
            cmd.Parameters.AddWithValue("p_role", role);
            cmd.Parameters.AddWithValue("p_is_active", (object?)isActive ?? DBNull.Value);

            var totalParam = new NpgsqlParameter("o_total_numbers", NpgsqlTypes.NpgsqlDbType.Integer)
            {
                Direction = ParameterDirection.InputOutput,
                Value = 0
            };
            cmd.Parameters.Add(totalParam);

            var cursorParam = new NpgsqlParameter("ref", NpgsqlTypes.NpgsqlDbType.Refcursor)
            {
                Direction = ParameterDirection.InputOutput,
                Value = "my_cursor"
            };
            cmd.Parameters.Add(cursorParam);

            await cmd.ExecuteNonQueryAsync();

            var users = conn.Query<UserGetResponse>("FETCH ALL IN \"my_cursor\"", transaction: tran).ToList();

            tran.Commit();

            return new UserListResponseViewModel
            {
                TotalNumbers = (int)totalParam.Value,
                UserData = users
            };

        }
        public async Task DeleteUserAsync(UserDeleteRequest request)
        {
            _logger.LogInformation("Deleting User With UserId={UserId}", request.Id);

            var userIdClaim = contextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var updatedBy = string.IsNullOrWhiteSpace(userIdClaim) ? (Guid?)null : Guid.Parse(userIdClaim);

            using var conn = _dbFactory.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("p_id", request.Id);
            parameters.Add("p_updated_by", updatedBy);

            await conn.ExecuteAsync(
                StoreProcedure.DeleteUser,
                parameters,
                commandType: CommandType.StoredProcedure
            );
        }
    }
}
