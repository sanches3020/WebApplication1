document.addEventListener('DOMContentLoaded', () => {
    const themeToggle = document.querySelector('.theme-toggle');
    const currentTheme = localStorage.getItem('theme');

    // Apply saved theme on page load
    if (currentTheme) {
        document.body.classList.add(currentTheme + '-theme'); // Assuming themes are named 'dark-theme', 'light-theme'
        if (currentTheme === 'dark' && themeToggle) {
            themeToggle.classList.add('active');
        }
    }

    // Add event listener to theme toggle
    if (themeToggle) {
        themeToggle.addEventListener('click', () => {
            themeToggle.classList.toggle('active');

            // Toggle a class on the body element to apply dark theme styles
            document.body.classList.toggle('dark-theme');

            // Save theme preference to localStorage
            if (document.body.classList.contains('dark-theme')) {
                localStorage.setItem('theme', 'dark');
            } else {
                localStorage.setItem('theme', 'light');
            }
        });
    }

    // Emotions Page Interaction
    const emotionItems = document.querySelectorAll('.emotion-item');

    emotionItems.forEach(item => {
        item.addEventListener('click', () => {
            item.classList.toggle('selected');
        });
    });

    // Notes Page Interaction
    const addNoteButton = document.querySelector('.add-note-button');
    const notesList = document.querySelector('.notes-list');
    const saveAllNotesButton = document.querySelector('.save-button'); // The 'СОХРАНИТЬ' button at the bottom

    // --- Notes Data Handling --- //
    const notesStorageKey = 'userNotes';

    // Load notes from localStorage
    const loadNotes = () => {
        const savedNotes = localStorage.getItem(notesStorageKey);
        if (savedNotes) {
            return JSON.parse(savedNotes);
        }
        return []; // Return an empty array if no notes are saved
    };

    // Save notes to localStorage
    const saveNotes = (notesData) => {
        localStorage.setItem(notesStorageKey, JSON.stringify(notesData));
    };

    // Create a note card element with a textarea and content
    const createNoteCardElement = (content = '') => {
        const newNoteCard = document.createElement('div');
        newNoteCard.classList.add('note-card');

        const noteTextarea = document.createElement('textarea');
        noteTextarea.placeholder = 'Введите текст заметки...';
        noteTextarea.value = content; // Set content if provided

        // Add input event listener for auto-saving
        noteTextarea.addEventListener('input', saveAllNotes);

        newNoteCard.appendChild(noteTextarea);

        return newNoteCard;
    };

    // Render notes to the page
    const renderNotes = (notesData) => {
        notesList.innerHTML = ''; // Clear current notes
        notesData.forEach(noteContent => {
            const noteElement = createNoteCardElement(noteContent);
            notesList.appendChild(noteElement);
        });
    };

    // Save all notes currently on the page
    const saveAllNotes = () => {
        const currentNotes = Array.from(notesList.querySelectorAll('.note-card textarea'))
            .map(textarea => textarea.value);
        saveNotes(currentNotes);
    };

    // --- Initialize Notes Display --- //
    if (notesList) {
        const initialNotes = loadNotes();
        if (initialNotes.length > 0) {
            renderNotes(initialNotes);
        } else {
            // If no notes are saved, maybe add a few empty ones initially?
            // Or leave it empty and let the user add the first one.
            // For now, let's clear any existing placeholders in HTML and rely on JS.
            notesList.innerHTML = ''; // Ensure list is empty if loading no notes
        }
    }

    // --- Event Listeners --- //
    if (addNoteButton && notesList) {
        addNoteButton.addEventListener('click', () => {
            const newNoteCard = createNoteCardElement(); // Create an empty new note
            notesList.appendChild(newNoteCard);
            saveAllNotes(); // Save all notes including the new one
        });
    }

    // Add event listener to the main 'СОХРАНИТЬ' button (optional, auto-save on input is active)
    if (saveAllNotesButton) {
        saveAllNotesButton.addEventListener('click', () => {
            saveAllNotes();
            alert('Заметки сохранены!'); // Optional feedback
        });
    }

    // Calendar Page Interaction
    const calendarDaysGrid = document.querySelector('.calendar-days');
    const currentMonthDisplay = document.querySelector('.current-month');
    const prevMonthButton = document.querySelector('.calendar-header button:first-child');
    const nextMonthButton = document.querySelector('.calendar-header button:last-child');
    const dayEmotionsSelector = document.querySelector('.day-emotions-selector');
    const saveDayEmotionsButton = dayEmotionsSelector ? dayEmotionsSelector.querySelector('.save-day-emotions') : null;
    const emotionsGridCalendar = dayEmotionsSelector ? dayEmotionsSelector.querySelector('.emotions-grid') : null;
    const emotionItemsCalendar = emotionsGridCalendar ? emotionsGridCalendar.querySelectorAll('.emotion-item') : [];

    let selectedDayElement = null;
    let currentCalendarDate = new Date(); // Start with the current date

    // --- Calendar Generation and Navigation --- //
    const renderCalendar = () => {
        currentCalendarDate.setDate(1); // Set to the first day of the month

        const year = currentCalendarDate.getFullYear();
        const month = currentCalendarDate.getMonth();

        // Get the first day of the month (0 = Sunday, 6 = Saturday)
        const firstDayOfMonth = currentCalendarDate.getDay();
        // Adjust so Monday is 0, Sunday is 6
        const startWeekday = (firstDayOfMonth === 0) ? 6 : firstDayOfMonth - 1;

        // Get the number of days in the current month
        const daysInMonth = new Date(year, month + 1, 0).getDate();

        // Get the number of days in the previous month
        const daysInPrevMonth = new Date(year, month, 0).getDate();

        // Clear previous days
        calendarDaysGrid.innerHTML = '';

        // Add days from the previous month
        for (let i = startWeekday; i > 0; i--) {
            const day = daysInPrevMonth - i + 1;
            const dayElement = document.createElement('div');
            dayElement.classList.add('other-month');
            dayElement.innerHTML = `<span class="day-number">${day}</span>`;
            // We don't add event listeners or data attributes for other months for simplicity
            calendarDaysGrid.appendChild(dayElement);
        }

        // Add days from the current month
        for (let i = 1; i <= daysInMonth; i++) {
            const dayElement = document.createElement('div');
            const dayNumberSpan = document.createElement('span');
            dayNumberSpan.classList.add('day-number');
            dayNumberSpan.textContent = i;
            dayElement.appendChild(dayNumberSpan);

            // Add data attribute for the date (YYYY-MM-DD format)
            const dateString = `${year}-${(month + 1).toString().padStart(2, '0')}-${i.toString().padStart(2, '0')}`;
            dayElement.dataset.date = dateString;

            // Check if it's the current day
            const today = new Date();
            if (year === today.getFullYear() && month === today.getMonth() && i === today.getDate()) {
                dayElement.classList.add('current-day');
            }

            // Load and display saved emotions for this day
            const dayKey = dateString; // Use the date string as the key
            const savedEmotions = loadCalendarEmotions()[dayKey];
            if (savedEmotions) {
                displayDayEmotions(dayElement, savedEmotions);
            }

            calendarDaysGrid.appendChild(dayElement);
        }

        // Add days from the next month (fill the last row)
        const totalCells = calendarDaysGrid.children.length;
        const remainingCells = 42 - totalCells; // 6 rows * 7 days = 42 cells

        for (let i = 1; i <= remainingCells; i++) {
            const dayElement = document.createElement('div');
            dayElement.classList.add('other-month');
            dayElement.innerHTML = `<span class="day-number">${i}</span>`;
            calendarDaysGrid.appendChild(dayElement);
        }

        // Update the current month display
        const monthNames = ['Январь', 'Февраль', 'Март', 'Апрель', 'Май', 'Июнь', 'Июль', 'Август', 'Сентябрь', 'Октябрь', 'Ноябрь', 'Декабрь'];
        currentMonthDisplay.textContent = `${monthNames[month]} ${year}`;

        // Re-attach click listeners to the newly created day elements
        attachDayClickListeners();
    };

    // Attach click listeners to calendar days (using event delegation is better for dynamic elements)
    const attachDayClickListeners = () => {
        // Remove any previous listeners to prevent duplicates if renderCalendar is called multiple times
        // However, with event delegation on the grid, we only need one listener on the parent.

        // Add click listener to the calendar grid parent (event delegation)
        calendarDaysGrid.removeEventListener('click', handleDayClick);
        calendarDaysGrid.addEventListener('click', handleDayClick);
    };

    const handleDayClick = (event) => {
        const dayElement = event.target.closest('.calendar-days div:not(.other-month)');

        if (dayElement) {
            selectedDayElement = dayElement;
            const dateString = dayElement.dataset.date;

            // Show the emotion selector
            if (dayEmotionsSelector) {
                dayEmotionsSelector.style.display = 'block';

                // Reset selected emotions in the selector
                emotionItemsCalendar.forEach(item => item.classList.remove('selected'));

                // Load previously saved emotions for this day into the selector
                const calendarEmotionsData = loadCalendarEmotions();
                if (calendarEmotionsData[dateString]) {
                    const savedDayEmotions = calendarEmotionsData[dateString];
                    emotionItemsCalendar.forEach(item => {
                        if (savedDayEmotions.includes(item.textContent)) {
                            item.classList.add('selected');
                        }
                    });
                }
            }
        }
    };

    // Event listeners for month navigation buttons
    if (prevMonthButton) {
        prevMonthButton.addEventListener('click', () => {
            currentCalendarDate.setMonth(currentCalendarDate.getMonth() - 1);
            renderCalendar();
        });
    }

    if (nextMonthButton) {
        nextMonthButton.addEventListener('click', () => {
            currentCalendarDate.setMonth(currentCalendarDate.getMonth() + 1);
            renderCalendar();
        });
    }

    // --- Calendar Data Handling --- //
    // Use a key specific to calendar emotions storage
    const calendarStorageKey = 'calendarEmotions';

    // Load emotions from localStorage
    const loadCalendarEmotions = () => {
        const savedEmotions = localStorage.getItem(calendarStorageKey);
        if (savedEmotions) {
            try {
                return JSON.parse(savedEmotions);
            } catch (e) {
                console.error('Error parsing calendar emotions from localStorage:', e);
                return {};
            }
        }
        return {};
    };

    // Save emotions to localStorage
    const saveCalendarEmotions = (emotionsData) => {
        localStorage.setItem(calendarStorageKey, JSON.stringify(emotionsData));
    };

    // Display emotions in the calendar grid element
    const displayDayEmotions = (dayElement, emotions) => {
        // Remove previous emotions first
        const existingEmotionsSpan = dayElement.querySelector('.day-emotions');
        if (existingEmotionsSpan) {
            existingEmotionsSpan.remove();
        }

        if (emotions && emotions.length > 0) {
            const emotionsSpan = document.createElement('span');
            emotionsSpan.classList.add('day-emotions');
            emotionsSpan.textContent = emotions.join(' '); // Join emojis with space
            dayElement.appendChild(emotionsSpan);
        }
    };

    // Add click listener to the save emotions button
    if (saveDayEmotionsButton) {
        saveDayEmotionsButton.addEventListener('click', () => {
            if (selectedDayElement) {
                // Get selected emotions
                const selectedEmotions = Array.from(emotionItemsCalendar)
                    .filter(item => item.classList.contains('selected'))
                    .map(item => item.textContent);

                // Get the date key for the selected day
                const dateString = selectedDayElement.dataset.date;
                if (dateString) {
                    const calendarEmotionsData = loadCalendarEmotions();
                    // Update the emotions data object and save to localStorage
                    calendarEmotionsData[dateString] = selectedEmotions;
                    saveCalendarEmotions(calendarEmotionsData);

                    // Display selected emotions in the calendar day
                    displayDayEmotions(selectedDayElement, selectedEmotions);

                    // Hide the emotion selector
                    dayEmotionsSelector.style.display = 'none';
                    selectedDayElement = null; // Reset selected day
                }
            }
        });
    }

    // Close the selector if clicking outside
    // Use event delegation on the body or a higher container
    document.body.addEventListener('click', (event) => {
        const isClickInsideSelector = dayEmotionsSelector && dayEmotionsSelector.contains(event.target);
        const isClickOnCalendarDay = event.target.closest('.calendar-days div');

        // Hide the selector if it's visible and the click is outside the selector and not on a calendar day
        if (dayEmotionsSelector && dayEmotionsSelector.style.display !== 'none' && !isClickInsideSelector && !isClickOnCalendarDay) {
            dayEmotionsSelector.style.display = 'none';
            // We don't reset selectedDayElement here immediately, it will be set on the next day click
        }
    });


    // --- Initial Render --- //
    if (calendarDaysGrid && currentMonthDisplay) {
        renderCalendar();
    }

    // Statistics Page Interaction
    const saveStatisticsButton = document.querySelector('#statistics-page .save-button');

    if (saveStatisticsButton) {
        saveStatisticsButton.addEventListener('click', () => {
            alert('Функция сохранения статистики пока не реализована.'); // Simple feedback
        });
    }
}); 