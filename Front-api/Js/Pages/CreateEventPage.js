import { createEvent } from '../Components/Services/AdminEventService.js';
import { getCurrentUser } from '../Components/Services/AuthService.js';
import { renderEventsPage } from './EventsPage.js';
import { showToast } from '../Components/Toast/toast.js';

export const renderCreateEventPage = () => {
    const app = document.getElementById('app');
    const user = getCurrentUser();

    app.innerHTML = `
        <div class="max-w-2xl mx-auto animate-in">
            <button id="back-btn" class="text-sm text-gray-500 mb-6 flex items-center gap-1 hover:text-slate-800 transition-colors font-medium">
                ← Volver al catálogo
            </button>

            <div class="text-center mb-8">
                <div class="inline-flex items-center justify-center w-16 h-16 bg-blue-100 rounded-full mb-4">
                    <svg class="w-8 h-8 text-blue-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 6v6m0 0v6m0-6h6m-6 0H6"/>
                    </svg>
                </div>
                <h2 class="text-2xl font-bold text-slate-800">Crear Nuevo Evento</h2>
                <p class="text-gray-400 text-sm mt-1">Completá la información del evento y sus sectores</p>
            </div>

            <form id="create-event-form" class="space-y-6">
                <!-- Datos del evento -->
                <div class="bg-white rounded-3xl border border-gray-100 shadow-sm p-6">
                    <h4 class="text-xs font-bold text-gray-300 uppercase tracking-widest mb-4">Información del evento</h4>
                    <div class="space-y-4">
                        <div>
                            <label class="block text-xs font-bold text-gray-400 uppercase tracking-wider mb-2">Nombre del evento</label>
                            <input type="text" id="event-name" required placeholder="Ej: Concierto de Babasonicos"
                                class="w-full px-4 py-3 rounded-xl border border-gray-200 focus:ring-2 focus:ring-blue-500 focus:border-transparent outline-none transition-all text-sm">
                        </div>
                        <div class="grid grid-cols-2 gap-4">
                            <div>
                                <label class="block text-xs font-bold text-gray-400 uppercase tracking-wider mb-2">Fecha</label>
                                <input type="datetime-local" id="event-date" required
                                    class="w-full px-4 py-3 rounded-xl border border-gray-200 focus:ring-2 focus:ring-blue-500 focus:border-transparent outline-none transition-all text-sm">
                            </div>
                            <div>
                                <label class="block text-xs font-bold text-gray-400 uppercase tracking-wider mb-2">Venue</label>
                                <input type="text" id="event-venue" required placeholder="Ej: Estadio Nacional"
                                    class="w-full px-4 py-3 rounded-xl border border-gray-200 focus:ring-2 focus:ring-blue-500 focus:border-transparent outline-none transition-all text-sm">
                            </div>
                        </div>
                        <div>
                            <label class="block text-xs font-bold text-gray-400 uppercase tracking-wider mb-2">URL de la Imagen</label>
                            <input type="url" id="event-image" placeholder="https://ejemplo.com/imagen.jpg"
                                class="w-full px-4 py-3 rounded-xl border border-gray-200 focus:ring-2 focus:ring-blue-500 focus:border-transparent outline-none transition-all text-sm">
                        </div>
                    </div>
                </div>

                <!-- Sectores -->
                <div class="bg-white rounded-3xl border border-gray-100 shadow-sm p-6">
                    <div class="flex items-center justify-between mb-4">
                        <h4 class="text-xs font-bold text-gray-300 uppercase tracking-widest">Sectores</h4>
                        <button type="button" id="add-sector-btn"
                            class="text-xs font-bold text-blue-600 hover:text-blue-800 transition-colors flex items-center gap-1">
                            <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 6v6m0 0v6m0-6h6m-6 0H6"/>
                            </svg>
                            Agregar sector
                        </button>
                    </div>
                    <div id="sectors-container" class="space-y-4">
                        <!-- Se genera dinámicamente -->
                    </div>
                </div>

                <!-- Submit -->
                <button type="submit" id="create-btn"
                    class="w-full bg-primary-dark text-white py-4 rounded-2xl font-bold shadow-lg hover:bg-slate-800 transition-all active:scale-95 flex justify-center items-center gap-2">
                    <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M5 13l4 4L19 7"/>
                    </svg>
                    Crear Evento
                </button>
            </form>
        </div>
    `;

    // Agregar un sector por defecto
    addSectorRow();

    // Event Listeners
    document.getElementById('back-btn').onclick = () => renderEventsPage();
    document.getElementById('add-sector-btn').onclick = () => addSectorRow();
    document.getElementById('create-event-form').addEventListener('submit', async (e) => {
        e.preventDefault();
        await handleCreateEvent(user);
    });
};

