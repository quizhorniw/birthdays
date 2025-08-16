import './tools.js'

document.getElementById('addEmailButton')?.addEventListener('click', () => {
    const emailAddress = (document.getElementById('email') as HTMLInputElement).value;
    
    fetch(`http://localhost:5070/api/emails?email=${emailAddress}`, {
        method: 'POST'
    })
        .then(response => {
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
        })
        .catch(error => {
            console.error('There was a problem with the upload operation:', error);
        });

    const container = document.getElementById('addEmailContainer');
    if (!container) return;
    
    const p = document.createElement('p');
    p.innerText = 'Вы успешно подписались на рассылку';
    
    container.appendChild(p);
})