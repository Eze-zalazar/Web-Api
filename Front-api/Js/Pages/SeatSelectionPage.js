import { renderEventsPage } from './EventsPage.js';
import { getSeatsByEvent } from '../Components/Services/SeatService.js';
import { getEventById } from '../Components/Services/EventService.js';
import { createReservation } from '../Components/Services/ReservationService.js';
import { showToast } from '../Components/Toast/toast.js';
import { createSeat } from '../Components/Carts/SectorCard.js';

const getEventBackground = (name) => {
    const backgrounds = {
        babasonicos: 'https://freight.cargo.site/i/e90f4f73ca28c9cfd211f9cfbc9dabb5fd2be4492d9c7884a47cc89fdf7c0980/Babasonicos-Prensa-1280.jpg',
        piojos: 'https://cdn.rock.com.ar/wp-content/uploads/2023/01/los-piojos-6.webp',
        jonas: 'https://hips.hearstapps.com/hmg-prod/images/2-68f15344b6337.jpg',
        anuel: 'https://akamai.sscdn.co/tb/letras-blog/wp-content/uploads/2023/06/298041e-anuel-aa-1024x576.jpg',
        miranda: 'https://upload.wikimedia.org/wikipedia/commons/7/7c/Miranda%21_en_el_Festival_Verano_Iquique_2020.jpg',
        duki: 'https://mdx.global/wp-content/uploads/2025/11/Copia-de-Copia-de-GDR04296-3-1024x576.jpg.webp'
    };

    const lower = name.toLowerCase();
    let bgUrl = '';

    if (lower.includes('babas') || lower.includes('babasonicos')) bgUrl = backgrounds.babasonicos;
    else if (lower.includes('piojos')) bgUrl = backgrounds.piojos;
    else if (lower.includes('jonas')) bgUrl = backgrounds.jonas;
    else if (lower.includes('anuel')) bgUrl = backgrounds.anuel;
    else if (lower.includes('miranda')) bgUrl = backgrounds.miranda;
    else if (lower.includes('duki')) bgUrl = backgrounds.duki;

    return bgUrl
        ? `background-image: url('${bgUrl}'); background-size: cover; background-position: center;`
        : 'background: linear-gradient(135deg, #1e3a5f, #2563eb);';
};

let selectedSeat = null;

