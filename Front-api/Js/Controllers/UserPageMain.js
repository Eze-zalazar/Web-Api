import { renderEventsPage } from '../Pages/EventsPage.js';

const init = async () => {
    console.log(" Stagely Frontend Initialized...");
    
   
    await renderEventsPage();
};

if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', init);
} else {
    init();
}