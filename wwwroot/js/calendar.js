document.addEventListener("DOMContentLoaded", function () {
    const modal = document.querySelector(".modal-overlay");
    const closeBtn = document.querySelector(".modal-close");
    const saveBtn = document.querySelector(".save-day-emotions");
    const selectedDateDisplay = document.getElementById("selected-date");

    // Забираем текущий месяц и год из DOM (можно передавать через data-атрибут, ViewData или скрытое поле)
    const currentYear = parseInt(document.body.dataset.year);
    const currentMonth = parseInt(document.body.dataset.month); // 1-12

    document.querySelectorAll(".calendar-day:not(.disabled)").forEach(day => {
        day.addEventListener("click", function () {
            const dayNum = this.dataset.day;
            const fullDate = `${String(dayNum).padStart(2, '0')}.${String(currentMonth).padStart(2, '0')}.${currentYear}`;

            selectedDateDisplay.textContent = fullDate;
            saveBtn.dataset.date = fullDate;
            saveBtn.dataset.emotion = "";
            modal.classList.add("visible");
        });
    });

    document.querySelectorAll(".emotion-item").forEach(item => {
        item.addEventListener("click", function () {
            const emotion = this.textContent.trim();
            document.querySelectorAll(".emotion-item").forEach(e => e.classList.remove("selected"));
            this.classList.add("selected");
            saveBtn.dataset.emotion = emotion;
        });
    });

    closeBtn.addEventListener("click", () => {
        modal.classList.remove("visible");
        resetModal();
    });

    modal.addEventListener("click", e => {
        if (e.target === modal) {
            modal.classList.remove("visible");
            resetModal();
        }
    });

    saveBtn.addEventListener("click", () => {
        const emotion = saveBtn.dataset.emotion;
        const date = saveBtn.dataset.date;

        if (!emotion || !date) {
            alert("Пожалуйста, выбери день и эмоцию.");
            return;
        }

        fetch("/Mood/SaveMood", {
            method: "POST",
            headers: {
                "Content-Type": "application/x-www-form-urlencoded"
            },
            body: `emotion=${encodeURIComponent(emotion)}&date=${encodeURIComponent(date)}`
        })
            .then(response => {
                if (response.ok) {
                    const shortDay = parseInt(date.split(".")[0]);
                    const targetDay = document.querySelector(`.calendar-day[data-day="${shortDay}"]`);
                    if (targetDay) {
                        let emojiSpan = targetDay.querySelector(".calendar-emoji");
                        if (!emojiSpan) {
                            emojiSpan = document.createElement("span");
                            emojiSpan.className = "calendar-emoji";
                            targetDay.appendChild(emojiSpan);
                        }
                        emojiSpan.textContent = emotion;
                    }
                    modal.classList.remove("visible");
                    resetModal();
                } else {
                    alert("Не удалось сохранить настроение.");
                }
            })
            .catch(error => {
                console.error("Ошибка сохранения:", error);
                alert("Сервер недоступен.");
            });
    });

    function resetModal() {
        selectedDateDisplay.textContent = "...";
        saveBtn.dataset.date = "";
        saveBtn.dataset.emotion = "";
        document.querySelectorAll(".emotion-item").forEach(e => e.classList.remove("selected"));
    }
});
