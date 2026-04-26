const BASE_URL = 'http://localhost:5280/api/v1/events'; // Cambiado a 7253 y https

export const getSeatsByEvent = async (eventId) => {
    try {
        const response = await fetch(`${BASE_URL}/${eventId}/seats`);
        if (!response.ok) throw new Error('Error al obtener asientos');
        return await response.json();
    } catch (error) {
        console.error("SeatService Error:", error);
        throw error;
    }
};