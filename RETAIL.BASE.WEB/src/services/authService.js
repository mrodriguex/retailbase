import { apiClient, normalizeApiError, setAuthToken } from "./apiClient";

const TOKEN_KEY = 'token';
const USERNAME_KEY = 'username';

export async function login(username, password) {
    try {
        const cleanUserName = username.trim() ?? '';
        const response = await apiClient.post('/api/v1/Auth/login', {
            username: cleanUserName,
            password,
        });

        const { success, data, error } = response.data ?? {};
        if (!success) {
            return { success: false, error: error?.message || 'Login failed' };
        }

        const token = typeof data === 'string' ? data : data?.token ?? null;

        if (!token) {
            return { success: false, error: 'No token received' };
        }

        localStorage.setItem(TOKEN_KEY, token);
        if (cleanUserName) {
            localStorage.setItem(USERNAME_KEY, cleanUserName);
        }
        setAuthToken(token);
        return { success: true, token };
    } catch (error) {
        const normalizedError = normalizeApiError(error, 'Login failed');
        return { success: false, error: normalizedError.message };
    }
}

export function logout() {
    localStorage.removeItem(TOKEN_KEY);
    localStorage.removeItem(USERNAME_KEY);
    setAuthToken(null);
}

export function getToken() {
    return localStorage.getItem(TOKEN_KEY);
}

export function getUsername() {
    return localStorage.getItem(USERNAME_KEY);
}

export function getCurrentUser() {
    const token = localStorage.getItem(TOKEN_KEY);
    const username = localStorage.getItem(USERNAME_KEY);
    if (token) {
        return { username, token };
    }
    return null;
}

export function isAuthenticated() {
    return !!localStorage.getItem(TOKEN_KEY);
}