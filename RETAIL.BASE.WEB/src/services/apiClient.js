import axios from 'axios';
import { env } from '../config';

const API_BASE_URL = env.apiBaseUrl;

export const apiClient = axios.create({
    baseURL: API_BASE_URL,
    headers: {
        'Content-Type': 'application/json',
    },
});

const AUTH_EXPIRED_EVENT = 'auth:expired';
let hasNotifiedAuthExpired = false;

export function setAuthToken(token) {
    if (token) {
        apiClient.defaults.headers.common['Authorization'] = `Bearer ${token}`;
        hasNotifiedAuthExpired = false;
    } else {
        delete apiClient.defaults.headers.common['Authorization'];
    }
}

//bootstrap existing session
const token = localStorage.getItem('token');
if (token) {
    setAuthToken(token);
}

function notifyAuthExpired() {
    const activeToken = localStorage.getItem('token');
    if (!activeToken || hasNotifiedAuthExpired) {
        return;
    }

    hasNotifiedAuthExpired = true;
    localStorage.removeItem('token');
    localStorage.removeItem('username');
    setAuthToken(null);
    window.dispatchEvent(
        new CustomEvent(AUTH_EXPIRED_EVENT, {
            detail: { message: 'Your session expired. Please log in again.' },
        })
    );
}

apiClient.interceptors.response.use(
    (response) => response,
    (error) => {
        const status = error?.response?.status;
        if (status === 401) {
            notifyAuthExpired();
        }
        return Promise.reject(error);
    }
);

export function unwrap(response) {
    const { success, data, error } = response.data ?? {};
    if (!success) {
        const errorMessage = error?.message || 'An unknown error occurred';
        throw new Error(errorMessage);
    }
    return data;
}

export function normalizeApiError(error, fallbackMessage = 'An error occurred while processing the request') {
    const message =
        error?.response?.data?.message ||
        error?.message ||
        fallbackMessage ||
        'Unexpected error';

    return new Error(message);
}

export { AUTH_EXPIRED_EVENT };
