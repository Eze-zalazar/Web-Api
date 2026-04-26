/**
 * Muestra una notificación visual en pantalla.
 * @param {string} message - El texto a mostrar.
 * @param {'success' | 'error' | 'warning'} type - El estilo del mensaje.
 */
export const showToast = (message, type = 'success') => {
    let container = document.getElementById('toast-container');
    if (!container) {
        container = document.createElement('div');
        container.id = 'toast-container';
        container.className = "fixed bottom-5 right-5 z-[100] flex flex-col gap-3";
        document.body.appendChild(container);
    }
 
    const toast = document.createElement('div');
    
    const styles = {
        success: "bg-emerald-600",
        error: "bg-rose-600",
        warning: "bg-amber-500"
    };
 
    const icons = {
        success: '<svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path d="M5 13l4 4L19 7" stroke-width="3" stroke-linecap="round" stroke-linejoin="round"/></svg>',
        error: '<svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12"/></svg>',
        warning: '<svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-3L13.732 4c-.77-1.333-2.694-1.333-3.464 0L3.34 16c-.77 1.333.192 3 1.732 3z"/></svg>'
    };
 
    toast.className = `
        ${styles[type]} text-white px-5 py-4 rounded-2xl shadow-2xl 
        flex items-center gap-3 min-w-[300px] max-w-sm
        transition-all duration-300
    `;
 
    toast.innerHTML = `
        <span class="bg-white/20 p-1.5 rounded-full flex-shrink-0">${icons[type]}</span>
        <p class="font-medium text-sm flex-grow">${message}</p>
        <button class="ml-2 opacity-60 hover:opacity-100 transition-opacity flex-shrink-0" onclick="this.closest('[id]').remove()">
            <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12"/>
            </svg>
        </button>
    `;
 
    container.appendChild(toast);
 
    // Auto-dismiss después de 4 segundos
    setTimeout(() => {
        toast.style.opacity = '0';
        toast.style.transform = 'translateX(20px)';
        setTimeout(() => toast.remove(), 300);
    }, 4000);
};
 