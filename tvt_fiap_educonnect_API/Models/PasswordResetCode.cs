namespace EduConnect_API.Models
{
    public class PasswordResetCode
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Codigo { get; set; } = string.Empty;
        public DateTime ExpiraEm { get; set; }
        public bool Usado { get; set; } = false;
    }
}