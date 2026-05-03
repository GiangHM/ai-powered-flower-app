import { createRouter, createWebHistory, type RouteRecordRaw } from "vue-router"
import HomePage from "../pages/home.vue"
import AdminPage from "../pages/admin.vue"
import FlowerDetailPage from "../pages/FlowerDetail.vue"
import RegisterPage from "../pages/Register.vue"
import LoginPage from "../pages/Login.vue"
import EmailVerificationPage from "../pages/EmailVerification.vue"
import CheckoutPage from "../pages/CheckoutPage.vue"
import OrderConfirmationPage from "../pages/OrderConfirmation.vue"
import { useAuthStore } from "@/stores/auth"

const routes: RouteRecordRaw[] = [
  {
    path: "/",
    component: HomePage,
    name: "Home",
  },
  {
    path: "/register",
    component: RegisterPage,
    name: "Register",
    meta: { requiresGuest: true }
  },
  {
    path: "/login",
    component: LoginPage,
    name: "Login",
    meta: { requiresGuest: true }
  },
  {
    path: "/email-verification",
    component: EmailVerificationPage,
    name: "EmailVerification",
  },
  {
    path: "/admin",
    component: AdminPage,
    name: "Admin",
    meta: { requiresAuth: true }
  },
  {
    path: "/flower/:slug",
    component: FlowerDetailPage,
    name: "FlowerDetail",
  },
  {
    path: "/checkout",
    component: CheckoutPage,
    name: "Checkout",
  },
  {
    path: "/order/:id",
    component: OrderConfirmationPage,
    name: "OrderConfirmation",
  },
]

const router = createRouter({
  history: createWebHistory(),
  routes,
})

// Navigation guards
router.beforeEach((to, from, next) => {
  const authStore = useAuthStore()

  // Check if route requires authentication
  if (to.meta.requiresAuth && !authStore.isAuthenticated) {
    next({ name: 'Login', query: { redirect: to.fullPath } })
    return
  }

  // Check if route is for guests only (already logged in users)
  if (to.meta.requiresGuest && authStore.isAuthenticated) {
    next({ name: 'Home' })
    return
  }

  next()
})

export default router

