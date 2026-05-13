import { getUserReservations } from '../Components/Services/ReservationService.js';
import { getCurrentUser } from '../Components/Services/AuthService.js';
import { renderEventsPage } from './EventsPage.js';
import { showToast } from '../Components/Toast/toast.js';

export const renderMyTicketsPage = async () => {
    const app = document.getElementById('app');
    const user = getCurrentUser();

    app.innerHTML = `
        <div class="max-w-4xl mx-auto animate-in">
            <div class="flex items-center justify-between mb-8">
                <div>
                    <h2 class="text-3xl font-bold text-slate-800 tracking-tight">Mis Entradas</h2>
                    <p class="text-gray-500">Historial de tus reservas y compras</p>
                </div>
                <button id="back-btn" class="text-sm font-bold text-blue-600 hover:text-blue-800 transition-colors">
                    ← Volver al catálogo
                </button>
            </div>

            <div id="tickets-container" class="space-y-4">
                <div class="flex justify-center py-12">
                    <div class="animate-spin rounded-full h-8 w-8 border-b-2 border-blue-600"></div>
                </div>
            </div>
        </div>

        <!-- Custom Modal for Cancellation -->
        <div id="cancel-modal" class="fixed inset-0 bg-slate-900/60 backdrop-blur-sm z-50 hidden flex items-center justify-center p-4">
            <div class="bg-white rounded-3xl max-w-sm w-full p-8 shadow-2xl scale-in">
                <div class="w-16 h-16 bg-red-50 text-red-500 rounded-full flex items-center justify-center mx-auto mb-6">
                    <svg class="w-8 h-8" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-3L13.732 4c-.77-1.333-2.694-1.333-3.464 0L3.34 16c-.77 1.333.192 3 1.732 3z"/>
                    </svg>
                </div>
                <h3 class="text-xl font-bold text-slate-800 text-center mb-2">¿Cancelar reserva?</h3>
                <p class="text-gray-500 text-center text-sm mb-8">La butaca se liberará inmediatamente para que otros puedan comprarla.</p>
                <div class="flex gap-3">
                    <button id="close-modal-btn" class="flex-1 px-4 py-3 rounded-xl bg-gray-100 text-gray-600 font-bold text-sm hover:bg-gray-200 transition-colors">Volver</button>
                    <button id="confirm-cancel-btn" class="flex-1 px-4 py-3 rounded-xl bg-red-500 text-white font-bold text-sm hover:bg-red-600 shadow-lg shadow-red-200 transition-all">Sí, cancelar</button>
                </div>
            </div>
        </div>
    `;

    document.getElementById('back-btn').onclick = () => renderEventsPage();
    const container = document.getElementById('tickets-container');
    const modal = document.getElementById('cancel-modal');
    let reservationToCancel = null;

    document.getElementById('close-modal-btn').onclick = () => modal.classList.add('hidden');

    document.getElementById('confirm-cancel-btn').onclick = async () => {
        if (reservationToCancel) {
            modal.classList.add('hidden');
            await handleCancelReservation(reservationToCancel);
        }
    };

    try {
        const reservations = await getUserReservations(user.id);

        if (reservations.length === 0) {
            container.innerHTML = `
                <div class="text-center py-20 bg-gray-50 rounded-3xl border-2 border-dashed border-gray-200">
                    <div class="bg-gray-100 w-16 h-16 rounded-full flex items-center justify-center mx-auto mb-4">
                        <svg class="w-8 h-8 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 5v2m0 4v2m0 4v2M5 5a2 2 0 00-2 2v3a2 2 0 110 4v3a2 2 0 002 2h14a2 2 0 002-2v-3a2 2 0 110-4V7a2 2 0 00-2-2H5z"/>
                        </svg>
                    </div>
                    <p class="text-gray-500 font-medium">Todavía no tenés entradas.</p>
                    <button id="start-shopping" class="mt-4 text-blue-600 font-bold hover:underline">¡Empezar a buscar eventos!</button>
                </div>
            `;
            document.getElementById('start-shopping').onclick = () => renderEventsPage();
            return;
        }

        container.innerHTML = reservations.map(res => {
            const date = new Date(res.eventDate).toLocaleDateString('es-AR', {
                day: '2-digit',
                month: 'long',
                year: 'numeric'
            });

            const statusColors = {
                'Completed': 'bg-green-100 text-green-700',
                'Pending': 'bg-amber-100 text-amber-700',
                'Cancelled': 'bg-red-100 text-red-700',
                'Expired': 'bg-gray-100 text-gray-500'
            };

            const statusLabels = {
                'Completed': 'Pagado',
                'Pending': 'Pendiente',
                'Cancelled': 'Cancelado',
                'Expired': 'Expirado'
            };

            return `
                <div class="bg-white rounded-2xl border border-gray-100 shadow-sm p-6 flex flex-col md:flex-row gap-6 items-center transition-all hover:shadow-md">
                    <div class="w-full md:w-32 h-20 rounded-xl overflow-hidden bg-gray-100 flex-shrink-0">
                        ${res.eventImageUrl 
                            ? `<img src="${res.eventImageUrl}" class="w-full h-full object-cover">`
                            : `<div class="w-full h-full bg-blue-600 flex items-center justify-center text-white font-bold">EVENT</div>`
                        }
                    </div>
                    
                    <div class="flex-grow text-center md:text-left">
                        <h4 class="font-bold text-lg text-slate-800">${res.eventName}</h4>
                        <p class="text-sm text-gray-500">${res.eventVenue} • ${date}</p>
                        <p class="text-xs text-gray-400 mt-1">Sector: ${res.sectorName} • Butaca: #${res.seatNumber}</p>
                    </div>

                    <div class="flex flex-col items-center md:items-end gap-2">
                        <span class="px-3 py-1 rounded-full text-[10px] font-bold uppercase tracking-wider ${statusColors[res.status] || 'bg-gray-100'}">
                            ${statusLabels[res.status] || res.status}
                        </span>
                        <p class="font-black text-slate-800 text-lg">$${res.price.toLocaleString('es-AR')}</p>
                        ${res.status === 'Pending' ? `
                            <button class="cancel-res-btn text-[10px] font-bold text-red-400 hover:text-red-600 transition-colors uppercase tracking-widest" data-id="${res.reservationId}">
                                Cancelar reserva
                            </button>
                        ` : ''}
                    </div>
                </div>
            `;
        }).join('');

        // Listeners para abrir modal
        document.querySelectorAll('.cancel-res-btn').forEach(btn => {
            btn.onclick = () => {
                reservationToCancel = btn.getAttribute('data-id');
                modal.classList.remove('hidden');
            };
        });

    } catch (error) {
        container.innerHTML = `<p class="text-center text-red-500 py-10">Error al cargar: ${error.message}</p>`;
    }
};

async function handleCancelReservation(id) {
    try {
        const response = await fetch(`http://localhost:5280/api/v1/reservations/${id}/cancel`, {
            method: 'POST'
        });
        if (response.ok) {
            showToast("Reserva cancelada correctamente.", "success");
            renderMyTicketsPage();
        } else {
            const data = await response.json();
            showToast(data.error || "No se pudo cancelar.", "error");
        }
    } catch (e) {
        showToast("Error de conexión.", "error");
    }
}
