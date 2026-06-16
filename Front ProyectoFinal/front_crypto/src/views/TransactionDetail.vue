<template>
  <div class="container">
    <div class="detail-box" v-if="transaction">
      <h2>Detalle de Transacción</h2>

      <div class="field">
        <label>Criptomoneda:</label>
        <span>{{ transaction.cryptoCode?.toUpperCase() }}</span>
      </div>
      <div class="field">
        <label>Acción:</label>
        <span :class="transaction.action === 'purchase' ? 'badge-buy' : 'badge-sell'">
          {{ transaction.action === 'purchase' ? 'Compra' : 'Venta' }}
        </span>
      </div>
      <div class="field">
        <label>Cantidad:</label>
        <span>{{ transaction.cryptoAmount }}</span>
      </div>
      <div class="field">
        <label>Monto (ARS):</label>
        <span>$ {{ transaction.money?.toLocaleString('es-AR') }}</span>
      </div>
      <div class="field">
        <label>Fecha:</label>
        <span>{{ new Date(transaction.transactionDate).toLocaleString('es-AR') }}</span>
      </div>

      <div class="buttons">
        <button  v-if="isAdmin" @click="$router.push(`/transactions/edit/${transaction.id}`)" class="btn-edit">
          Editar
        </button>
        <button @click="$router.push('/transactions')" class="btn-back">
          Volver
        </button>
      </div>
    </div>

    <div v-else class="loading">Cargando...</div>
  </div>
</template>

<script>
import { getTransactionById } from '@/services/transactionService';

export default {
  data() {
    return { transaction: null };
  },
  async mounted() {
    this.transaction = await getTransactionById(this.$route.params.id);
  }
};
</script>

<style scoped>
.container { display: flex; justify-content: center; padding: 40px 20px; }
.detail-box { background: white; padding: 40px; border-radius: 10px; box-shadow: 0 4px 15px rgba(0,0,0,0.1); width: 100%; max-width: 500px; }
h2 { margin-bottom: 30px; color: #333; text-align: center; }
.field { display: flex; justify-content: space-between; padding: 14px 0; border-bottom: 1px solid #eee; font-size: 1rem; }
.field label { color: #666; font-weight: 500; }
.badge-buy { background: #d4edda; color: #155724; padding: 4px 12px; border-radius: 20px; }
.badge-sell { background: #f8d7da; color: #721c24; padding: 4px 12px; border-radius: 20px; }
.buttons { display: flex; gap: 10px; margin-top: 30px; }
.btn-edit { flex: 1; padding: 12px; background: #ffc107; border: none; border-radius: 5px; cursor: pointer; font-weight: bold; }
.btn-back { flex: 1; padding: 12px; background: #eee; border: none; border-radius: 5px; cursor: pointer; }
.loading { text-align: center; padding: 40px; color: #999; }
</style>