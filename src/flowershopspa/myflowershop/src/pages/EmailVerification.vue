<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth'

const route = useRoute()
const router = useRouter()
const authStore = useAuthStore()

const verifying = ref(false)
const verified = ref(false)
const verificationError = ref('')

onMounted(async () => {
  const token = route.query.token as string

  if (token) {
    // Auto-verify if token is in URL query
    await verifyEmail(token)
  }
})

const verifyEmail = async (token: string) => {
  verifying.value = true
  verificationError.value = ''

  try {
    await authStore.confirmEmail(token)
    verified.value = true

    // Redirect to login after 3 seconds
    setTimeout(() => {
      router.push('/login')
    }, 3000)
  } catch (error: any) {
    verificationError.value = error.response?.data?.message || error.message || 'Verification failed'
  } finally {
    verifying.value = false
  }
}
</script>

<template>
  <div class="min-h-screen bg-gray-50 flex flex-col justify-center py-12 sm:px-6 lg:px-8">
    <div class="sm:mx-auto sm:w-full sm:max-w-md">
      <h2 class="mt-6 text-center text-3xl font-extrabold text-gray-900">
        Email Verification
      </h2>
    </div>

    <div class="mt-8 sm:mx-auto sm:w-full sm:max-w-md">
      <div class="bg-white py-8 px-4 shadow sm:rounded-lg sm:px-10">
        <!-- Verifying State -->
        <div v-if="verifying" class="text-center">
          <div class="inline-block animate-spin rounded-full h-12 w-12 border-b-2 border-indigo-600"></div>
          <p class="mt-4 text-gray-700">Verifying your email...</p>
        </div>

        <!-- Success State -->
        <div v-else-if="verified" class="text-center">
          <div class="mx-auto flex items-center justify-center h-12 w-12 rounded-full bg-green-100">
            <svg class="h-6 w-6 text-green-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M5 13l4 4L19 7"></path>
            </svg>
          </div>
          <h3 class="mt-4 text-lg font-medium text-gray-900">Email Verified!</h3>
          <p class="mt-2 text-sm text-gray-600">
            Your email has been successfully verified. You can now log in to your account.
          </p>
          <p class="mt-4 text-sm text-gray-500">
            Redirecting to login page...
          </p>
        </div>

        <!-- Error State -->
        <div v-else-if="verificationError" class="text-center">
          <div class="mx-auto flex items-center justify-center h-12 w-12 rounded-full bg-red-100">
            <svg class="h-6 w-6 text-red-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12"></path>
            </svg>
          </div>
          <h3 class="mt-4 text-lg font-medium text-gray-900">Verification Failed</h3>
          <p class="mt-2 text-sm text-red-600">
            {{ verificationError }}
          </p>
          <div class="mt-6">
            <router-link
              to="/login"
              class="inline-flex justify-center py-2 px-4 border border-transparent rounded-md shadow-sm text-sm font-medium text-white bg-indigo-600 hover:bg-indigo-700"
            >
              Go to Login
            </router-link>
          </div>
        </div>

        <!-- Waiting for Verification State -->
        <div v-else class="text-center">
          <svg class="mx-auto h-12 w-12 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M3 8l7.89 5.26a2 2 0 002.22 0L21 8M5 19h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v10a2 2 0 002 2z"></path>
          </svg>
          <h3 class="mt-4 text-lg font-medium text-gray-900">Check Your Email</h3>
          <p class="mt-2 text-sm text-gray-600">
            We've sent a verification link to your email address.
          </p>
          <p class="mt-2 text-sm text-gray-600">
            Please click the link in the email to verify your account.
          </p>
          <div class="mt-6">
            <router-link
              to="/login"
              class="text-sm font-medium text-indigo-600 hover:text-indigo-500"
            >
              Back to Login
            </router-link>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>
