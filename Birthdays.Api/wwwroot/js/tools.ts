export function formatBirthday(date: Date) : string {
    const options: Intl.DateTimeFormatOptions = { day: 'numeric', month: 'long' };
    return date.toLocaleDateString('ru-RU', options);
}

document.getElementById('back-button')?.addEventListener('click', () => {
    window.history.back();
});

document.getElementById('add-button')?.addEventListener('click', () => {
    window.location.href = `create.html`;
});

document.getElementById('add-email')?.addEventListener('click', () => {
    window.location.href = `subscribe.html`;
})