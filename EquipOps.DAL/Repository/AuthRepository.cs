using Dapper;
using EquipOps.Common.Helper;
using EquipOps.DAL.Interfaces;
using EquipOps.Model.AuthLogin;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Data;
using System.Text.Json;

namespace EquipOps.DAL.Repository
{
    public class AuthRepository (IDbConnectionFactory _dbFactory, ILogger<AuthRepository> _logger, JwtTokenHelper _jwtTokenHelper, IHttpContextAccessor _httpContextAccessor, JwtSettings _jwtSettings) : IAuthRepository
    {
        public async Task<AuthLoginResponseViewModel> UserLoginAsync(AuthLoginRequestViewModel request)
        {
            _logger.LogInformation("Executing UserLogin Stored Procedure For Email={Email}", request.Email);

            using var conn = _dbFactory.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("p_email", request.Email);
            parameters.Add("user_id", dbType: DbType.Guid, direction: ParameterDirection.Output);
            parameters.Add("role_id", dbType: DbType.Guid, direction: ParameterDirection.Output);
            parameters.Add("role_name", dbType: DbType.String, size: 200, direction: ParameterDirection.Output);
            parameters.Add("full_name", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);
            parameters.Add("hash_password", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);

            await conn.ExecuteAsync(
                "master.sp_user_login",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            var data = new AuthLoginResponseViewModel
            {
                UserId = parameters.Get<Guid?>("user_id"),
                RoleId = parameters.Get<Guid?>("role_id"),
                RoleName = parameters.Get<string?>("role_name"),
                FullName = parameters.Get<string?>("full_name") ?? "",
                HashPassword = parameters.Get<string?>("hash_password") ?? "",
                Email = request.Email
            };

            if (data.UserId is not null)
            {
                var token = _jwtTokenHelper.GenerateToken(
                    userId: data.UserId.Value.ToString(),
                    email: data.Email,
                    role: data.RoleName ?? "",
                    organizationId: null,
                    jwtSettings: _jwtSettings
                );

                if (string.IsNullOrWhiteSpace(token))
                    throw new InvalidOperationException("JWT Generation Failed.");

                var payloadJson = JsonSerializer.Serialize(new
                {
                    data.UserId,
                    data.Email
                });

                await AddUserToken(new UserTokenRequestViewModel
                {
                    User_Id = data.UserId,
                    Email = data.Email,
                    Token = token,
                    Token_data = payloadJson,
                    Token_type = "UserLogin",
                    Token_expiry = DateTime.UtcNow.AddHours(24),
                    Ip_address = _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString() ?? "0.0.0.0"
                });

                data.Token = token;
            }

            return data;
        }

        public async Task<bool> AddUserToken(UserTokenRequestViewModel user)
        {
            using var conn = _dbFactory.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("user_id", user.User_Id);
            parameters.Add("email", user.Email);
            parameters.Add("token", user.Token);
            parameters.Add("token_data", user.Token_data);
            parameters.Add("token_type", user.Token_type);
            parameters.Add("token_expiry", user.Token_expiry);
            parameters.Add("status", user.Status.ToString());
            parameters.Add("created_by", user.User_Id);
            parameters.Add("ip_address", user.Ip_address);
            parameters.Add("out_user_token_id", dbType: DbType.Guid, direction: ParameterDirection.Output);

            var result = await conn.ExecuteAsync(StoreProcedure.AddUserToken,parameters,commandType: CommandType.StoredProcedure);

            return result > 0;
        }
        public async Task<UserResponse?> GetUserByEmailAsync(string email)
        {
            using var conn = _dbFactory.CreateConnection();

            _logger.LogInformation("Fetching User By Email: {Email}", email);

            const string query = @"
            SELECT 
                id AS user_id,
                email,
                first_name,
                last_name,
                hash_password
            FROM master.""user""
            WHERE email = @Email
            LIMIT 1;
        ";

            var user = await conn.QueryFirstOrDefaultAsync<UserResponse>(query, new { Email = email });

            if (user == null)
            {
                _logger.LogWarning("User Not Found For Email: {Email}", email);
            }
            else
            {
                _logger.LogInformation("User Found: {Email}", email);
            }

            return user;
        }
        public async Task<bool> SaveResetTokenAsync(UserResponse userInfo, string resetToken)
        {
            using var conn = _dbFactory.CreateConnection();

            var ip = _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString()
                     ?? "0.0.0.0";

            await conn.ExecuteAsync(
                    "SELECT master.fn_save_reset_token(@p_user_id, @p_email, @p_token, @p_ip_address)",
                    new
                    {
                        p_user_id = userInfo.user_id,
                        p_email = userInfo.email,
                        p_token = resetToken,
                        p_ip_address = ip
                    }
                );

            return true;
        }
        public async Task<UserTokenResponse?> GetUserByResetTokenAsync(string token)
        {
            _logger.LogInformation("Fetching User By Reset Token: {Token}", token);

            using var conn = _dbFactory.CreateConnection();

            const string query = @"
                                    SELECT 
                                        user_id, 
                                        email,
                                        expired_at
                                    FROM master.user_tokens
                                    WHERE token = @Token 
                                      AND is_used = false";

            var user = await conn.QueryFirstOrDefaultAsync<UserTokenResponse>(query, new { Token = token });

            if (user == null)
            {
                _logger.LogWarning("No User Found For Reset Token: {Token}", token);
            }
            else
            {
                _logger.LogInformation("User Found For Reset Token. UserId: {UserId}, Email: {Email}",
                                       user.user_id, user.email);
            }

            return user;
        }
        public async Task<bool> UpdatePasswordAsync(string token, string hashedPassword, UserTokenResponse user)
        {
            using var conn = _dbFactory.CreateConnection();

            _logger.LogInformation("Starting Password Update For UserId: {UserId}, Email: {Email}",
                                   user.user_id, user.email);

            const string updateTokenQuery = @"
                    UPDATE master.user_tokens
                    SET 
                        is_used = true,
                        expired_at = NULL,
                        updated_date = NOW()
                    WHERE token = @Token";

            var affectedTokenRows = await conn.ExecuteAsync(updateTokenQuery, new { Token = token });

            _logger.LogInformation("Token Update Affected Rows: {Rows}", affectedTokenRows);


            const string updatePasswordQuery = @"
            UPDATE master.user
            SET hash_password = @HashedPassword
            WHERE id = @UserId";

            var affectedUserRows = await conn.ExecuteAsync(updatePasswordQuery, new
            {
                UserId = user.user_id,
                HashedPassword = hashedPassword
            });


            _logger.LogInformation("Password Updated For UserId: {UserId}, RowsAffected: {Rows}",
                                    user.user_id, affectedUserRows);

            return affectedUserRows > 0;
        }
        public async Task<bool> ChangePasswordAsync(Guid userId, string hashedPassword)
        {
            _logger.LogInformation("ChangePasswordAsync started for UserId={UserId}", userId);

            using var conn = _dbFactory.CreateConnection();

            const string query = @"CALL master.sp_change_password(@p_user_id, @p_new_password)";

            _logger.LogInformation("Executing stored procedure sp_change_password for UserId={UserId}", userId);

            await conn.ExecuteAsync(query, new
            {
                p_user_id = userId,
                p_new_password = hashedPassword
            });

            return true;
        }
        public async Task<bool> LogoutAsync(Guid? userId)
        {
            using var conn = _dbFactory.CreateConnection();

            _logger.LogInformation("Executing Logout For UserId={UserId}", userId);

            const string query = @"
            UPDATE master.user_tokens
            SET is_used = TRUE,
                updated_by = @UserId,
                token_type = 'UserLogout',
                is_delete = TRUE,
                updated_date = NOW()
            WHERE user_id = @UserId
              AND token_type='UserLogin'
              AND is_delete = FALSE
              AND is_used = FALSE;
        ";

            var affected = await conn.ExecuteAsync(query, new { UserId = userId });

            if (affected > 0)
            {
                _logger.LogInformation("Logout Successful — Token Invalidated For UserId={UserId}", userId);
                return true;
            }

            _logger.LogWarning("Logout failed — No Active Token Found For UserId={UserId}", userId);
            return false;
        }
    }
}