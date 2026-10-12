import axios from "axios";

const baseUrl = 'http://localhost:5000/api/v1';

export const apiService = axios.create({
    baseURL: baseUrl,
    withCredentials: true
});

apiService.interceptors.request.use(
    (config) => {
        const token = localStorage.getItem('token');
        if (token) {
            config.headers.Authorization = `Bearer ${token}`;
        }
        return config;
    },
    (error) => {
        return Promise.reject(error);
    }
);

apiService.interceptors.response.use(
    (response) => response,
    (error) => {
        if (error.response?.status === 401) {
            localStorage.removeItem('token');

            if (window.location.pathname !== '/auth' && window.location.pathname !== '/auth/register') {
                window.location.href = '/auth';
            }
        }

        return Promise.reject(error);
    }
);