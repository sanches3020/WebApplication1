let moodChartInstance = null;
let currentChartType = "bar"; // по умолчанию

document.addEventListener("DOMContentLoaded", () => {
    updateStatistics();

    document.getElementById("startDate").addEventListener("change", updateStatistics);
    document.getElementById("endDate").addEventListener("change", updateStatistics);
    document.getElementById("chartTypeSelector").addEventListener("change", function () {
        currentChartType = this.value;
        updateStatistics();
    });
});

function updateStatistics() {
    const startDate = document.getElementById("startDate").value;
    const endDate = document.getElementById("endDate").value;

    if (!startDate || !endDate) return;

    fetch(`/Statistics/GetMoodStatistics?startDate=${startDate}&endDate=${endDate}`)
        .then(res => res.json())
        .then(data => {
            renderChart(data);
        })
        .catch(() => {
            alert("⚠️ Ошибка при загрузке статистики.");
        });
}

function renderChart(data) {
    const ctx = document.getElementById("moodChart").getContext("2d");
    const labels = data.map(item => item.emotion);
    const counts = data.map(item => item.count);

    if (moodChartInstance) moodChartInstance.destroy();

    const config = {
        type: currentChartType,
        data: {
            labels: labels,
            datasets: [{
                label: "Количество эмоций",
                data: counts,
                backgroundColor: [
                    "#4bc0c0", "#ff9f40", "#9966ff",
                    "#36a2eb", "#ff6384", "#c9cbcf"
                ],
                borderColor: "#333",
                borderWidth: 1
            }]
        },
        options: {
            responsive: true,
            animation: {
                duration: 500,
                easing: "easeOutQuart"
            },
            plugins: {
                legend: {
                    display: currentChartType !== "bar"
                }
            },
            scales: currentChartType === "bar" || currentChartType === "line"
                ? {
                    y: {
                        beginAtZero: true,
                        ticks: { stepSize: 1 }
                    }
                }
                : {}
        }
    };

    moodChartInstance = new Chart(ctx, config);
}
