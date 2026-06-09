<template>
  <div class="transactions-page">
    <h1>Mis Transacciones</h1>
    
    <div class="filters">
      <button 
        @click="filter = 'all'"
        :class="['filter-btn', { active: filter === 'all' }]"
      >
        Todas
      </button>
      <button 
        @click="filter = 'buy'"
        :class="['filter-btn', { active: filter === 'buy' }]"
      >
        Compras
      </button>
      <button 
        @click="filter = 'sell'"
        :class="['filter-btn', { active: filter === 'sell' }]"
      >
        Ventas
      </button>
    </div>

    <div v-if="loading" class="loading">Cargando transacciones...</div>
    <div v-else-if="filteredTransactions.length === 0" class="no-data">
      No hay transacciones
    </div>
    <table v-else class="transaction-table">
      <thead>
        <tr>
          <th>ID</th>
          <th>Criptomoneda</th>
          <th>Tipo</th>
          <th>Cantidad</th>
          <th>Precio</th>
          <th>Total</th>
          <th>Fecha</th>
          <th>Acciones</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="transaction in filteredTransactions" :key="transaction.id">
          <td>{{ transaction.id }}</td>
          <td>{{ transaction.cryptoCurrencyId }}</td>
          <td :class="transaction.transactionTypeId === 1 ? 'buy' : 'sell'">
            {{ transaction.transactionTypeId === 1 ? 'Compra' : 'Venta' }}
          </td>
          <td>{{ transaction.amount }}</td>
          <td>${{ transaction.price }}</td>
          <td>${{ (transaction.amount * transaction.price).toFixed(2) }}</td>
          <td>{{ formatDate(transaction.transactionDate) }}</td>
          <td>
            <button @click="deleteTransaction(transaction.id)" class="btn-delete">
              Eliminar
            </button>
          </td>
        </tr>
      </tbody>
    </table>
  </div>
</template>

<script>
import { getUserTransactions, deleteTransaction } from '@/services/api';
import { getUser } from '@/services/authService';

export default {
  data() {
    return {
      transactions: [],
      loading: true,
      filter: 'all'
    };
  },
  computed: {
    filteredTransactions() {
      if (this.filter === 'buy') {
        return this.transactions.filter(t => t.transactionTypeId === 1);
      } else if (this.filter === 'sell') {
        return this.transactions.filter(t => t.transactionTypeId === 2);
      }
      return this.transactions;
    }
  },
  async mounted() {
    const user = getUser();
    if (user) {
      await this.loadTransactions(user.id);
    }
  },
  methods: {
    async loadTransactions(userId) {
      this.loading = true;
      try {
        this.transactions = await getUserTransactions(userId);
      } catch (error) {
        console.error('Error loading transactions:', error);
      } finally {
        this.loading = false;
      }
    },
    async deleteTransaction(id) {
      if (confirm('¿Estás seguro de que deseas eliminar esta transacción?')) {
        try {
          await deleteTransaction(id);
          this.transactions = this.transactions.filter(t => t.id !== id);
        } catch (error) {
          console.error('Error deleting transaction:', error);
        }
      }
    },
    formatDate(date) {
      return new Date(date).toLocaleDateString('es-AR');
    }
  }
};
</script>

<style scoped>
.transactions-page {
  padding: 20px;
  max-width: 1200px;
  margin: 0 auto;
}

.transactions-page h1 {
  color: #333;
  margin-bottom: 30px;
  text-align: center;
}

.filters {
  display: flex;
  gap: 10px;
  margin-bottom: 20px;
  justify-content: center;
}

.filter-btn {
  padding: 10px 20px;
  border: 2px solid #667eea;
  background: white;
  color: #667eea;
  border-radius: 5px;
  cursor: pointer;
  font-weight: bold;
  transition: all 0.3s;
}

.filter-btn.active {
  background: #667eea;
  color: white;
}

.transaction-table {
  width: 100%;
  border-collapse: collapse;
  background: white;
  border-radius: 10px;
  overflow: hidden;
  box-shadow: 0 4px 15px rgba(0, 0, 0, 0.1);
}

.transaction-table th {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  padding: 15px;
  text-align: left;
  font-weight: bold;
}

.transaction-table td {
  padding: 12px 15px;
  border-bottom: 1px solid #eee;
}

.transaction-table tr:hover {
  background: #f9f9f9;
}

.buy {
  color: #28a745;
  font-weight: bold;
}

.sell {
  color: #dc3545;
  font-weight: bold;
}

.btn-delete {
  background: #dc3545;
  color: white;
  border: none;
  padding: 8px 12px;
  border-radius: 5px;
  cursor: pointer;
  transition: background 0.3s;
}

.btn-delete:hover {
  background: #c82333;
}

.loading,
.no-data {
  text-align: center;
  padding: 40px;
  color: #999;
  font-size: 1.1rem;
}
</style>
