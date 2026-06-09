<template>
  <div class="crypto-detail">
    <button @click="goBack" class="btn-back">← Volver</button>
    
    <div v-if="loading" class="loading">Cargando...</div>
    <div v-else-if="crypto" class="detail-container">
      <div class="crypto-header">
        <h1>{{ crypto.name }}</h1>
        <span class="code-badge">{{ crypto.code.toUpperCase() }}</span>
      </div>

      <div class="crypto-info">
        <div class="info-card">
          <h3>Precio Actual</h3>
          <p class="price">${{ crypto.currentPrice }}</p>
          <small>Última actualización: {{ formatDate(crypto.lastUpdated) }}</small>
        </div>

        <div class="transaction-form">
          <h3>Realizar Transacción</h3>
          <form @submit.prevent="submitTransaction">
            <div class="form-group">
              <label>Tipo de Transacción:</label>
              <select v-model="transactionForm.transactionTypeId" required>
                <option value="">Selecciona...</option>
                <option value="1">Compra</option>
                <option value="2">Venta</option>
              </select>
            </div>

            <div class="form-group">
              <label>Cantidad:</label>
              <input 
                v-model.number="transactionForm.amount" 
                type="number" 
                step="0.00000001"
                required
                placeholder="0.00"
              />
            </div>

            <div class="form-group">
              <label>Total: ${{ (transactionForm.amount * crypto.currentPrice).toFixed(2) }}</label>
            </div>

            <button type="submit" class="btn-submit" :disabled="submitting">
              {{ submitting ? 'Procesando...' : 'Confirmar Transacción' }}
            </button>
          </form>

          <div v-if="message" :class="['message', messageType]">
            {{ message }}
          </div>
        </div>
      </div>
    </div>
    <div v-else class="no-data">Criptomoneda no encontrada</div>
  </div>
</template>

<script>
import { getCryptoCurrencyById, createTransaction } from '@/services/api';
import { getUser } from '@/services/authService';

export default {
  data() {
    return {
      crypto: null,
      loading: true,
      submitting: false,
      message: '',
      messageType: '',
      transactionForm: {
        amount: 0,
        transactionTypeId: '',
        price: 0
      }
    };
  },
  async mounted() {
    const id = this.$route.params.id;
    await this.loadCrypto(id);
  },
  methods: {
    async loadCrypto(id) {
      this.loading = true;
      try {
        this.crypto = await getCryptoCurrencyById(id);
        this.transactionForm.price = this.crypto.currentPrice;
      } catch (error) {
        console.error('Error loading cryptocurrency:', error);
        this.message = 'Error al cargar la criptomoneda';
        this.messageType = 'error';
      } finally {
        this.loading = false;
      }
    },
    async submitTransaction() {
      this.submitting = true;
      this.message = '';

      try {
        const user = getUser();
        if (!user) {
          this.message = 'Debes estar autenticado';
          this.messageType = 'error';
          return;
        }

        const transaction = {
          userId: user.id,
          cryptoCurrencyId: this.crypto.id,
          transactionTypeId: parseInt(this.transactionForm.transactionTypeId),
          amount: this.transactionForm.amount,
          price: this.transactionForm.price,
          transactionDate: new Date()
        };

        await createTransaction(transaction);

        this.message = '¡Transacción realizada exitosamente!';
        this.messageType = 'success';

        // Limpiar formulario
        this.transactionForm = {
          amount: 0,
          transactionTypeId: '',
          price: this.crypto.currentPrice
        };

        setTimeout(() => {
          this.$router.push('/transactions');
        }, 2000);
      } catch (error) {
        console.error('Error creating transaction:', error);
        this.message = 'Error al realizar la transacción: ' + error.message;
        this.messageType = 'error';
      } finally {
        this.submitting = false;
      }
    },
    goBack() {
      this.$router.back();
    },
    formatDate(date) {
      return new Date(date).toLocaleDateString('es-AR');
    }
  }
};
</script>

<style scoped>
.crypto-detail {
  padding: 20px;
  max-width: 900px;
  margin: 0 auto;
}

.btn-back {
  background: #667eea;
  color: white;
  border: none;
  padding: 10px 20px;
  border-radius: 5px;
  cursor: pointer;
  font-size: 1rem;
  margin-bottom: 20px;
  transition: background 0.3s;
}

.btn-back:hover {
  background: #5568d3;
}

.detail-container {
  background: white;
  padding: 30px;
  border-radius: 10px;
  box-shadow: 0 4px 15px rgba(0, 0, 0, 0.1);
}

.crypto-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  margin-bottom: 30px;
  padding-bottom: 20px;
  border-bottom: 2px solid #f0f0f0;
}

.crypto-header h1 {
  color: #333;
  font-size: 2rem;
}

.code-badge {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  padding: 10px 20px;
  border-radius: 20px;
  font-weight: bold;
  font-size: 1.1rem;
}

.crypto-info {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 30px;
}

.info-card {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  padding: 25px;
  border-radius: 10px;
}

.info-card h3 {
  font-size: 1rem;
  opacity: 0.9;
  margin-bottom: 10px;
}

.price {
  font-size: 2.5rem;
  font-weight: bold;
  margin-bottom: 10px;
}

.info-card small {
  opacity: 0.8;
}

.transaction-form {
  background: #f9f9f9;
  padding: 25px;
  border-radius: 10px;
  border: 2px solid #e0e0e0;
}

.transaction-form h3 {
  color: #333;
  margin-bottom: 20px;
}

.form-group {
  margin-bottom: 15px;
}

.form-group label {
  display: block;
  color: #555;
  font-weight: 500;
  margin-bottom: 8px;
}

.form-group input,
.form-group select {
  width: 100%;
  padding: 12px;
  border: 1px solid #ddd;
  border-radius: 5px;
  font-size: 1rem;
  transition: border-color 0.3s;
}

.form-group input:focus,
.form-group select:focus {
  outline: none;
  border-color: #667eea;
}

.btn-submit {
  width: 100%;
  padding: 12px;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  border: none;
  border-radius: 5px;
  font-size: 1rem;
  font-weight: bold;
  cursor: pointer;
  transition: opacity 0.3s;
}

.btn-submit:hover:not(:disabled) {
  opacity: 0.9;
}

.btn-submit:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

.message {
  margin-top: 15px;
  padding: 12px;
  border-radius: 5px;
  text-align: center;
}

.message.success {
  background: #d4edda;
  color: #155724;
  border: 1px solid #c3e6cb;
}

.message.error {
  background: #f8d7da;
  color: #721c24;
  border: 1px solid #f5c6cb;
}

.loading,
.no-data {
  text-align: center;
  padding: 40px;
  color: #999;
  font-size: 1.1rem;
}

@media (max-width: 768px) {
  .crypto-info {
    grid-template-columns: 1fr;
  }

  .crypto-header {
    flex-direction: column;
    align-items: flex-start;
  }

  .code-badge {
    margin-top: 10px;
  }
}
</style>
