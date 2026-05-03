import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import authService from '@/services/auth.service'
import type { RegisterDto, LoginDto, User } from '@/models/auth/auth'

export const useAuthStore = defineStore('auth', () => {
  const user = ref<User | null>(null)
  const token = ref<string | null>(authService.getToken())
  const loading = ref(false)
  const error = ref<string | null>(null)

  const isAuthenticated = computed(() => {
    return !!token.value && authService.isAuthenticated()
  })
  const isAdmin = computed(() => {
    return !!token.value && authService.isAdmin()
  })
  const isEmailConfirmed = computed(() => {
    return user.value?.emailConfirmed ?? true // Set to true for now since email verification is deactivated in backend
  })

  async function register(data: RegisterDto): Promise<void> {
    loading.value = true
    error.value = null

    try {
      const response = await authService.register(data)
      token.value = response.token
      authService.setToken(response.token)

      // After registration, user needs to verify email
      // Set basic user info (we don't have full user data from register response)
      user.value = {
        id: 0, // Will be populated after email confirmation or login
        name: data.name,
        email: data.email,
        phone: data.phone,
        deliveryAddress: data.deliveryAddress,
        status: 'Pending',
        emailConfirmed: true // Set to true for now since email verification is deactivated in backend
      }
    } catch (err: any) {
      error.value = err.response?.data?.message || err.message || 'Registration failed'
      throw err
    } finally {
      loading.value = false
    }
  }

  async function login(data: LoginDto): Promise<void> {
    loading.value = true
    error.value = null

    try {
      const response = await authService.login(data)
      token.value = response.data.token
      authService.setToken(response.data.token)

      // Parse JWT to get user info
      const tokenPayload = response.data.token.split('.')[1]
      if (!tokenPayload) {
        throw new Error('Invalid authentication token format')
      }
      const payload = JSON.parse(atob(tokenPayload))
      user.value = {
        id: parseInt(payload.nameid || payload.sub),
        name: payload.name || payload.unique_name || '',
        email: payload.email,
        phone: '',
        deliveryAddress: '',
        status: 'Active',
        emailConfirmed: true
      }
    } catch (err: any) {
      error.value = err.response?.data?.message || err.message || 'Login failed'
      throw err
    } finally {
      loading.value = false
    }
  }

  async function confirmEmail(verificationToken: string): Promise<void> {
    loading.value = true
    error.value = null

    try {
      await authService.confirmEmail({ token: verificationToken })

      // Update user email confirmed status
      if (user.value) {
        user.value.emailConfirmed = true
        user.value.status = 'Active'
      }
    } catch (err: any) {
      error.value = err.response?.data?.message || err.message || 'Email confirmation failed'
      throw err
    } finally {
      loading.value = false
    }
  }

  function logout(): void {
    authService.logout()
    user.value = null
    token.value = null
    error.value = null
  }

  function clearError(): void {
    error.value = null
  }

  return {
    user,
    token,
    loading,
    error,
    isAuthenticated,
    isAdmin,
    isEmailConfirmed,
    register,
    login,
    confirmEmail,
    logout,
    clearError
  }
})
