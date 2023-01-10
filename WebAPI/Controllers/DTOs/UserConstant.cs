namespace WebAPI.Controllers.DTOs
{
    // We are not taking data from data base so we get data from constant
    public class UserConstant
    {
        public static List<UserDTO> Users = new()
            {
                    new UserDTO(){ Username="admin",Password="admin1"}
            };
    }
}