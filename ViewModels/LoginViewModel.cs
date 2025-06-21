using System.ComponentModel.DataAnnotations;

namespace MoodMapper.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Email обязателен.")]
        [EmailAddress(ErrorMessage = "Неверный формат Email.")]
        public string Email { get; set; } = "t@t.t";

        [Required(ErrorMessage = "Пароль обязателен.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = "12345678"; // То же самое для пароля

        public bool RememberMe { get; set; } = true;

        // Если требуется возвращаемый URL после входа
        public string ReturnUrl { get; set; } = "/";
    }
}
