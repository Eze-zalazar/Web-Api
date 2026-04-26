/**
 * Filtra las tarjetas de eventos basándose en el texto de búsqueda.
 * @param {string} query - El texto que escribe el usuario.
 * @param {HTMLElement} container - El div que contiene todas las Cards (#events-grid).
 */
export const filterEvents = (query, container) => {
    const searchTerm = query.toLowerCase().trim();
    const cards = container.querySelectorAll('.bg-white.rounded-2xl'); // Seleccionamos las cards por su clase base

    cards.forEach(card => {
        // Buscamos el título del evento dentro de la card
        const eventName = card.querySelector('h3').innerText.toLowerCase();
        const eventVenue = card.querySelector('.flex.items-center span').innerText.toLowerCase();

        if (eventName.includes(searchTerm) || eventVenue.includes(searchTerm)) {
            card.style.display = "flex"; // Mostramos
            card.classList.add('animate-in', 'fade-in'); 
        } else {
            card.style.display = "none"; // Ocultamos
        }
    });

    // Opcional: Mostrar mensaje de "No se encontraron resultados"
    updateNoResultsMessage(container, searchTerm);
};

function updateNoResultsMessage(container, searchTerm) {
    let msg = document.getElementById('no-results-msg');
    const visibleCards = Array.from(container.children).filter(c => c.style.display !== 'none' && c.id !== 'no-results-msg');

    if (visibleCards.length === 0) {
        if (!msg) {
            msg = document.createElement('p');
            msg.id = 'no-results-msg';
            msg.className = "text-center text-gray-400 py-12 col-span-full";
            container.appendChild(msg);
        }
        msg.innerText = `No se encontraron eventos para "${searchTerm}"`;
    } else if (msg) {
        msg.remove();
    }
}