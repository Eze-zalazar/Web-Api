import { login } from '../Components/Services/AuthService.js';
import { renderEventsPage } from './EventsPage.js';
import { showToast } from '../Components/Toast/toast.js';
import { renderMyTicketsPage } from './MyTicketsPage.js';

export const renderLoginPage = () => {
    const app = document.getElementById('app');

    app.innerHTML = `
        <div class="min-h-[70vh] flex items-center justify-center animate-in">
            <div class="w-full max-w-md">
                
                <!-- Logo -->
                <div class="text-center mb-8">
                    <div class="inline-flex items-center justify-center w-20 h-20 bg-blue-900 text-white rounded-2xl mb-4 shadow-lg">
                        <svg xmlns="http://www.w3.org/2000/svg" width="36" height="36" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><rect width="18" height="18" x="3" y="3" rx="2"/><path d="M7 3v18"/><path d="M3 7h18"/><path d="M3 12h18"/><path d="M3 17h18"/><path d="M17 3v18"/></svg>
                    </div>
                    <h2 class="text-3xl font-black text-slate-800 tracking-tight">Bienvenido a Stagely</h2>
                    <p class="text-gray-400 mt-2 text-sm">Iniciá sesión para reservar tus entradas</p>
                </div>

                <!-- Form -->
                <div class="bg-white rounded-3xl border border-gray-100 shadow-sm p-8">
                    <form id="login-form" class="space-y-5">
                        <div>
                            <label class="block text-xs font-bold text-gray-400 uppercase tracking-wider mb-2">Correo electrónico</label>
                            <input type="email" id="login-email" required
                                placeholder="tu@email.com"
                                class="w-full px-4 py-3 rounded-xl border border-gray-200 focus:ring-2 focus:ring-blue-500 focus:border-transparent outline-none transition-all text-sm">
                        </div>
                        <div>
                            <label class="block text-xs font-bold text-gray-400 uppercase tracking-wider mb-2">Contraseña</label>
                            <input type="password" id="login-password" required
                                placeholder="••••••••"
                                class="w-full px-4 py-3 rounded-xl border border-gray-200 focus:ring-2 focus:ring-blue-500 focus:border-transparent outline-none transition-all text-sm">
                        </div>

                        <button type="submit" id="login-btn"
                            class="w-full bg-primary-dark text-white py-4 rounded-2xl font-bold shadow-lg hover:bg-slate-800 transition-all active:scale-95 flex justify-center items-center gap-2">
                            <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M11 16l-4-4m0 0l4-4m-4 4h14m-5 4v1a3 3 0 01-3 3H6a3 3 0 01-3-3V7a3 3 0 013-3h7a3 3 0 013 3v1"/>
                            </svg>
                            Iniciar Sesión
                        </button>
                    </form>

                    <div class="mt-6 pt-6 border-t border-gray-50">
                        <p class="text-[11px] text-gray-300 text-center uppercase font-bold tracking-wider">Cuentas de prueba</p>
                        <div class="mt-3 space-y-2">
                            <button id="quick-admin" class="w-full text-left px-4 py-2.5 rounded-xl bg-slate-50 hover:bg-slate-100 transition-colors text-xs">
                                <span class="font-bold text-slate-700">Admin:</span> 
                                <span class="text-gray-400">admin@admin.com / admin123</span>
                            </button>
                            <button id="quick-client" class="w-full text-left px-4 py-2.5 rounded-xl bg-slate-50 hover:bg-slate-100 transition-colors text-xs">
                                <span class="font-bold text-slate-700">Cliente:</span> 
                                <span class="text-gray-400">cliente@cliente.com / cliente123</span>
                            </button>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    `;

    // Event Listeners
    document.getElementById('login-form').addEventListener('submit', async (e) => {
        e.preventDefault();
        await handleLogin();
    });

    // Quick fill buttons
    document.getElementById('quick-admin').onclick = () => {
        document.getElementById('login-email').value = 'admin@admin.com';
        document.getElementById('login-password').value = 'admin123';
    };

    document.getElementById('quick-client').onclick = () => {
        document.getElementById('login-email').value = 'cliente@cliente.com';
        document.getElementById('login-password').value = 'cliente123';
    };
};

async function handleLogin() {
    const email = document.getElementById('login-email').value.trim();
    const password = document.getElementById('login-password').value;
    const btn = document.getElementById('login-btn');

    btn.innerHTML = `
        <svg class="animate-spin h-5 w-5 text-white" fill="none" viewBox="0 0 24 24">
            <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
            <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
        </svg> Verificando...`;
    btn.disabled = true;

    try {
        const user = await login(email, password);
        showToast(`¡Bienvenido, ${user.name}!`, "success");
        
        // Actualizar header con info del usuario
        updateHeader(user);
        
        // Navegar al catálogo
        renderEventsPage();

    } catch (error) {
        if (error.status === 401) {
            showToast("Contraseña incorrecta.", "error");
        } else if (error.status === 404) {
            showToast("El usuario no existe.", "error");
        } else if (error instanceof TypeError) {
            showToast("No se pudo conectar con el servidor.", "error");
        } else {
            showToast(error.message || "Error al iniciar sesión.", "error");
        }

        btn.innerHTML = `
            <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M11 16l-4-4m0 0l4-4m-4 4h14m-5 4v1a3 3 0 01-3 3H6a3 3 0 01-3-3V7a3 3 0 013-3h7a3 3 0 013 3v1"/>
            </svg>
            Iniciar Sesión`;
        btn.disabled = false;
    }
}

export function updateHeader(user) {
    const nav = document.querySelector('header nav');
    if (nav && user) {
        nav.innerHTML = `
            <div class="flex items-center gap-6">
                <button id="my-tickets-btn" class="text-sm font-bold text-slate-800 hover:text-blue-600 transition-colors flex items-center gap-1">
                    <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 5v2m0 4v2m0 4v2M5 5a2 2 0 00-2 2v3a2 2 0 110 4v3a2 2 0 002 2h14a2 2 0 002-2v-3a2 2 0 110-4V7a2 2 0 00-2-2H5z"/>
                    </svg>
                    Mis Entradas
                </button>
                <span class="text-sm text-gray-500">
                    Hola, <span class="font-bold text-slate-800">${user.name}</span>
                    ${user.isAdmin ? '<span class="ml-1 text-[10px] bg-blue-100 text-blue-700 px-2 py-0.5 rounded-full font-bold uppercase">Admin</span>' : ''}
                </span>
                <button id="logout-btn" class="text-sm font-medium text-gray-400 hover:text-red-500 transition-colors">
                    Cerrar sesión
                </button>
            </div>
        `;

        document.getElementById('my-tickets-btn').onclick = () => renderMyTicketsPage();

        document.getElementById('logout-btn').onclick = () => {
            localStorage.removeItem('stagely_user');
            showToast("Sesión cerrada.", "warning");
            renderLoginPage();
            // Restaurar header original
            nav.innerHTML = `<button class="text-sm font-medium text-gray-600 hover:text-blue-900 transition-colors">Events</button>`;
        };
    }
}