let sectorCount = 0;

function addSectorRow() {
    sectorCount++;
    const container = document.getElementById('sectors-container');
    const row = document.createElement('div');
    row.className = 'grid grid-cols-12 gap-3 items-end';
    row.id = `sector-row-${sectorCount}`;
    row.innerHTML = `
        <div class="col-span-5">
            <label class="block text-[10px] font-bold text-gray-300 uppercase mb-1">Nombre</label>
            <input type="text" required placeholder="Ej: VIP" data-field="name"
                class="sector-input w-full px-3 py-2.5 rounded-xl border border-gray-200 focus:ring-2 focus:ring-blue-500 outline-none text-sm">
        </div>
        <div class="col-span-3">
            <label class="block text-[10px] font-bold text-gray-300 uppercase mb-1">Precio ($)</label>
            <input type="number" required min="1" placeholder="5000" data-field="price"
                class="sector-input w-full px-3 py-2.5 rounded-xl border border-gray-200 focus:ring-2 focus:ring-blue-500 outline-none text-sm">
        </div>
        <div class="col-span-3">
            <label class="block text-[10px] font-bold text-gray-300 uppercase mb-1">Butacas</label>
            <input type="number" required min="1" max="500" placeholder="50" data-field="capacity"
                class="sector-input w-full px-3 py-2.5 rounded-xl border border-gray-200 focus:ring-2 focus:ring-blue-500 outline-none text-sm">
        </div>
        <div class="col-span-1 flex justify-center">
            <button type="button" class="remove-sector-btn text-gray-300 hover:text-red-500 transition-colors p-2" title="Eliminar sector">
                <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16"/>
                </svg>
            </button>
        </div>
    `;
    container.appendChild(row);

    row.querySelector('.remove-sector-btn').onclick = () => {
        if (document.querySelectorAll('#sectors-container > div').length > 1) {
            row.remove();
        } else {
            showToast("Necesitás al menos un sector.", "warning");
        }
    };
}

async function handleCreateEvent(user) {
    const btn = document.getElementById('create-btn');
    btn.innerHTML = `
        <svg class="animate-spin h-5 w-5 text-white" fill="none" viewBox="0 0 24 24">
            <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
            <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
        </svg> Creando...`;
    btn.disabled = true;

    try {
        // Recolectar sectores del formulario
        const sectorRows = document.querySelectorAll('#sectors-container > div');
        const sectors = [];

        sectorRows.forEach(row => {
            const inputs = row.querySelectorAll('.sector-input');
            const sector = {};
            inputs.forEach(input => {
                const field = input.getAttribute('data-field');
                sector[field] = field === 'name' ? input.value : Number(input.value);
            });
            sectors.push(sector);
        });

        const eventData = {
            userId: user.id,
            name: document.getElementById('event-name').value,
            eventDate: new Date(document.getElementById('event-date').value).toISOString(),
            venue: document.getElementById('event-venue').value,
            imageUrl: document.getElementById('event-image').value,
            sectors
        };

        await createEvent(eventData);
        showToast("🎉 ¡Evento creado con éxito!", "success");
        sectorCount = 0;
        renderEventsPage();

    } catch (error) {
        if (error.status === 403) {
            showToast("No tenés permisos para crear eventos.", "error");
        } else {
            showToast(error.message || "Error al crear el evento.", "error");
        }

        btn.innerHTML = `
            <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M5 13l4 4L19 7"/>
            </svg>
            Crear Evento`;
        btn.disabled = false;
    }
}
