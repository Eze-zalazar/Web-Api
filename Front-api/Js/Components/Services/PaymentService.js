const BASE_URL = 'http://localhost:5280/api/v1/payments';

export const processPayment = async (reservationId, userId) => {
    try {
        const response = await fetch(BASE_URL, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ reservationId, userId }),
            credentials: 'include'
        });

        const data = await response.json();

        if (!response.ok) {
            throw { status: response.status, message: data.error || "Error al procesar el pago" };
        }

        return data;
    } catch (error) {
        throw error;
    }
};
