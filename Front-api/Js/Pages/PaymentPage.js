import { renderEventsPage } from './EventsPage.js';
import { processPayment } from '../Components/Services/PaymentService.js';
import { showToast } from '../Components/Toast/toast.js';
import { getCurrentUser } from '../Components/Services/AuthService.js';

let timerInterval = null;

export const renderPaymentPage = (reservationData, eventData, seatData) => {
    const app = document.getElementById('app');

    // Limpiar cualquier timer anterior
    if (timerInterval) {
        clearInterval(timerInterval);
        timerInterval = null;
    }

    const sectorName = seatData.sectorName ?? seatData.sectorId;
    const price = seatData.price
        ? seatData.price.toLocaleString('es-AR', { style: 'currency', currency: 'ARS' })
        : '—';

    app.innerHTML = `
        <div class="max-w-2xl mx-auto animate-in">
            
            <!-- Header -->
            <div class="text-center mb-8">
                <div class="inline-flex items-center justify-center w-16 h-16 bg-blue-100 rounded-full mb-4">
                    <svg class="w-8 h-8 text-blue-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M3 3h2l.4 2M7 13h10l4-8H5.4M7 13L5.4 5M7 13l-2.293 2.293c-.63.63-.184 1.707.707 1.707H17m0 0a2 2 0 100 4 2 2 0 000-4zm-8 2a2 2 0 100 4 2 2 0 000-4z"/>
                    </svg>
                </div>
                <h2 class="text-2xl font-bold text-slate-800">Confirmá tu compra</h2>
                <p class="text-gray-400 text-sm mt-1">Completá el pago antes de que expire el tiempo</p>
            </div>

            <!-- Timer -->
            <div id="timer-container" class="bg-slate-900 text-white rounded-2xl p-6 mb-6 text-center shadow-lg">
                <p class="text-[10px] uppercase font-bold tracking-widest text-gray-400 mb-2">Tiempo restante</p>
                <p id="timer-display" class="text-5xl font-black font-mono tracking-wider">05:00</p>
                <div class="w-full bg-white/10 rounded-full h-1.5 mt-4">
                    <div id="timer-bar" class="bg-emerald-400 h-1.5 rounded-full transition-all duration-1000" style="width: 100%"></div>
                </div>
            </div>

            <!-- Detalle de la compra -->
            <div class="bg-white rounded-2xl border border-gray-100 shadow-sm p-6 mb-6">
                <h4 class="text-xs font-bold text-gray-300 uppercase tracking-widest mb-4">Detalle de la reserva</h4>
                
                <div class="space-y-4">
                    <div class="flex justify-between items-center">
                        <span class="text-sm text-gray-500">Evento</span>
                        <span class="font-bold text-slate-800">${eventData.name}</span>
                    </div>
                    <div class="flex justify-between items-center">
                        <span class="text-sm text-gray-500">Fecha</span>
                        <span class="font-medium text-slate-700">${new Date(eventData.eventDate).toLocaleDateString('es-AR')}</span>
                    </div>
                    <div class="flex justify-between items-center">
                        <span class="text-sm text-gray-500">Venue</span>
                        <span class="font-medium text-slate-700">${eventData.venue}</span>
                    </div>
                    <hr class="border-gray-50">
                    <div class="flex justify-between items-center">
                        <span class="text-sm text-gray-500">Sector</span>
                        <span class="font-medium text-slate-700">${sectorName}</span>
                    </div>
                    <div class="flex justify-between items-center">
                        <span class="text-sm text-gray-500">Butaca</span>
                        <span class="font-bold text-slate-800">#${seatData.seatNumber}</span>
                    </div>
                    <hr class="border-gray-50">
                    <div class="flex justify-between items-center">
                        <span class="text-sm font-bold text-slate-800">Total a pagar</span>
                        <span class="text-xl font-black text-slate-900">${price}</span>
                    </div>
                </div>
            </div>

            <!-- Botones -->
            <div class="flex gap-3">
                <button id="cancel-btn" class="flex-1 border border-gray-200 text-gray-600 py-4 rounded-2xl font-bold text-sm hover:bg-gray-50 transition-colors">
                    Cancelar
                </button>
                <button id="pay-btn" class="flex-1 bg-primary-dark text-white py-4 rounded-2xl font-bold shadow-lg hover:bg-slate-800 transition-all active:scale-95 flex justify-center items-center gap-2">
                    <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17 9V7a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2m2 4h10a2 2 0 002-2v-6a2 2 0 00-2-2H9a2 2 0 00-2 2v6a2 2 0 002 2zm7-5a2 2 0 11-4 0 2 2 0 014 0z"/>
                    </svg>
                    Confirmar Pago
                </button>
            </div>
        </div>
    `;

    // Iniciar Temporizador
    startTimer(reservationData.expiresAt);

    // Event Listeners
    document.getElementById('cancel-btn').onclick = () => {
        clearInterval(timerInterval);
        timerInterval = null;
        showToast("Reserva cancelada. La butaca se liberará automáticamente.", "warning");
        renderEventsPage();
    };

    document.getElementById('pay-btn').onclick = () => handlePayment(reservationData.id);
};

