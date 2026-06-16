<template>
  <div id="app">
    <nav class="navbar">
      <div class="nav-container">
        <router-link to="/" class="nav-logo">💰 CryptoApp</router-link>
        <ul class="nav-menu" :class="{ active: mobileMenuOpen }">
          <li class="nav-item" v-if="!isAuthenticated">
            <router-link to="/login" class="nav-link">Iniciar Sesión</router-link>
          </li>
          <li class="nav-item" v-if="!isAuthenticated">
            <router-link to="/register" class="nav-link">Registrarse</router-link>
          </li>
          <li class="nav-item" v-if="isAuthenticated">
            <router-link to="/dashboard" class="nav-link">Dashboard</router-link>
          </li>
          <li class="nav-item" v-if="isAuthenticated">
            <router-link to="/transactions" class="nav-link">Transacciones</router-link>
          </li>
          <li class="nav-item" v-if="isAuthenticated">
            <router-link to="/portfolio" class="nav-link">Mi Cartera</router-link>
          </li>
          <li class="nav-item" v-if="isAuthenticated">
            <router-link to="/deposit" class="nav-link">Cargar Saldo</router-link>
          </li>
          <li class="nav-item" v-if="isAuthenticated">
            <span class="nav-link user-name">{{ userName }}</span>
          </li>
          <li class="nav-item" v-if="isAuthenticated">
            <button @click="logout" class="nav-link logout-btn">Cerrar Sesión</button>
          </li>
          
        </ul>
        <div class="hamburger" @click="mobileMenuOpen = !mobileMenuOpen">
          <span></span>
          <span></span>
          <span></span>
        </div>
      </div>
    </nav>

    <main class="main-content">
      <router-view />
    </main>

    <footer class="footer">
      <p>&copy; 2025 CryptoApp. Todos los derechos reservados.</p>
    </footer>
  </div>
</template>

<script>
import { isAuthenticated, getUser, logout as logoutService } from '@/services/authService';

export default {
  data() {
    return {
      mobileMenuOpen: false,
      authenticated: isAuthenticated(),
      currentUser: getUser()
    };
  },
  computed: {
    isAuthenticated() {
      return this.authenticated;
    },
    userName() {
      return this.currentUser ? this.currentUser.name : '';
    }
  },
  methods: {
    logout() {
      logoutService();
      this.authenticated = false;
      this.currentUser = null;
      this.$router.push('/login');
    }
  },
  watch: {
    $route() {
      this.authenticated = isAuthenticated();
      this.currentUser = getUser();
    }
  }
};
</script>

<style scoped>
* {
  margin: 0;
  padding: 0;
  box-sizing: border-box;
}

#app {
  display: flex;
  flex-direction: column;
  min-height: 100vh;
}

/* Navbar */
.navbar {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  padding: 1rem 0;
  box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
  position: sticky;
  top: 0;
  z-index: 1000;
}

.nav-container {
  display: flex;
  justify-content: space-between;
  align-items: center;
  max-width: 1200px;
  margin: 0 auto;
  padding: 0 20px;
}

.nav-logo {
  color: white;
  font-size: 1.5rem;
  font-weight: bold;
  text-decoration: none;
  transition: transform 0.3s;
}

.nav-logo:hover {
  transform: scale(1.05);
}

.nav-menu {
  display: flex;
  list-style: none;
  gap: 2rem;
  align-items: center;
}

.nav-item {
  margin: 0;
}

.nav-link {
  color: white;
  text-decoration: none;
  font-weight: 500;
  transition: opacity 0.3s;
  border: none;
  background: none;
  cursor: pointer;
  font-size: 1rem;
}

.nav-link:hover {
  opacity: 0.8;
}

.user-name {
  background: rgba(255, 255, 255, 0.2);
  padding: 0.5rem 1rem;
  border-radius: 20px;
}

.logout-btn {
  background: rgba(255, 255, 255, 0.3);
  padding: 0.5rem 1rem;
  border-radius: 5px;
  transition: background 0.3s;
}

.logout-btn:hover {
  background: rgba(255, 255, 255, 0.5);
}

/* Hamburger Menu */
.hamburger {
  display: none;
  flex-direction: column;
  cursor: pointer;
}

.hamburger span {
  width: 25px;
  height: 3px;
  background: white;
  margin: 5px 0;
  transition: 0.3s;
}

/* Main Content */
.main-content {
  flex: 1;
  max-width: 1200px;
  margin: 2rem auto;
  width: 100%;
  padding: 0 20px;
}

/* Footer */
.footer {
  background: #333;
  color: white;
  text-align: center;
  padding: 2rem;
  margin-top: auto;
}

/* Mobile Responsive */
@media (max-width: 768px) {
  .hamburger {
    display: flex;
  }

  .nav-menu {
    position: fixed;
    left: -100%;
    top: 70px;
    flex-direction: column;
    background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
    width: 100%;
    text-align: center;
    transition: 0.3s;
    gap: 1rem;
    padding: 2rem 0;
  }

  .nav-menu.active {
    left: 0;
  }

  .main-content {
    margin: 1rem auto;
  }
}
</style>