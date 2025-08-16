import {Birthday} from "./model.js";
import {formatBirthday} from "./tools.js";

function fetchBirthdays() : void {
    fetch('http://localhost:5070/api/birthdays')
        .then(response => {
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            return response.json();
        })
        .then((data: Birthday[]) => {
            const upcomingBirthdays = sortUpcomingBirthdays(filterUpcomingBirthdays(data));
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

function sortUpcomingBirthdays(birthdays: Birthday[]): Birthday[] {
    const today = new Date();

    return birthdays.map(birthday => {
        const birthdayDate = new Date(birthday.birthDay);
        const nextOccurrence = new Date(
            birthdayDate.getMonth() < today.getMonth() ? today.getFullYear() + 1 : today.getFullYear(),
            birthdayDate.getMonth(),
            birthdayDate.getDay()
        );

        return {
            ...birthday,
            nextOccurrence: nextOccurrence
        };
    }).sort((a, b) => a.nextOccurrence.getTime() - b.nextOccurrence.getTime());
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
        card.className = 'card';

        const birthdayDate = new Date(birthday.birthDay);
        const photoFileName = '../images/' + (birthday.photoFileName ? birthday.photoFileName : 'default-card.jpg');
        card.innerHTML = `
            <img src="${photoFileName}" alt="Изображение" style="max-width: 200px;">
            <h3>${birthday.firstName} ${birthday.lastName}</h3>
            <p><i class="fas fa-birthday-cake"></i> ${formatBirthday(birthdayDate)}</p>
        `;

        card.addEventListener('click', () => {
            window.location.href = `card.html?id=${birthday.id}`;
        });

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

window.onload = fetchBirthdays;