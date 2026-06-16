import axios from 'axios';
import { getToken } from './authService';

const API_URL = 'https://localhost:7192/api';

// Crear instancia de axios con configuración
const apiClient = axios.create({
  baseURL: API_URL,
  headers: {
    'Content-Type': 'application/json'
  }
});

// Interceptor para agregar token a cada request
apiClient.interceptors.request.use(
  (config) => {
    const token = getToken();
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

// Criptomonedas
export async function getCryptoCurrencies() {
  try {
    const response = await apiClient.get('/cryptocurrencies');
    return response.data;
  } catch (error) {
    console.error('Error fetching cryptocurrencies:', error);
    throw error;
  }
}

export async function getCryptoCurrencyById(id) {
  try {
    const response = await apiClient.get(`/cryptocurrencies/${id}`);
    return response.data;
  } catch (error) {
    console.error('Error fetching cryptocurrency:', error);
    throw error;
  }
}

export async function createCryptoCurrency(data) {
  try {
    const response = await apiClient.post('/cryptocurrencies', data);
    return response.data;
  } catch (error) {
    console.error('Error creating cryptocurrency:', error);
    throw error;
  }
}

export async function updateCryptoCurrency(id, data) {
  try {
    const response = await apiClient.put(`/cryptocurrencies/${id}`, data);
    return response.data;
  } catch (error) {
    console.error('Error updating cryptocurrency:', error);
    throw error;
  }
}

export async function deleteCryptoCurrency(id) {
  try {
    await apiClient.delete(`/cryptocurrencies/${id}`);
  } catch (error) {
    console.error('Error deleting cryptocurrency:', error);
    throw error;
  }
}

// Transacciones
export async function getUserTransactions(userId) {
  try {
    const response = await apiClient.get(`/transactions/user/${userId}`);
    return response.data;
  } catch (error) {
    console.error('Error fetching transactions:', error);
    throw error;
  }
}

export async function getTransactionById(id) {
  try {
    const response = await apiClient.get(`/transactions/${id}`);
    return response.data;
  } catch (error) {
    console.error('Error fetching transaction:', error);
    throw error;
  }
}

export async function getAllTransactions() {
  try {
    const response = await apiClient.get('/transactions');
    return response.data;
  } catch (error) {
    console.error('Error fetching transactions:', error);
    throw error;
  }
}

export async function createTransaction(data) {
  try {
    const response = await apiClient.post('/transactions', data);
    return response.data;
  } catch (error) {
    console.error('Error creating transaction:', error);
    throw error;
  }
}

export async function deleteTransaction(id) {
  try {
    await apiClient.delete(`/transactions/${id}`);
  } catch (error) {
    console.error('Error deleting transaction:', error);
    throw error;
  }
}

export async function getCryptoPrices() {
  const response = await axios.get('https://localhost:7192/api/cryptocurrencies/prices');
  return response.data;
}