export const renderSeatSelection = async (eventId) => {
    const app = document.getElementById('app');

    app.innerHTML = `
        <div class="animate-pulse space-y-6">
            <div class="h-8 w-32 bg-gray-200 rounded-lg"></div>
            <div class="bg-gray-200 h-48 rounded-3xl"></div>
            <div class="flex gap-8">
                <div class="flex-grow bg-gray-100 h-96 rounded-3xl"></div>
                <div class="w-80 bg-gray-100 h-64 rounded-3xl"></div>
            </div>
        </div>
    `;

    try {
        const [event, seats] = await Promise.all([
            getEventById(eventId),
            getSeatsByEvent(eventId)
        ]);

        app.innerHTML = `
            <button id="back-btn" class="text-sm text-gray-500 mb-4 flex items-center gap-1 hover:text-slate-800 transition-colors font-medium">
                ← Volver a eventos
            </button>
            
            <div class="event-card-header rounded-3xl p-8 text-white mb-8 shadow-lg" style="${getEventBackground(event.name)}">
                <span class="bg-white/20 backdrop-blur-sm text-[10px] px-3 py-1 rounded-full uppercase font-bold tracking-wider">En venta</span>
                <h2 class="text-4xl font-black mt-2">${event.name}</h2>
                <p class="opacity-90 font-medium mt-1">${event.venue} • ${new Date(event.eventDate).toLocaleDateString('es-AR')}</p>
            </div>

            <div class="flex flex-col lg:flex-row gap-8">
                <div class="flex-grow bg-white p-8 rounded-3xl border border-gray-100 shadow-sm">
                    <div class="w-full h-1.5 bg-blue-900/10 mb-10 rounded-full overflow-hidden relative">
                        <div class="w-full h-full bg-blue-900 opacity-30"></div>
                        <p class="absolute -bottom-6 left-1/2 -translate-x-1/2 text-[10px] uppercase font-bold text-gray-300 tracking-[0.2em]">Escenario</p>
                    </div>
                    <div id="seats-container" class="space-y-10"></div>
                </div>

                <aside class="w-full lg:w-80 bg-white p-6 rounded-3xl border border-gray-100 h-fit sticky top-24 shadow-sm">
                    <h4 class="font-bold text-slate-800 mb-4">Tu selección</h4>
                    <div id="selection-info">
                        <p class="text-sm text-gray-400 py-4">Seleccioná una butaca disponible del mapa para comenzar.</p>
                    </div>
                    
                    <button id="reserve-btn" disabled 
                        class="w-full mt-6 bg-primary-dark text-white py-4 rounded-2xl font-bold opacity-50 cursor-not-allowed transition-all shadow-lg active:scale-95 flex justify-center items-center gap-2">
                        Reservar butaca
                    </button>

                    <div class="mt-8 pt-6 border-t border-gray-50 flex flex-col gap-3">
                        <div class="flex items-center gap-3 text-[11px] font-bold text-gray-400 uppercase">
                            <span class="w-3 h-3 rounded-full bg-available"></span> Disponible
                        </div>
                        <div class="flex items-center gap-3 text-[11px] font-bold text-gray-400 uppercase">
                            <span class="w-3 h-3 rounded-full bg-selected"></span> Seleccionado
                        </div>
                        <div class="flex items-center gap-3 text-[11px] font-bold text-gray-400 uppercase">
                            <span class="w-3 h-3 rounded-full bg-occupied"></span> Reservado
                        </div>
                        <div class="flex items-center gap-3 text-[11px] font-bold text-gray-400 uppercase">
                            <span class="w-3 h-3 rounded-full bg-gray-400"></span> Vendido
                        </div>
                    </div>
                </aside>
            </div>
        `;

        renderSectors(seats);

        document.getElementById('back-btn').onclick = () => {
            selectedSeat = null;
            renderEventsPage();
        };

        document.getElementById('reserve-btn').onclick = () => handleReservation(eventId);

    } catch (error) {
        console.error(error);
        app.innerHTML = `
            <div class="text-center py-20">
                <div class="w-16 h-16 bg-red-100 rounded-full flex items-center justify-center mx-auto mb-4">
                    <svg class="w-8 h-8 text-red-500" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-3L13.732 4c-.77-1.333-2.694-1.333-3.464 0L3.34 16c-.77 1.333.192 3 1.732 3z"/>
                    </svg>
                </div>
                <p class="text-slate-800 font-bold text-xl mb-2">No se pudo cargar el mapa de butacas</p>
                <p class="text-gray-400 text-sm mb-6">Verificá que la API esté corriendo e intentá de nuevo.</p>
                <div class="flex gap-3 justify-center">
                    <button onclick="location.reload()" class="bg-blue-900 text-white px-6 py-2 rounded-xl font-semibold text-sm hover:bg-slate-800 transition-colors">
                        Reintentar
                    </button>
                    <button id="back-err-btn" class="border border-gray-200 text-gray-600 px-6 py-2 rounded-xl font-semibold text-sm hover:bg-gray-50 transition-colors">
                        Volver a eventos
                    </button>
                </div>
            </div>`;
        document.getElementById('back-err-btn').onclick = () => renderEventsPage();
    }
};

function renderSectors(seats) {
    const container = document.getElementById('seats-container');
    container.innerHTML = '';

    const sectorIds = [...new Set(seats.map(s => s.sectorId))].sort((a, b) => a - b);

    if (sectorIds.length === 0) {
        container.innerHTML = `
            <div class="text-center py-16 text-gray-400">
                <p class="font-semibold">No hay butacas disponibles para este evento.</p>
            </div>`;
        return;
    }

    sectorIds.forEach(sec => {
        const sectorSeats = seats
            .filter(s => s.sectorId === sec)
            .sort((a, b) => {
                if (a.row !== b.row) return a.row - b.row;
                return a.seatNumber - b.seatNumber;
            });

        if (sectorSeats.length === 0) return;

        const available = sectorSeats.filter(s => s.status === 'Available').length;
        const total = sectorSeats.length;
        const firstSeat = sectorSeats[0];
        const sectorName = firstSeat.sectorName ?? firstSeat.sectorId;  // usa el nombre real de la API
        const sectorPrice = firstSeat.price
            ? firstSeat.price.toLocaleString('es-AR', { style: 'currency', currency: 'ARS' })
            : 'Ver precio';

        const sectorDiv = document.createElement('div');
        sectorDiv.innerHTML = `
            <div class="flex items-center justify-between mb-4">
                <h5 class="text-xs font-black text-gray-300 uppercase tracking-widest">
                    ${sectorName} — ${sectorPrice}
                </h5>
                <span class="text-[10px] font-bold ${available === 0 ? 'text-red-400' : 'text-gray-400'}">
                    ${available === 0 ? 'AGOTADO' : `${available} / ${total} disponibles`}
                </span>
            </div>
        `;

        const grid = document.createElement('div');
        grid.className = "flex flex-wrap gap-2";

        if (available === 0) {
            grid.innerHTML = `<p class="text-xs text-gray-300 italic py-2">Todas las butacas de este sector están reservadas.</p>`;
        } else {
            sectorSeats.forEach(seat => {
                const seatEl = createSeat(seat, selectedSeat?.id, (clickedSeat) => {
                    selectedSeat = clickedSeat;
                    updateSelectionUI();
                    renderSectors(seats);
                });
                grid.appendChild(seatEl);
            });
        }

        sectorDiv.appendChild(grid);
        container.appendChild(sectorDiv);
    });
}

