/**
 * Genera el HTML para una tarjeta de evento.
 * @param {Object} event - Objeto que viene de la API (id, name, venue, date, price, status, etc.)
 * @param {Function} onSelect - Función que se ejecuta al darle a "Select seats"
 */
export const createEventCard = (event, onSelect) => {
    const card = document.createElement('div');
    card.className = "bg-white rounded-2xl overflow-hidden shadow-sm border border-gray-100 flex flex-col transition-all hover:shadow-md";

    const eventDate = new Date(event.eventDate).toLocaleDateString('es-AR', {
        weekday: 'short',
        day: '2-digit',
        month: 'short',
        year: 'numeric',
        hour: '2-digit',
        minute: '2-digit'
    });

    const backgrounds = {
        babasonicos: 'https://freight.cargo.site/i/e90f4f73ca28c9cfd211f9cfbc9dabb5fd2be4492d9c7884a47cc89fdf7c0980/Babasonicos-Prensa-1280.jpg',
        piojos: 'https://cdn.rock.com.ar/wp-content/uploads/2023/01/los-piojos-6.webp',
        jonas: 'https://hips.hearstapps.com/hmg-prod/images/2-68f15344b6337.jpg',
        anuel: 'https://akamai.sscdn.co/tb/letras-blog/wp-content/uploads/2023/06/298041e-anuel-aa-1024x576.jpg',
        miranda: 'https://upload.wikimedia.org/wikipedia/commons/7/7c/Miranda%21_en_el_Festival_Verano_Iquique_2020.jpg',
        duki: 'https://mdx.global/wp-content/uploads/2025/11/Copia-de-Copia-de-GDR04296-3-1024x576.jpg.webp',
        rock: 'https://images.unsplash.com/photo-1540039155733-5bb30b53aa14?w=800'
    };

    const name = event.name.toLowerCase();

    let bgUrl = '';

    if (name.includes('babasónicos') || name.includes('babasonicos')) {
        bgUrl = backgrounds.babasonicos;
    } else if (name.includes('piojos')) {
        bgUrl = backgrounds.piojos;
    } else if (name.includes('jonas')) {
        bgUrl = backgrounds.jonas;
    } else if (name.includes('anuel')) {
        bgUrl = backgrounds.anuel;
    } else if (name.includes('miranda')) {
        bgUrl = backgrounds.miranda;
    } else if (name.includes('duki')) {
        bgUrl = backgrounds.duki;
    } else if (name.includes('rock')) {
        bgUrl = backgrounds.rock;
    }

    if (event.imageUrl) {
        bgUrl = event.imageUrl;
    }

    const headerStyle = `
        style="
            background-image: url('${bgUrl}');
            background-size: cover;
            background-position: center;
            background-color: #1e293b;
        "
    `;

    card.innerHTML = `
        <div class="event-card-header h-32 p-4 flex flex-col justify-between relative" ${headerStyle}>
            <span class="bg-white/20 backdrop-blur-md text-white text-[10px] uppercase font-bold px-2 py-1 rounded-full w-fit">
                ${event.status === 'Active' ? 'En venta' : event.status || 'En venta'}
            </span>
            <p class="text-white/80 text-xs font-bold tracking-widest uppercase">
                ${event.venueCity || 'CIUDAD'}
            </p>
        </div>

        <div class="p-5 flex flex-col flex-grow">
            <h3 class="font-bold text-lg text-slate-800 mb-2 leading-tight">
                ${event.name}
            </h3>
            
            <div class="space-y-2 mb-6">
                <div class="flex items-center text-gray-500 text-sm gap-2">
                    <span>${event.venue}</span>
                </div>

                <div class="flex items-center text-gray-500 text-sm gap-2">
                    <span>${eventDate}</span>
                </div>
            </div>

            <div class="mt-auto pt-4 border-t border-gray-50 flex items-center justify-between">
                <div>
                    <p class="text-[10px] text-gray-400 uppercase">Desde</p>
                    <p class="font-bold text-slate-800">
                        ${event.price 
                            ? event.price.toLocaleString('es-AR', { style: 'currency', currency: 'ARS' }) 
                            : 'Ver precios'}
                    </p>
                </div>

                <button class="select-seats-btn bg-primary-dark text-white text-sm font-semibold px-4 py-2 rounded-lg flex items-center gap-2 hover:bg-slate-800 transition-colors">
                    Seleccionar butacas
                </button>
            </div>
        </div>
    `;

    card.querySelector('.select-seats-btn')
        .addEventListener('click', () => onSelect(event.id));

    return card;
};