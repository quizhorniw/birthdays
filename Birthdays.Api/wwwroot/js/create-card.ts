import './tools.js'
import {Birthday, CreateBirthday} from "./model.js";

document.getElementById('create-button')?.addEventListener('click', () => {
    const newName = (document.getElementById('name-input') as HTMLInputElement).value;
    const newBirthday = (document.getElementById('birthday-input') as HTMLInputElement).value;

    const [firstName, lastName] = newName.split(' ');
    const [day, month] = newBirthday.split('.');
    const date = new Date(new Date().getFullYear(), Number.parseInt(month) - 1, Number.parseInt(day) + 1);
    const formattedDate = date.toISOString().split('T')[0];
    const body: CreateBirthday = { firstName: firstName, lastName: lastName, birthDay: formattedDate};

    fetch(`http://localhost:5070/api/birthdays`, {
        method: 'POST',
        body: JSON.stringify(body),
        headers: {
            'Content-Type': 'application/json',
        }
    })
        .then(response => {
            if (!response.ok) {
                console.log(response)
                throw new Error('Network response was not ok');
            }
            return response.json();
        })
        .then((date: Birthday) => {
            window.location.href = `card.html?id=${date.id}`;
        })
        .catch(error => {
            console.error('There was a problem with the fetch operation:', error);
            alert('Произошла ошибка при создании карточки');
        })
});