<template>
  <div class="container">
    <div class="header">
      <h2>Historial de Transacciones</h2>
      <button @click="$router.push('/transactions/new')" class="btn-new">
        + Nueva Transacción
      </button>
    </div>

    <div v-if="loading">Cargando...</div>

    <div v-else-if="transactions.length === 0" class="empty">
      No hay transacciones registradas.
    </div>

    <table v-else class="table">
      <thead>
        <tr>
          <th>Fecha</th>
          <th>Acción</th>
          <th>Cripto</th>
          <th>Cantidad</th>
          <th>Monto (ARS)</th>
          <th>Acciones</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="t in transactions" :key="t.id">
          <td>{{ formatDate(t.transactionDate) }}</td>
          <td>
            <span :class="t.action === 'purchase' ? 'badge-buy' : 'badge-sell'">
              {{ t.action === 'purchase' ? 'Compra' : 'Venta' }}
            </span>
          </td>
          <td>{{ t.cryptoCode.toUpperCase() }}</td>
          <td>{{ t.cryptoAmount }}</td>
          <td>$ {{ t.money?.toLocaleString('es-AR') }}</td>
          <td class="actions">
            <button @click="$router.push(`/transactions/${t.id}`)" class="btn-view">Ver</button>
            <button @click="$router.push(`/transactions/edit/${t.id}`)" class="btn-edit">Editar</button>
            <button @click="confirmDelete(t.id)" class="btn-delete">Borrar</button>
          </td>
        </tr>
      </tbody>
    </table>

    <!-- Modal de confirmación -->
    <div v-if="showModal" class="modal-overlay">
      <div class="modal">
        <p>¿Estás seguro que querés borrar esta transacción?</p>
        <div class="modal-buttons">
          <button @click="handleDelete" class="btn-delete">Sí, borrar</button>
          <button @click="showModal = false" class="btn-cancel">Cancelar</button>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import { getUserTransactions, deleteTransaction } from '@/services/transactionService';

export default {
  data() {
    return {
      transactions: [],
      loading: true,
      showModal: false,
      deleteId: null
    };
  },
  async mounted() {
    await this.loadTransactions();
  },
  methods: {
    async loadTransactions() {
      try {
        const user = JSON.parse(localStorage.getItem('user'));
        this.transactions = await getUserTransactions(user.id);
      } catch (e) {
        console.error(e);
      } finally {
        this.loading = false;
      }
    },
    formatDate(date) {
      return new Date(date).toLocaleString('es-AR');
    },
    confirmDelete(id) {
      this.deleteId = id;
      this.showModal = true;
    },
    async handleDelete() {
      await deleteTransaction(this.deleteId);
      this.showModal = false;
      await this.loadTransactions();
    }
  }
};
</script>

<style scoped>
.container { padding: 30px; max-width: 1000px; margin: 0 auto; }
.header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 20px; }
.btn-new { padding: 10px 20px; background: #667eea; color: white; border: none; border-radius: 5px; cursor: pointer; font-weight: bold; }
.table { width: 100%; border-collapse: collapse; }
.table th, .table td { padding: 12px; text-align: left; border-bottom: 1px solid #ddd; }
.table th { background: #f5f5f5; font-weight: 600; }
.badge-buy { background: #d4edda; color: #155724; padding: 4px 10px; border-radius: 20px; font-size: 13px; }
.badge-sell { background: #f8d7da; color: #721c24; padding: 4px 10px; border-radius: 20px; font-size: 13px; }
.actions { display: flex; gap: 8px; }
.btn-edit { padding: 6px 12px; background: #ffc107; border: none; border-radius: 4px; cursor: pointer; }
.btn-delete { padding: 6px 12px; background: #dc3545; color: white; border: none; border-radius: 4px; cursor: pointer; }
.btn-cancel { padding: 6px 12px; background: #eee; border: none; border-radius: 4px; cursor: pointer; }
.empty { text-align: center; color: #888; margin-top: 40px; }
.modal-overlay { position: fixed; top: 0; left: 0; width: 100%; height: 100%; background: rgba(0,0,0,0.5); display: flex; justify-content: center; align-items: center; }
.modal { background: white; padding: 30px; border-radius: 10px; text-align: center; }
.modal-buttons { display: flex; gap: 10px; justify-content: center; margin-top: 20px; }
.btn-view { padding: 6px 12px; background: #17a2b8; color: white; border: none; border-radius: 4px; cursor: pointer; }
</style>