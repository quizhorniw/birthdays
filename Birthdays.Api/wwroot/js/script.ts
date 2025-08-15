window.onload = fetchBirthdays;

function fetchBirthdays() : void {
    fetch('http://localhost:5070/api/birthdays')
        .then(response => {
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            return response.json();
        })
        .then(data => {
            const upcomingBirthdays = filterUpcomingBirthdays(data);
            displayBirthdays(upcomingBirthdays);
        })
        .catch(error => {
            console.error('There was a problem with the fetch operation:', error);
        });
}

function filterUpcomingBirthdays(birthdays: Birthday[]): Birthday[] {
    const today = new Date();
    const twoWeeksFromNow = new Date();
    twoWeeksFromNow.setDate(today.getDate() + 14);

    return birthdays.filter(birthday => {
        const birthdayDate = new Date(birthday.birthDay);
        return isBirthdayInNextTwoWeeks(new Date, birthdayDate);
    });
}

function displayBirthdays(birthdays: Birthday[]): void {
    const container = document.getElementById('birthday-container');
    if (!container) return;

    container.innerHTML = '';

    if (birthdays.length === 0) {
        container.innerHTML = '<p>В ближайшие 2 недели не предстоит никаких дней рождения.</p>';
        return;
    }

    birthdays.forEach(birthday => {
        const card = document.createElement('div');
        card.className = 'card'; // Add the card class

        const birthdayDate = new Date(birthday.birthDay);
        card.innerHTML = `
            <p>${birthday.firstName} ${birthday.lastName}</p>
            <p><i class="fas fa-birthday-cake"></i> ${formatBirthday(birthdayDate)}</p>
        `;

        container.appendChild(card);
    });
}

function isBirthdayInNextTwoWeeks(today: Date, birthDay: Date) : boolean {
    const birthdayThisYear = new Date(today.getFullYear(), birthDay.getMonth(), birthDay.getDate());
    const birthdayNextYear = new Date(today.getFullYear() + 1, birthDay.getMonth(), birthDay.getDate());

    const twoWeeksFromNow = new Date(today.valueOf());
    twoWeeksFromNow.setDate(today.getDate() + 14);
    
    return (birthdayThisYear >= today && birthdayThisYear <= twoWeeksFromNow) || 
        (birthdayNextYear >= today && birthdayNextYear <= twoWeeksFromNow);
}

function formatBirthday(date: Date) : string {
    const options: Intl.DateTimeFormatOptions = { day: 'numeric', month: 'long' };
    return date.toLocaleDateString('ru-RU', options);
}

interface Birthday {
    id: number;
    firstName: string;
    lastName: string;
    birthDay: string;
    photoPath?: string | undefined;
}