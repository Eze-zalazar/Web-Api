const BASE_URL = 'http://localhost:5280/api/v1/reservations'; // Cambiado a 7253 y https// Cambiado a 7253 y https

export const createReservation = async (seatId, userId) => {
    try {
        const response = await fetch(BASE_URL, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ seatId, userId })
        });

        const data = await response.json();

        if (response.status === 409) {
            // Este es el caso de "asiento ocupado" o error de concurrencia
            throw { status: 409, message: data.error };
        }

        if (!response.ok) {
            throw { status: response.status, message: data.error || "Error en el servidor" };
        }

        return data; // Retorna el 201 Created
    } catch (error) {
        throw error;
    }
};