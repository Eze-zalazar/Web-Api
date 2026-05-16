const BASE_URL = 'http://localhost:5280/api/v1/events';

export const createEvent = async (eventData) => {
    const response = await fetch(BASE_URL, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(eventData),
        credentials: 'include'
    });

    const data = await response.json();

    if (response.status === 403) {
        throw { status: 403, message: data.error || "No tenés permisos para crear eventos." };
    }

    if (!response.ok) {
        throw { status: response.status, message: data.error || "Error al crear el evento." };
    }

    return data;
};
