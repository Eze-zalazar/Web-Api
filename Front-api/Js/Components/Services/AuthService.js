const BASE_URL = 'http://localhost:5280/api/v1/auth';

export const login = async (email, password) => {
    const response = await fetch(`${BASE_URL}/login`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ email, password }),
        credentials: 'include'
    });

    const data = await response.json();

    if (!response.ok) {
        throw { status: response.status, message: data.error || "Error de autenticación" };
    }

    // Guardar usuario en localStorage
    localStorage.setItem('stagely_user', JSON.stringify(data));
    return data;
};

export const getCurrentUser = () => {
    const user = localStorage.getItem('stagely_user');
    return user ? JSON.parse(user) : null;
};

export const logout = async () => {
    localStorage.removeItem('stagely_user');
    try {
        await fetch(`${BASE_URL}/logout`, { method: 'POST', credentials: 'include' });
    } catch (error) {
        console.error("Error al cerrar sesión", error);
    }
};

export const isAdmin = () => {
    const user = getCurrentUser();
    return user?.isAdmin === true;
};
