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
        jonas: 'https://hips.hearstapps.com/hmg-prod/images/2-68f15344b6337.jpg'
    };

    const name = event.name.toLowerCase();

    let bgUrl = '';

    if (name.includes('babasónicos') || name.includes('babasonicos')) {
        bgUrl = backgrounds.babasonicos;
    } else if (name.includes('piojos')) {
        bgUrl = backgrounds.piojos;
    } else if (name.includes('jonas')) {
        bgUrl = backgrounds.jonas;
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
                    <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                        d="M17.657 16.657L13.414 20.9a1.998 1.998 0 01-2.827 0l-4.244-4.243a8 8 0 1111.314 0z"/>
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                        d="M15 11a3 3 0 11-6 0 3 3 0 016 0z"/>
                    </svg>
                    <span>${event.venue}</span>
                </div>

                <div class="flex items-center text-gray-500 text-sm gap-2">
                    <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                        d="M8 7V3m8 4V3m-9 8h10M5 21h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v12a2 2 0 002 2z"/>
                    </svg>
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
                    <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                        d="M14 5l7 7m0 0l-7 7m7-7H3"/>
                    </svg>
                </button>
            </div>
        </div>
    `;

    card.querySelector('.select-seats-btn')
        .addEventListener('click', () => onSelect(event.id));

    return card;
};