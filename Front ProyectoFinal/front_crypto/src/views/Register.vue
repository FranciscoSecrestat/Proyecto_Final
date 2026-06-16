<template>
  <div class="auth-container">
    <div class="auth-box">
      <h2>Crear Cuenta</h2>
      <form @submit.prevent="handleRegister">
        <div class="form-group">
          <label>Nombre:</label>
          <input 
            v-model="form.name" 
            type="text" 
            required
            placeholder="Tu nombre"
          />
        </div>

        <div class="form-group">
          <label>Email:</label>
          <input 
            v-model="form.email" 
            type="email" 
            required
            placeholder="tu@email.com"
          />
        </div>

        <div class="form-group">
          <label>Contraseña:</label>
          <input 
            v-model="form.password" 
            type="password" 
            required
            placeholder="Contraseña"
          />
        </div>

        <div class="form-group">
          <label>Confirmar Contraseña:</label>
          <input 
            v-model="form.confirmPassword" 
            type="password" 
            required
            placeholder="Confirmar contraseña"
          />
        </div>

        <button type="submit" class="btn-register" :disabled="loading">
          {{ loading ? 'Registrando...' : 'Registrarse' }}
        </button>
        
        <p class="text-center">
          ¿Ya tienes cuenta? 
          <router-link to="/login">Inicia sesión</router-link>
        </p>
      </form>

      <div v-if="message" :class="['message', messageType]">
        {{ message }}
      </div>
    </div>
  </div>
</template>

<script>
import { register } from '@/services/authService';

export default {
  data() {
    return {
      form: {
        name: '',
        email: '',
        password: '',
        confirmPassword: ''
      },
      message: '',
      messageType: '',
      loading: false
    };
  },
  methods: {
    async handleRegister() {
      this.loading = true;
      this.message = '';
      const passwordRegex = /^(?=.*[A-Z])(?=.*\d).{8,}$/;
      if (!passwordRegex.test(this.form.password)) {
        this.message = 'La contraseña debe tener al menos 8 caracteres, una mayúscula y un número';
        this.messageType = 'error';
        this.loading = false;
        return;
      }

      try {
        const response = await register(this.form);
        
        
        if (response.success) {
          localStorage.setItem('token', response.token);
          localStorage.setItem('user', JSON.stringify(response.user));
          
          this.message = '¡Registrado exitosamente!';
          this.messageType = 'success';
          
          setTimeout(() => {
            this.$router.push('/dashboard');
          }, 1500);
        } else {
          this.message = response.message;
          this.messageType = 'error';
        }
      } catch (error) {
        this.message = 'Error: ' + error.message;
        this.messageType = 'error';
      } finally {
        this.loading = false;
      }
    }
  }
};
</script>

<style scoped>
.auth-container {
  display: flex;
  justify-content: center;
  align-items: center;
  min-height: 100vh;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
}

.auth-box {
  background: white;
  padding: 40px;
  border-radius: 10px;
  box-shadow: 0 10px 25px rgba(0, 0, 0, 0.2);
  width: 100%;
  max-width: 400px;
}

.auth-box h2 {
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

.form-group input {
  width: 100%;
  padding: 12px;
  border: 1px solid #ddd;
  border-radius: 5px;
  font-size: 14px;
  transition: border-color 0.3s;
}

.form-group input:focus {
  outline: none;
  border-color: #667eea;
}

.btn-register {
  width: 100%;
  padding: 12px;
  background: #667eea;
  color: white;
  border: none;
  border-radius: 5px;
  font-size: 16px;
  font-weight: bold;
  cursor: pointer;
  transition: background 0.3s;
}

.btn-register:hover:not(:disabled) {
  background: #5568d3;
}

.btn-register:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

.text-center {
  text-align: center;
  margin-top: 20px;
  color: #666;
}

.text-center a {
  color: #667eea;
  text-decoration: none;
  font-weight: bold;
}

.message {
  margin-top: 20px;
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
</style>
