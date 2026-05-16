const BASE_URL = 'http://localhost:5280/api/v1/reservations';

export const createReservation = async (seatId, userId) => {
    try {
        const response = await fetch(BASE_URL, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ seatId, userId }),
            credentials: 'include'
        });

        const data = await response.json();

        if (response.status === 409) {
            throw { status: 409, message: data.error };
        }

        if (!response.ok) {
            throw { status: response.status, message: data.error || "Error en el servidor" };
        }

        return data;
    } catch (error) {
        throw error;
    }
};

export const getUserReservations = async (userId) => {
    const response = await fetch(`${BASE_URL}/user/${userId}`, { credentials: 'include' });
    if (!response.ok) {
        throw new Error("Error al obtener las reservas.");
    }
    return await response.json();
};