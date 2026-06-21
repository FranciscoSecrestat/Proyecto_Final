<template>
  <div class="container">
    <div class="form-box">
      <h2>{{ isEditing ? 'Editar Transacción' : 'Nueva Transacción' }}</h2>

      <div class="form-group">
        <label>Acción:</label>
        <select v-model="form.action">
          <option value="purchase">Compra</option>
          <option value="sale">Venta</option>
        </select>
      </div>

      <div class="form-group">
        <label>Criptomoneda:</label>
        <select v-model="form.cryptoCode">
          <option value="btc">Bitcoin (BTC)</option>
          <option value="usdt">USDT</option>
          <option value="eth">Ethereum (ETH)</option>
        </select>
      </div>

      <div class="form-group">
        <label>Cantidad:</label>
        <input
          v-model="form.cryptoAmount"
          type="number"
          step="0.00000001"
          min="0"
          placeholder="Ej: 0.00070"
        />
      </div>

      <div class="form-group">
        <label>Fecha y hora:</label>
        <input v-model="form.dateTime" type="datetime-local" />
      </div>

      <div v-if="error" class="message error">{{ error }}</div>
      <div v-if="success" class="message success">{{ success }}</div>

      <div class="buttons">
        <button @click="handleSubmit" :disabled="loading" class="btn-primary">
          {{ loading ? 'Guardando...' : isEditing ? 'Guardar Cambios' : 'Confirmar' }}
        </button>
        <button @click="$router.push('/transactions')" class="btn-secondary">
          Cancelar
        </button>
      </div>
    </div>
  </div>
</template>

<script>
import { createTransaction, getTransactionById, updateTransaction } from '@/services/transactionService';

export default {
  data() {
    return {
      form: {
        action: 'purchase',
        cryptoCode: 'btc',
        cryptoAmount: '',
        dateTime: new Date().toISOString().slice(0, 16)
      },
      loading: false,
      error: '',
      success: '',
      isEditing: false
    };
  },
  async mounted() {
    const id = this.$route.params.id;
    if (id) {
      this.isEditing = true;
      const transaction = await getTransactionById(id);
      this.form = {
        action: transaction.action,
        cryptoCode: transaction.cryptoCode,
        cryptoAmount: transaction.cryptoAmount,
        dateTime: new Date(transaction.transactionDate).toISOString().slice(0, 16)
      };
    }
  },
  methods: {
    async handleSubmit() {
      this.error = '';
      this.success = '';

      if (!this.form.cryptoAmount || this.form.cryptoAmount <= 0) {
        this.error = 'La cantidad debe ser mayor a 0';
        return;
      }

      this.loading = true;
      try {
        if (this.isEditing) {
          await updateTransaction(this.$route.params.id, this.form);
          this.success = 'Transacción actualizada exitosamente';
        } else {
          await createTransaction(this.form);
          this.success = `${this.form.action === 'purchase' ? 'Compra' : 'Venta'} registrada exitosamente`;
        }
        setTimeout(() => this.$router.push('/transactions'), 1500);
      } catch (err) {
        this.error = err.message;
      } finally {
        this.loading = false;
      }
    }
  }
};
</script>

<style scoped>
.container {
  display: flex;
  justify-content: center;
  align-items: center;
  min-height: 100vh;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
}

.form-box {
  background: white;
  padding: 40px;
  border-radius: 10px;
  box-shadow: 0 10px 25px rgba(0,0,0,0.2);
  width: 100%;
  max-width: 450px;
}

h2 {
  text-align: center;
  margin-bottom: 30px;
  color: #333;
}

.form-group {
  margin-bottom: 20px;
}

.form-group label {
  display: block;
  margin-bottom: 8px;
  color: #555;
  font-weight: 500;
}

.form-group input,
.form-group select {
  width: 100%;
  padding: 12px;
  border: 1px solid #ddd;
  border-radius: 5px;
  font-size: 14px;
}

.form-group input:focus,
.form-group select:focus {
  outline: none;
  border-color: #667eea;
}

.buttons {
  display: flex;
  gap: 10px;
  margin-top: 20px;
}

.btn-primary {
  flex: 1;
  padding: 12px;
  background: #667eea;
  color: white;
  border: none;
  border-radius: 5px;
  font-size: 16px;
  font-weight: bold;
  cursor: pointer;
}

.btn-primary:hover:not(:disabled) {
  background: #5568d3;
}

.btn-primary:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

.btn-secondary {
  flex: 1;
  padding: 12px;
  background: #eee;
  color: #333;
  border: none;
  border-radius: 5px;
  font-size: 16px;
  cursor: pointer;
}

.btn-secondary:hover {
  background: #ddd;
}

.message {
  padding: 12px;
  border-radius: 5px;
  text-align: center;
  margin-bottom: 15px;
}

.success {
  background: #d4edda;
  color: #155724;
  border: 1px solid #c3e6cb;
}

.error {
  background: #f8d7da;
  color: #721c24;
  border: 1px solid #f5c6cb;
}
</style>