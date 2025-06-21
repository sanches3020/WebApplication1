namespace MoodMapper.Configurations
{
    /// <summary>
    /// Класс настроек для экспорта и импорта данных в формате JSON.
    /// Значения этих настроек будут загружаться из секции "JsonSettings" файла appsettings.json.
    /// </summary>
    public class JsonSettings
    {
        /// <summary>
        /// Путь к файлу, в который будут экспортироваться данные.
        /// </summary>
        public required string ExportFilePath { get; set; }

        /// <summary>
        /// Путь к файлу, из которого будут импортироваться данные.
        /// </summary>
        public required string ImportFilePath { get; set; }
    }
}
