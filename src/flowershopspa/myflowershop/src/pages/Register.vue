<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth'
import type { RegisterDto } from '@/models/auth/auth'

const router = useRouter()
const authStore = useAuthStore()

const form = ref<RegisterDto>({
  name: '',
  email: '',
  password: '',
  phone: '',
  deliveryAddress: ''
})

const confirmPassword = ref('')
const passwordMismatch = ref(false)
const successMessage = ref('')

const validateForm = (): boolean => {
  if (form.value.password !== confirmPassword.value) {
    passwordMismatch.value = true
    return false
  }
  passwordMismatch.value = false
  return true
}

const handleRegister = async () => {
  if (!validateForm()) return

  try {
    await authStore.register(form.value)
    successMessage.value = 'Registration successful! Please check your email to verify your account.'

    // Redirect to email verification page after 3 seconds
    setTimeout(() => {
      router.push('/email-verification')
    }, 3000)
  } catch (error) {
    // Error is handled by the store
    console.error('Registration error:', error)
  }
}
</script>

<template>
  <div class="min-h-screen bg-gray-50 flex flex-col justify-center py-12 sm:px-6 lg:px-8">
    <div class="sm:mx-auto sm:w-full sm:max-w-md">
      <h2 class="mt-6 text-center text-3xl font-extrabold text-gray-900">
        Create your account
      </h2>
      <p class="mt-2 text-center text-sm text-gray-600">
        Already have an account?
        <router-link to="/login" class="font-medium text-indigo-600 hover:text-indigo-500">
          Sign in
        </router-link>
      </p>
    </div>

    <div class="mt-8 sm:mx-auto sm:w-full sm:max-w-md">
      <div class="bg-white py-8 px-4 shadow sm:rounded-lg sm:px-10">
        <div v-if="successMessage" class="mb-4 bg-green-50 border border-green-400 text-green-700 px-4 py-3 rounded">
          {{ successMessage }}
        </div>

        <div v-if="authStore.error" class="mb-4 bg-red-50 border border-red-400 text-red-700 px-4 py-3 rounded">
          {{ authStore.error }}
        </div>

        <form @submit.prevent="handleRegister" class="space-y-6">
          <div>
            <label for="name" class="block text-sm font-medium text-gray-700">
              Full Name
            </label>
            <input
              id="name"
              v-model="form.name"
              type="text"
              required
              class="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-indigo-500 focus:border-indigo-500"
            />
          </div>

          <div>
            <label for="email" class="block text-sm font-medium text-gray-700">
              Email Address
            </label>
            <input
              id="email"
              v-model="form.email"
              type="email"
              required
              class="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-indigo-500 focus:border-indigo-500"
            />
          </div>

          <div>
            <label for="password" class="block text-sm font-medium text-gray-700">
              Password
            </label>
            <input
              id="password"
              v-model="form.password"
              type="password"
              required
              class="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-indigo-500 focus:border-indigo-500"
            />
          </div>

          <div>
            <label for="confirmPassword" class="block text-sm font-medium text-gray-700">
              Confirm Password
            </label>
            <input
              id="confirmPassword"
              v-model="confirmPassword"
              type="password"
              required
              class="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-indigo-500 focus:border-indigo-500"
              :class="{ 'border-red-500': passwordMismatch }"
            />
            <p v-if="passwordMismatch" class="mt-1 text-sm text-red-600">
              Passwords do not match
            </p>
          </div>

          <div>
            <label for="phoneNumber" class="block text-sm font-medium text-gray-700">
              Phone Number
            </label>
            <input
              id="phoneNumber"
              v-model="form.phone"
              type="tel"
              required
              class="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-indigo-500 focus:border-indigo-500"
            />
          </div>

          <div>
            <label for="deliveryAddress" class="block text-sm font-medium text-gray-700">
              Delivery Address
            </label>
            <textarea
              id="deliveryAddress"
              v-model="form.deliveryAddress"
              required
              rows="3"
              class="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-indigo-500 focus:border-indigo-500"
            ></textarea>
          </div>

          <div>
            <button
              type="submit"
              :disabled="authStore.loading"
              class="w-full flex justify-center py-2 px-4 border border-transparent rounded-md shadow-sm text-sm font-medium text-white bg-indigo-600 hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500 disabled:bg-gray-400 disabled:cursor-not-allowed"
            >
              <span v-if="authStore.loading">Creating account...</span>
              <span v-else>Create Account</span>
            </button>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>
