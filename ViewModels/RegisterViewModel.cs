using System.ComponentModel.DataAnnotations;

namespace MoodMapper.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Email обязателен.")]
        [EmailAddress(ErrorMessage = "Неверный формат Email.")]
        public string Email { get; set; } = "t@t.t";

        [Required(ErrorMessage = "Пароль обязателен.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = "12345678";

        [Required(ErrorMessage = "Подтверждение пароля обязательно.")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Пароли не совпадают.")]
        public string ConfirmPassword { get; set; } = "12345678";

        public string ReturnUrl { get; set; } = "/";
    }
}
