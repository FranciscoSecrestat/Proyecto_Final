<template>
  <div class="container">
    <h2>📊 Estado de mi Cartera</h2>

    <div v-if="loading" class="loading">Cargando datos...</div>

    <div v-else-if="portfolio.length === 0" class="empty">
      No tenés criptomonedas en tu cartera actualmente.
    </div>

    <div v-else>
      <table class="table">
        <thead>
          <tr>
            <th>Criptomoneda</th>
            <th>Cantidad</th>
            <th>Precio Actual</th>
            <th>Valor Total (ARS)</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="item in portfolio" :key="item.cryptoCode">
            <td>{{ item.cryptoCode }}</td>
            <td>{{ item.amount }}</td>
            <td>$ {{ item.currentPrice?.toLocaleString('es-AR') }}</td>
            <td>$ {{ item.totalValue?.toLocaleString('es-AR') }}</td>
          </tr>
        </tbody>
        <tfoot>
          <tr>
            <td colspan="3"><strong>Total</strong></td>
            <td><strong>$ {{ total?.toLocaleString('es-AR') }}</strong></td>
          </tr>
        </tfoot>
      </table>
    </div>
  </div>
</template>

<script>
import axios from 'axios';
import { getUser } from '@/services/authService';

const API_URL = 'https://localhost:7192/api';

export default {
  data() {
    return {
      portfolio: [],
      total: 0,
      loading: true
    };
  },
  async mounted() {
    const user = getUser();
    if (!user) return;

    try {
      const token = localStorage.getItem('token');
      const response = await axios.get(`${API_URL}/portfolio/user/${user.id}`, {
        headers: { Authorization: `Bearer ${token}` }
      });
      this.portfolio = response.data.portfolio;
      this.total = response.data.total;
    } catch (error) {
      console.error('Error cargando portfolio:', error);
    } finally {
      this.loading = false;
    }
  }
};
</script>

<style scoped>
.container { padding: 30px; max-width: 900px; margin: 0 auto; }
h2 { margin-bottom: 30px; color: #333; }
.table { width: 100%; border-collapse: collapse; background: white; border-radius: 10px; box-shadow: 0 4px 15px rgba(0,0,0,0.1); overflow: hidden; }
.table th { background: #667eea; color: white; padding: 14px; text-align: left; }
.table td { padding: 14px; border-bottom: 1px solid #eee; }
.table tfoot td { background: #f5f5f5; font-size: 1.1rem; }
.table tr:hover td { background: #f9f9f9; }
.loading, .empty { text-align: center; padding: 40px; color: #999; }
</style>