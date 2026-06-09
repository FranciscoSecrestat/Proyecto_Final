import axios from 'axios';

const API_URL = 'https://localhost:7192/api/auth';

export async function register(data) {
  try {
    const response = await axios.post(`${API_URL}/register`, data);
    return response.data;
  } catch (error) {
    throw new Error(error.response?.data?.message || 'Error en el registro');
  }
}

export async function login(data) {
  try {
    const response = await axios.post(`${API_URL}/login`, data);
    return response.data;
  } catch (error) {
    throw new Error(error.response?.data?.message || 'Error en el login');
  }
}

export function logout() {
  localStorage.removeItem('token');
  localStorage.removeItem('user');
}

export function isAuthenticated() {
  return !!localStorage.getItem('token');
}

export function getToken() {
  return localStorage.getItem('token');
}

export function getUser() {
  const user = localStorage.getItem('user');
  return user ? JSON.parse(user) : null;
}