function startTimer(expiresAtStr) {
    const timerDisplay = document.getElementById('timer-display');
    const timerBar = document.getElementById('timer-bar');
    const timerContainer = document.getElementById('timer-container');
    const payBtn = document.getElementById('pay-btn');

    const expiresAt = new Date(expiresAtStr).getTime();
    const totalDuration = 5 * 60 * 1000; // 5 minutos en ms

    const updateTimer = () => {
        const now = Date.now();
        const remaining = expiresAt - now;

        if (remaining <= 0) {
            clearInterval(timerInterval);
            timerInterval = null;
            timerDisplay.textContent = '00:00';
            timerBar.style.width = '0%';
            timerContainer.classList.remove('bg-slate-900');
            timerContainer.classList.add('bg-red-900');
            payBtn.disabled = true;
            payBtn.classList.add('opacity-50', 'cursor-not-allowed');
            payBtn.classList.remove('hover:bg-slate-800');

            showToast("⏰ Tu reserva ha expirado. Elegí otra butaca.", "error");

            setTimeout(() => renderEventsPage(), 3000);
            return;
        }

        const minutes = Math.floor(remaining / 60000);
        const seconds = Math.floor((remaining % 60000) / 1000);
        timerDisplay.textContent = `${String(minutes).padStart(2, '0')}:${String(seconds).padStart(2, '0')}`;

        // Barra de progreso
        const percentage = (remaining / totalDuration) * 100;
        timerBar.style.width = `${Math.max(0, percentage)}%`;

        // Cambio de color cuando queda poco tiempo
        if (remaining < 60000) {
            timerBar.classList.remove('bg-emerald-400');
            timerBar.classList.add('bg-red-400');
            timerDisplay.classList.add('text-red-400');
        } else if (remaining < 120000) {
            timerBar.classList.remove('bg-emerald-400');
            timerBar.classList.add('bg-amber-400');
        }
    };

    updateTimer(); // Primera ejecución inmediata
    timerInterval = setInterval(updateTimer, 1000);
}

async function handlePayment(reservationId) {
    const payBtn = document.getElementById('pay-btn');

    payBtn.innerHTML = `
        <svg class="animate-spin h-5 w-5 text-white" fill="none" viewBox="0 0 24 24">
            <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
            <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
        </svg> Procesando...`;
    payBtn.disabled = true;

    try {
        // Usar el userId del usuario logueado
        const user = getCurrentUser();
        await processPayment(reservationId, user.id);

        clearInterval(timerInterval);
        timerInterval = null;

        showToast("🎉 ¡Pago confirmado! Ya tenés tu entrada.", "success");

        setTimeout(() => renderEventsPage(), 2000);

    } catch (error) {
        payBtn.innerHTML = `
            <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17 9V7a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2m2 4h10a2 2 0 002-2v-6a2 2 0 00-2-2H9a2 2 0 00-2 2v6a2 2 0 002 2zm7-5a2 2 0 11-4 0 2 2 0 014 0z"/>
            </svg>
            Confirmar Pago`;
        payBtn.disabled = false;

        if (error.message?.includes('expirado') || error.message?.includes('expirada')) {
            showToast("⏰ Tu reserva ha expirado.", "error");
            setTimeout(() => renderEventsPage(), 2000);
        } else {
            showToast(error.message || "Error al procesar el pago.", "error");
        }
    }
}
