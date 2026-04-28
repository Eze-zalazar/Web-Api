import { getAllEvents } from '../Components/Services/EventService.js';
import { createEventCard } from '../Components/Carts/EventsCard.js';
import { renderSeatSelection } from './SeatSelectionPage.js';
import { filterEvents } from '../Components/Search/filterEvents.js';

export const renderEventsPage = async () => {
    const app = document.getElementById('app');
    
    app.innerHTML = `
        <div class="mb-8 flex flex-col md:flex-row md:items-end justify-between gap-4 animate-in fade-in duration-500">
            <div>
                <h2 class="text-3xl font-bold text-slate-800 tracking-tight">Catálogo de eventos</h2>
                <p class="text-gray-500">Encontrá tu lugar. Viví el momento.</p>
            </div>
            
            <div class="relative w-full md:w-80">
                <input type="text" id="event-search" 
                    placeholder="Buscar conciertos o venues..." 
                    class="w-full pl-10 pr-4 py-2.5 rounded-xl border border-gray-200 focus:ring-2 focus:ring-blue-500 outline-none transition-all text-sm shadow-sm">
                <svg class="w-4 h-4 absolute left-3 top-3.5 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z" stroke-width="2" stroke-linecap="round"/>
                </svg>
            </div>
        </div>

        <div id="events-grid" class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
            <div class="h-64 bg-gray-100 animate-pulse rounded-2xl"></div>
            <div class="h-64 bg-gray-100 animate-pulse rounded-2xl"></div>
            <div class="h-64 bg-gray-100 animate-pulse rounded-2xl"></div>
        </div>
    `;

    const grid = document.getElementById('events-grid');
    const searchInput = document.getElementById('event-search');

    searchInput.addEventListener('input', (e) => {
        filterEvents(e.target.value, grid);
    });

    try {
        const events = await getAllEvents();
        
        grid.innerHTML = '';

        if (events.length === 0) {
            grid.innerHTML = `<p class="col-span-full text-center py-20 text-gray-400">No hay eventos disponibles.</p>`;
            return;
        }

        events.forEach(event => {
            const card = createEventCard(event, (id) => {
                renderSeatSelection(id);
            });
            grid.appendChild(card);
        });

    } catch (error) {
        console.error(error);
        grid.innerHTML = `
            <div class="col-span-full bg-red-50 p-8 rounded-2xl text-center">
                <p class="text-red-600 font-bold">Error al conectar con el servidor.</p>
                <p class="text-red-400 text-sm">Asegurate de que la API en el puerto 5280 esté corriendo.</p>
            </div>
        `;
    }
};  