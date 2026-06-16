<template>
  <div class="container">
    <div class="deposit-box">
      <h2>💵 Cargar Saldo</h2>

      <div class="balance-info">
        <p>Saldo actual: <strong>$ {{ balance.toLocaleString('es-AR') }}</strong></p>
      </div>

      <div class="form-group">
        <label>Monto a cargar (ARS):</label>
        <input
          v-model="amount"
          type="number"
          min="1"
          placeholder="Ej: 10000"
        />
      </div>

      <div v-if="error" class="message error">{{ error }}</div>
      <div v-if="success" class="message success">{{ success }}</div>

      <button @click="handleDeposit" :disabled="loading" class="btn-deposit">
        {{ loading ? 'Cargando...' : 'Cargar Saldo' }}
      </button>

      <h3>Historial de cargas</h3>
      <div v-if="deposits.length === 0" class="empty">No hay cargas registradas.</div>
      <table v-else class="table">
        <thead>
          <tr>
            <th>Fecha</th>
            <th>Monto (ARS)</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="d in deposits" :key="d.id">
            <td>{{ new Date(d.date).toLocaleString('es-AR') }}</td>
            <td>$ {{ d.amount.toLocaleString('es-AR') }}</td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>
</template>

<script>
import axios from 'axios';

const API_URL = 'https://localhost:7192/api';

export default {
  data() {
    return {
      amount: '',
      balance: 0,
      deposits: [],
      loading: false,
      error: '',
      success: ''
    };
  },
  async mounted() {
    await this.loadBalance();
    await this.loadDeposits();
  },
  methods: {
    async loadBalance() {
      try {
        const token = localStorage.getItem('token');
        const response = await axios.get(`${API_URL}/auth/balance`, {
          headers: { Authorization: `Bearer ${token}` }
        });
        this.balance = response.data.balance;
      } catch (e) {
        console.error(e);
      }
    },
    async loadDeposits() {
      try {
        const token = localStorage.getItem('token');
        const response = await axios.get(`${API_URL}/auth/deposits`, {
          headers: { Authorization: `Bearer ${token}` }
        });
        this.deposits = response.data;
      } catch (e) {
        console.error(e);
      }
    },
    async handleDeposit() {
      this.error = '';
      this.success = '';

      if (!this.amount || this.amount <= 0) {
        this.error = 'El monto debe ser mayor a 0';
        return;
      }

      this.loading = true;
      try {
        const token = localStorage.getItem('token');
        await axios.post(`${API_URL}/auth/deposit`,
          { amount: parseFloat(this.amount) },
          { headers: { Authorization: `Bearer ${token}` } }
        );
        this.success = `Se cargaron $ ${parseFloat(this.amount).toLocaleString('es-AR')} exitosamente`;
        this.amount = '';
        await this.loadBalance();
        await this.loadDeposits();
      } catch (e) {
        this.error = e.response?.data?.message || 'Error al cargar saldo';
      } finally {
        this.loading = false;
      }
    }
  }
};
</script>

<style scoped>
.container { display: flex; justify-content: center; padding: 40px 20px; }
.deposit-box { background: white; padding: 40px; border-radius: 10px; box-shadow: 0 4px 15px rgba(0,0,0,0.1); width: 100%; max-width: 550px; }
h2 { text-align: center; margin-bottom: 20px; color: #333; }
h3 { margin: 30px 0 15px; color: #333; }
.balance-info { background: #f0f4ff; padding: 15px; border-radius: 8px; text-align: center; margin-bottom: 25px; font-size: 1.1rem; color: #333; }
.form-group { margin-bottom: 20px; }
.form-group label { display: block; margin-bottom: 8px; color: #555; font-weight: 500; }
.form-group input { width: 100%; padding: 12px; border: 1px solid #ddd; border-radius: 5px; font-size: 14px; }
.form-group input:focus { outline: none; border-color: #667eea; }
.btn-deposit { width: 100%; padding: 12px; background: #28a745; color: white; border: none; border-radius: 5px; font-size: 16px; font-weight: bold; cursor: pointer; margin-bottom: 30px; }
.btn-deposit:hover:not(:disabled) { background: #218838; }
.btn-deposit:disabled { opacity: 0.6; cursor: not-allowed; }
.message { padding: 12px; border-radius: 5px; text-align: center; margin-bottom: 15px; }
.success { background: #d4edda; color: #155724; border: 1px solid #c3e6cb; }
.error { background: #f8d7da; color: #721c24; border: 1px solid #f5c6cb; }
.table { width: 100%; border-collapse: collapse; }
.table th { background: #f5f5f5; padding: 12px; text-align: left; border-bottom: 2px solid #667eea; }
.table td { padding: 12px; border-bottom: 1px solid #eee; }
.empty { text-align: center; color: #999; padding: 20px; }
</style>