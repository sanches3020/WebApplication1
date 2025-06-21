document.addEventListener("DOMContentLoaded", () => {
    const container = document.getElementById("notesContainer");
    const addBtn = document.getElementById("addNote");
    const saveBtn = document.getElementById("saveNotes");
    const searchInput = document.getElementById("noteSearch");

    // Добавление новой карточки
    addBtn.addEventListener("click", () => {
        const card = createNoteCard();
        container.appendChild(card);
    });

    // Поиск
    searchInput.addEventListener("input", () => {
        const query = searchInput.value.toLowerCase();
        const cards = container.querySelectorAll(".note-card");
        cards.forEach(card => {
            const text = card.querySelector("textarea").value.toLowerCase();
            card.style.display = text.includes(query) ? "block" : "none";
        });
    });

    // Сохранение (заготовка)
    saveBtn.addEventListener("click", () => {
        const notes = [];
        const cards = container.querySelectorAll(".note-card textarea");
        cards.forEach(textarea => {
            const value = textarea.value.trim();
            if (value) notes.push(value);
        });

        console.log("Сохраняем заметки:", notes);
        // TODO: отправить на сервер
    });

    // Стартовая карточка
    addBtn.click();
});
