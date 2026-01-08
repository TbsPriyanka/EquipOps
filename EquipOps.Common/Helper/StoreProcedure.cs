namespace EquipOps.Common.Helper
{
    public static class StoreProcedure
    {
        public const string UserCreateUpdate = "master.sp_user_create_update";
        public const string GetUserList = "master.sp_user_list_get";
        public const string GetUserById = "master.sp_user_get_by_id";
        public const string DeleteUser = "master.sp_user_delete";

        public const string UserLogin = "master.sp_user_login";
        public const string AddUserToken = "master.sp_user_token_create";
    }
}
