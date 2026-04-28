import { renderEventsPage } from '../Pages/EventsPage.js';

const init = async () => {
    console.log(" Stagely Frontend Initialized...");
    
    
    // Forzamos la limpieza del spinner si por alguna razón no se reemplaza automáticamente
    await renderEventsPage();
};

// Con type="module", a veces es más seguro verificar el estado del documento
if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', init);
} else {
    init();
}