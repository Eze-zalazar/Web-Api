import { renderEventsPage } from '../Pages/EventsPage.js';
import { renderLoginPage, updateHeader } from '../Pages/LoginPage.js';
import { getCurrentUser } from '../Components/Services/AuthService.js';

const init = async () => {
    console.log(" Stagely Frontend Initialized...");
    
    const user = getCurrentUser();

    if (user) {
        // Usuario ya logueado: mostrar catálogo y actualizar header
        updateHeader(user);
        await renderEventsPage();
    } else {
        // Sin sesión: mostrar login
        renderLoginPage();
    }
};

// Con type="module", a veces es más seguro verificar el estado del documento
if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', init);
} else {
    init();
}