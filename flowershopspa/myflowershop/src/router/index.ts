import { createRouter, createWebHistory, type RouteRecordRaw } from "vue-router"
import HomePage from "../pages/home.vue"
import AdminPage from "../pages/admin.vue"

const routes: RouteRecordRaw[] = [
  {
    path: "/",
    component: HomePage,
    name: "Home",
  },
  {
    path: "/admin",
    component: AdminPage,
    name: "Admin",
  },
]

const router = createRouter({
  history: createWebHistory(),
  routes,
})

export default router

