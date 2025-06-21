document.addEventListener("DOMContentLoaded", () => {
    const toggle = document.querySelector(".theme-toggle");
    const root = document.documentElement;

    // Применяем тему при загрузке
    const savedTheme = localStorage.getItem("theme");
    if (savedTheme === "dark") {
        root.classList.add("dark-theme");
        toggle.classList.add("enabled");
    }

    // Переключение темы
    toggle.addEventListener("click", () => {
        const isDark = root.classList.toggle("dark-theme");
        toggle.classList.toggle("enabled");

        localStorage.setItem("theme", isDark ? "dark" : "light");
    });
});
