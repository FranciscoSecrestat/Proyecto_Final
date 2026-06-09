
<template>
  <div class="crypto-container">
    <h1>Criptomonedas</h1>
    <table class="crypto-table">
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
          <td>${{ crypto.currentPrice }}</td>
          <td>
            <button @click="selectCrypto(crypto)">Comprar/Vender</button>
          </td>
        </tr>
      </tbody>
    </table>
  </div>
</template>

<script>
import { getCryptoCurrencies } from '@/services/api';

export default {
  data() {
    return {
      cryptos: []
    };
  },
  async mounted() {
    try {
      this.cryptos = await getCryptoCurrencies();
    } catch (error) {
      console.error('Error:', error);
    }
  },
  methods: {
    selectCrypto(crypto) {
      this.$router.push({
        name: 'Transaction',
        params: { id: crypto.id }
      });
    }
  }
};
</script>

<style scoped>
.crypto-container {
  padding: 20px;
}

.crypto-table {
  width: 100%;
  border-collapse: collapse;
}

.crypto-table th,
.crypto-table td {
  padding: 10px;
  border: 1px solid #ddd;
}

.crypto-table th {
  background-color: #f2f2f2;
}
</style>