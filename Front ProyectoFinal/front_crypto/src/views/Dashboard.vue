<template>
  <div class="dashboard">
    <h1>Dashboard - Bienvenido {{ userName }}</h1>
    
    <div class="dashboard-content">
      <div class="cards-container">
        <div class="card">
          <h2>💰 Criptomonedas</h2>
          <p>Total: {{ cryptos.length }}</p>
          <router-link to="/transactions/new" class="btn-card">Ver Más</router-link>
        </div>
        
        <div class="card">
          <h2>📊 Transacciones</h2>
          <p>Total: {{ transactions.length }}</p>
          <router-link to="/transactions" class="btn-card">Ver Más</router-link>
        </div>
        
        <div class="card">
          <h2>📈 Saldo</h2>
          <p>$ {{ totalBalance }}</p>
          <router-link to="/deposit" class="btn-card">Detalles</router-link>
        </div>
      </div>

      <div class="tables-container">
        <div class="crypto-list">
          <h3>Criptomonedas Disponibles</h3>
          <div v-if="loading" class="loading">Cargando...</div>
          <div v-else-if="cryptos.length === 0" class="no-data">No hay criptomonedas disponibles</div>
          <table v-else class="table">
            <thead>
              <tr>
                <th>Nombre</th>
                <th>Código</th>
                <th>Precio Actual</th>
                <th>Acciones</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="crypto in cryptos" :key="crypto.id">
                <td>{{ crypto.name }}</td>
                <td>{{ crypto.code }}</td>
                <td>$ {{ crypto.currentPrice?.toLocaleString('es-AR') }}</td>
                <td>
                  <button @click="$router.push('/transactions/new')" class="btn-small">
                    Comprar/Vender
                  </button>
                </td>
              </tr>
            </tbody>
          </table>
        </div>

        <div class="transaction-list">
          <h3>Últimas Transacciones</h3>
          <div v-if="loading" class="loading">Cargando...</div>
          <div v-else-if="transactions.length === 0" class="no-data">No hay transacciones</div>
          <table v-else class="table">
            <thead>
              <tr>
                <th>Criptomoneda</th>
                <th>Tipo</th>
                <th>Cantidad</th>
                <th>Monto (ARS)</th>
                <th>Fecha</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="transaction in transactions.slice(0, 5)" :key="transaction.id">
                <td>{{ transaction.cryptoCode?.toUpperCase() }}</td>
                <td>{{ transaction.action === 'purchase' ? 'Compra' : 'Venta' }}</td>
                <td>{{ transaction.cryptoAmount }}</td>
                <td>$ {{ transaction.money?.toLocaleString('es-AR') }}</td>
                <td>{{ formatDate(transaction.transactionDate) }}</td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import { getCryptoCurrencies, getUserTransactions } from '@/services/api';
import { getUser } from '@/services/authService';
import axios from 'axios';

export default {
  data() {
    return {
      cryptos: [],
      transactions: [],
      loading: true,
      user: null,
      totalBalance: 0
    };
  },
  computed: {
    userName() {
      return this.user ? this.user.name : 'Usuario';
    }
  },
  async mounted() {
    this.user = getUser();
    await this.loadData();
  },
  methods: {
    async loadData() {
      this.loading = true;
      try {
        this.cryptos = await getCryptoCurrencies();
        if (this.user) {
          this.transactions = await getUserTransactions(this.user.id);

          const token = localStorage.getItem('token');
          const response = await axios.get('https://localhost:7192/api/auth/balance', {
            headers: { Authorization: `Bearer ${token}` }
          });
          this.totalBalance = response.data.balance.toLocaleString('es-AR');
        }
      } catch (error) {
        console.error('Error loading data:', error);
      } finally {
        this.loading = false;
      }
    },
    formatDate(date) {
      return new Date(date).toLocaleDateString('es-AR');
    }
  }
};
</script>

<style scoped>
.dashboard { padding: 20px; }
.dashboard h1 { color: #333; margin-bottom: 30px; text-align: center; }
.cards-container { display: grid; grid-template-columns: repeat(auto-fit, minmax(250px, 1fr)); gap: 20px; margin-bottom: 40px; }
.card { background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: white; padding: 20px; border-radius: 10px; box-shadow: 0 4px 15px rgba(0,0,0,0.1); text-align: center; transition: transform 0.3s; }
.card:hover { transform: translateY(-5px); }
.card h2 { font-size: 1.5rem; margin-bottom: 10px; }
.card p { font-size: 1.2rem; margin-bottom: 15px; }
.btn-card { display: inline-block; background: white; color: #667eea; padding: 10px 20px; border-radius: 5px; text-decoration: none; font-weight: bold; }
.btn-card:hover { background: #f0f0f0; }
.tables-container { display: grid; grid-template-columns: 1fr 1fr; gap: 20px; }
.crypto-list, .transaction-list { background: white; padding: 20px; border-radius: 10px; box-shadow: 0 4px 15px rgba(0,0,0,0.1); }
.crypto-list h3, .transaction-list h3 { color: #333; margin-bottom: 20px; }
.table { width: 100%; border-collapse: collapse; }
.table th { background: #f5f5f5; color: #333; padding: 12px; text-align: left; font-weight: bold; border-bottom: 2px solid #667eea; }
.table td { padding: 12px; border-bottom: 1px solid #eee; }
.table tr:hover { background: #f9f9f9; }
.btn-small { background: #667eea; color: white; border: none; padding: 8px 12px; border-radius: 5px; cursor: pointer; }
.btn-small:hover { background: #5568d3; }
.loading, .no-data { text-align: center; padding: 20px; color: #999; }
@media (max-width: 1024px) { .tables-container { grid-template-columns: 1fr; } }
@media (max-width: 768px) { .cards-container { grid-template-columns: 1fr; } }
</style>