function updateSelectionUI() {
    const info = document.getElementById('selection-info');
    const btn = document.getElementById('reserve-btn');

    if (selectedSeat) {
        const sectorName = selectedSeat.sectorName ?? selectedSeat.sectorId;  // usa el nombre real de la API
        const price = selectedSeat.price
            ? selectedSeat.price.toLocaleString('es-AR', { style: 'currency', currency: 'ARS' })
            : 'Ver precio';

        info.innerHTML = `
            <div class="bg-slate-50 p-4 rounded-2xl border border-blue-50">
                <p class="text-[10px] text-blue-600 font-bold uppercase tracking-tight">${sectorName}</p>
                <p class="font-bold text-xl text-slate-800">Butaca ${selectedSeat.seatNumber}</p>
                <div class="flex justify-between items-center mt-4 pt-4 border-t border-slate-200/50">
                    <span class="text-[10px] text-gray-400 uppercase font-bold">Total</span>
                    <span class="font-black text-slate-900">${price}</span>
                </div>
            </div>
        `;
        btn.disabled = false;
        btn.classList.remove('opacity-50', 'cursor-not-allowed');
        btn.classList.add('hover:bg-slate-800');
    } else {
        info.innerHTML = `<p class="text-sm text-gray-400 py-4">Seleccioná una butaca disponible del mapa para comenzar.</p>`;
        btn.disabled = true;
        btn.classList.add('opacity-50', 'cursor-not-allowed');
        btn.classList.remove('hover:bg-slate-800');
    }
}

async function handleReservation(eventId) {
    const btn = document.getElementById('reserve-btn');
    const container = document.getElementById('seats-container');

    btn.innerHTML = `
        <svg class="animate-spin h-5 w-5 text-white" fill="none" viewBox="0 0 24 24">
            <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
            <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
        </svg> Procesando...`;
    btn.disabled = true;

    try {
        await createReservation(selectedSeat.id, 1);

        showToast("¡Reserva confirmada! Disfrutá el show 🎵", "success");
        selectedSeat = null;

        container.style.opacity = '0.4';
        container.style.pointerEvents = 'none';

        const freshSeats = await getSeatsByEvent(eventId);

        container.style.opacity = '1';
        container.style.pointerEvents = 'auto';

        renderSectors(freshSeats);
        updateSelectionUI();

        btn.innerHTML = 'Reservar butaca';
        btn.disabled = true;
        btn.classList.add('opacity-50', 'cursor-not-allowed');
        btn.classList.remove('hover:bg-slate-800');

    } catch (error) {
        container.style.opacity = '1';
        container.style.pointerEvents = 'auto';

        if (error.status === 409) {
            showToast("¡Lo sentimos! Otro usuario acaba de tomar esa butaca.", "error");
            selectedSeat = null;
            const freshSeats = await getSeatsByEvent(eventId);
            renderSectors(freshSeats);
            updateSelectionUI();
        } else if (error instanceof TypeError) {
            showToast("No se pudo conectar con el servidor. Verificá tu conexión.", "error");
        } else {
            showToast("Algo salió mal. Por favor intentá de nuevo.", "error");
        }

        btn.innerHTML = 'Reservar butaca';
        btn.disabled = false;
    }
}