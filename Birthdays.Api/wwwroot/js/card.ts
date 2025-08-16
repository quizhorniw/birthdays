import {Birthday, UpdateBirthday} from "./model.js";
import {formatBirthday} from "./tools.js";

const urlParams = new URLSearchParams(window.location.search);
const birthdayId: string | null = urlParams.get('id');

function fetchBirthdayDetails(): void {
    if (!birthdayId) {
        console.error('Birthday ID is missing in the URL.');
        return;
    }

    fetch(`http://localhost:5070/api/birthdays/${birthdayId}`)
        .then(response => {
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            return response.json();
        })
        .then((data: Birthday) => {
            displayBirthdayDetails(data);
        })
        .catch(error => {
            console.error('There was a problem with the fetch operation:', error);
        });
}

function displayBirthdayDetails(birthday: Birthday): void {
    const infoContainer = document.getElementById('card-info');
    if (!infoContainer) return;
    
    document.title = `${birthday.firstName} ${birthday.lastName}`;
    
    const birthdayDate = new Date(birthday.birthDay);
    const photoFileName = '../images/' + (birthday.photoFileName ? birthday.photoFileName : 'default-card.jpg');
    infoContainer.innerHTML = `
        <div style="position: relative;">
            <h2 id="name">${birthday.firstName} ${birthday.lastName}</h2>
            <button id="edit-card"><i class="fa-regular fa-edit" style="font-size: 20px"></i></button>
            <p id="birthday-date"><i class="fas fa-birthday-cake"></i> ${formatBirthday(birthdayDate)}</p>
        </div>
        <img src="${photoFileName}" alt="Изображение" style="max-width: 200px;">
        <div><button id="delete-card"><i class="fa-regular fa-trash-can"></i></button></div>
    `;
    
    const editCardButton = document.getElementById('edit-card');
    editCardButton?.addEventListener('click', function(){
        const nameElement = document.getElementById('name');
        const birthdayElement = document.getElementById('birthday-date');
        if (!nameElement || !birthdayElement) return;
        
        const nameInput = `<input type="text" id="name-input" value="${nameElement.innerText}" style="width: 100%;">`;
        const birthdayInput = `
              <input 
                type="text" 
                id="birthday-input" 
                value="" 
                style="width: 100%;" 
                placeholder="__.__" 
                pattern="\\d{2}\\.\\d{2}" 
                maxlength="5"
                oninput="this.value = this.value.replace(/[^\\d.]/g, '').replace(/(\\..*?)\\..*/g, '$1').replace(/(\\d{2})(\\d)/, '$1.$2');"
              >
        `;

        nameElement.outerHTML = nameInput;
        birthdayElement.outerHTML = birthdayInput;
        editCardButton.outerHTML = 
            `<button id="save-button"><i class="fa-regular fa-save" style="font-size: 20px"></i></button>`;

        document.getElementById('save-button')!.addEventListener('click', function() {
            const newName = (document.getElementById('name-input') as HTMLInputElement).value;
            const newBirthday = (document.getElementById('birthday-input') as HTMLInputElement).value;
            
            const [firstName, lastName] = newName.split(' ');
            const [day, month] = newBirthday.split('.');
            const date = new Date(new Date().getFullYear(), Number.parseInt(month) - 1, Number.parseInt(day) + 1);
            const formattedDate = date.toISOString().split('T')[0];
            const body: UpdateBirthday = { firstName: firstName, lastName: lastName, birthDay: formattedDate};
            
            fetch(`http://localhost:5070/api/birthdays/${birthdayId}`, {
                method: 'PUT',
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
                .then(() => {
                    fetchBirthdayDetails();
                })
                .catch(error => {
                    console.error('There was a problem with the fetch operation:', error);
                    alert('Произошла ошибка при обновлении карточки');
                })
        });
    });
    
    document.getElementById('delete-card')?.addEventListener('click', () => {
        const isConfirmed = confirm('Удалить данную карточку?');
        if (!isConfirmed) return;
    
        fetch(`http://localhost:5070/api/birthdays/${birthdayId}`, {
            method: 'DELETE'
        })
            .then(response => {
                if (!response.ok) {
                    throw new Error('Network response was not ok');
                }
            })
            .then(() => {
                window.location.href = `index.html`;
            })
            .catch((error) => {
                console.error('There was a problem with the upload operation:', error);
            })
    });
}

document.getElementById('upload-form')?.addEventListener('submit', event => {
    event.preventDefault();

    const formData = new FormData();
    const photoInput = document.getElementById('photo') as HTMLInputElement;
    if (photoInput.files) {
        formData.append('file', photoInput.files[0]);
    }

    fetch(`http://localhost:5070/api/birthdays/${birthdayId}/upload`, {
        method: 'POST',
        body: formData
    })
        .then(response => {
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
        })
        .then(() => {
            fetchBirthdayDetails();
        })
        .catch(error => {
            console.error('There was a problem with the upload operation:', error);
            const uploadMessage = document.getElementById('upload-message');
            if (!uploadMessage) return;

            uploadMessage.innerHTML=`
                <p><i class="fas fa-exclamation-triangle"></i> Ошибка при загрузке изображения</p>
            `;
        });

    photoInput.value = '';
    
    const span = document.getElementById('file-name');
    if (span) {
        span.textContent = 'Файл не выбран';
    }
});

const fileInput = document.getElementById('photo') as HTMLInputElement;

fileInput?.addEventListener('change', ()=> {
    if (!fileInput.files) return;
    
    const span = document.getElementById('file-name');
    if (!span) return;
    
    span.textContent = fileInput.files.length > 0 ? fileInput.files[0].name : 'Файл не выбран';
});

window.onload = fetchBirthdayDetails;