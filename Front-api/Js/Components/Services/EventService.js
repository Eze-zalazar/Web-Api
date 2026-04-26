const BASE_URL = 'http://localhost:5280/api/v1/events'; // Cambiado a 7253 y https

export const getAllEvents = async (page = 1, pageSize = 10) => {
    try {
        const response = await fetch(`${BASE_URL}?page=${page}&pageSize=${pageSize}`);
        if (!response.ok) throw new Error('Error al obtener eventos');
        return await response.json();
    } catch (error) {
        console.error("EventService Error:", error);
        throw error;
    }
};

export const getEventById = async (id) => {
    try {
        const response = await fetch(`${BASE_URL}/${id}`);
        if (!response.ok) throw new Error('Evento no encontrado');
        return await response.json();
    } catch (error) {
        console.error("EventService Error:", error);
        throw error;
    }
};