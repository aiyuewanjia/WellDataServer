namespace WellDataService.Dtos.RequestDto
{
    public class ChangePasswordRequestDto
    {
        public string UserName { get; set; }
        public string NewPassword { get; set; }
        public string Role { get; set; }
    }
}