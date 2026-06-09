import axios from 'axios';

const API_URL = 'https://localhost:7192/api/transactions';

function getHeaders() {
  const token = localStorage.getItem('token');
  return { Authorization: `Bearer ${token}` };
}

export async function getAllTransactions() {
  const response = await axios.get(API_URL, { headers: getHeaders() });
  return response.data;
}

export async function getUserTransactions(userId) {
  const response = await axios.get(`${API_URL}/user/${userId}`, { headers: getHeaders() });
  return response.data;
}

export async function getTransactionById(id) {
  const response = await axios.get(`${API_URL}/${id}`, { headers: getHeaders() });
  return response.data;
}

export async function createTransaction(data) {
  try {
    const response = await axios.post(API_URL, data, { headers: getHeaders() });
    return response.data;
  } catch (error) {
    throw new Error(error.response?.data?.message || 'Error al crear la transacción');
  }
}

export async function updateTransaction(id, data) {
  try {
    const response = await axios.patch(`${API_URL}/${id}`, data, { headers: getHeaders() });
    return response.data;
  } catch (error) {
    throw new Error(error.response?.data?.message || 'Error al actualizar la transacción');
  }
}

export async function deleteTransaction(id) {
  try {
    await axios.delete(`${API_URL}/${id}`, { headers: getHeaders() });
  } catch (error) {
    throw new Error(error.response?.data?.message || 'Error al eliminar la transacción');
  }
}