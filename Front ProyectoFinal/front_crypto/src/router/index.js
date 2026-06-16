import { createRouter, createWebHistory } from 'vue-router';
import { isAuthenticated } from '@/services/authService';

const routes = [
  { path: '/', redirect: '/dashboard' },
  {
    path: '/register',
    name: 'Register',
    component: () => import('@/views/Register.vue'),
    meta: { requiresAuth: false }
  },
  {
    path: '/login',
    name: 'Login',
    component: () => import('@/views/Login.vue'),
    meta: { requiresAuth: false }
  },
  {
    path: '/dashboard',
    name: 'Dashboard',
    component: () => import('@/views/Dashboard.vue'),
    meta: { requiresAuth: true }
  },
  {
    path: '/transactions',
    name: 'Transactions',
    component: () => import('@/views/TransactionList.vue'),
    meta: { requiresAuth: true }
  },
  {
    path: '/transactions/new',
    name: 'NewTransaction',
    component: () => import('@/views/TransactionForm.vue'),
    meta: { requiresAuth: true }
  },
  {
    path: '/transactions/edit/:id',
    name: 'EditTransaction',
    component: () => import('@/views/TransactionForm.vue'),
    meta: { requiresAuth: true }
  },
  {
    path: '/crypto/:id',
    name: 'CryptoDetail',
    component: () => import('@/views/CryptoDetail.vue'),
    meta: { requiresAuth: true }
  },
  {
    path: '/portfolio',
    name: 'Portfolio',
    component: () => import('@/views/Portfolio.vue'),
    meta: { requiresAuth: true }
  },
  {
    path: '/transactions/:id',
    name: 'TransactionDetail',
    component: () => import('@/views/TransactionDetail.vue'),
    meta: { requiresAuth: true }
  }
];

const router = createRouter({
  history: createWebHistory(),
  routes
});

// Guard para proteger rutas
router.beforeEach((to, from, next) => {
  const authenticated = isAuthenticated();

  if (to.meta.requiresAuth && !authenticated) {
    next('/login');
  } else if ((to.path === '/login' || to.path === '/register') && authenticated) {
    next('/dashboard');
  } else {
    next();
  }
});

export default router;
