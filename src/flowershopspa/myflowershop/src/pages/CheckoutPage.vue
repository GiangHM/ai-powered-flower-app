<template>
  <div class="min-h-screen bg-gray-50">
    <!-- Header / Back link -->
    <div class="bg-white border-b border-gray-100 shadow-sm">
      <div class="max-w-5xl mx-auto px-4 sm:px-6 lg:px-8 py-4 flex items-center gap-3">
        <button
          @click="router.push('/')"
          class="flex items-center gap-1.5 text-sm text-gray-500 hover:text-pink-600 transition-colors"
        >
          <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 19l-7-7 7-7" />
          </svg>
          Back to Shop
        </button>
        <span class="text-gray-300">|</span>
        <h1 class="text-xl font-bold text-gray-900">Checkout</h1>
      </div>
    </div>

    <main class="max-w-5xl mx-auto px-4 sm:px-6 lg:px-8 py-10">
      <!-- Error banner -->
      <div
        v-if="errorMessage"
        class="mb-6 flex items-start gap-3 bg-red-50 border border-red-200 text-red-700 rounded-xl px-5 py-4"
      >
        <svg class="w-5 h-5 mt-0.5 flex-shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8v4m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
        </svg>
        <p class="text-sm font-medium">{{ errorMessage }}</p>
      </div>

      <div class="grid grid-cols-1 lg:grid-cols-5 gap-8">
        <!-- Left column: Delivery form -->
        <section class="lg:col-span-3 space-y-6">
          <div class="bg-white rounded-2xl shadow-sm border border-gray-100 p-6">
            <h2 class="text-lg font-bold text-gray-900 mb-5 flex items-center gap-2">
              <svg class="w-5 h-5 text-pink-500" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17.657 16.657L13.414 20.9a1.998 1.998 0 01-2.827 0l-4.244-4.243a8 8 0 1111.314 0z" />
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 11a3 3 0 11-6 0 3 3 0 016 0z" />
              </svg>
              Delivery Details
            </h2>

            <form @submit.prevent="placeOrder" class="space-y-4" novalidate>
              <!-- Full Name -->
              <div>
                <label for="fullName" class="block text-sm font-medium text-gray-700 mb-1">
                  Full Name <span class="text-red-500">*</span>
                </label>
                <input
                  id="fullName"
                  v-model="form.fullName"
                  type="text"
                  autocomplete="name"
                  placeholder="Jane Smith"
                  class="w-full rounded-lg border border-gray-200 px-4 py-2.5 text-sm text-gray-900 placeholder-gray-400
                         focus:outline-none focus:ring-2 focus:ring-pink-400 focus:border-transparent transition"
                  :class="{ 'border-red-400 focus:ring-red-300': fieldErrors.fullName }"
                />
                <p v-if="fieldErrors.fullName" class="mt-1 text-xs text-red-500">{{ fieldErrors.fullName }}</p>
              </div>

              <!-- Email -->
              <div>
                <label for="email" class="block text-sm font-medium text-gray-700 mb-1">
                  Email <span class="text-red-500">*</span>
                </label>
                <input
                  id="email"
                  v-model="form.email"
                  type="email"
                  autocomplete="email"
                  placeholder="jane@example.com"
                  class="w-full rounded-lg border border-gray-200 px-4 py-2.5 text-sm text-gray-900 placeholder-gray-400
                         focus:outline-none focus:ring-2 focus:ring-pink-400 focus:border-transparent transition"
                  :class="{ 'border-red-400 focus:ring-red-300': fieldErrors.email }"
                />
                <p v-if="fieldErrors.email" class="mt-1 text-xs text-red-500">{{ fieldErrors.email }}</p>
              </div>

              <!-- Phone -->
              <div>
                <label for="phone" class="block text-sm font-medium text-gray-700 mb-1">
                  Phone Number <span class="text-red-500">*</span>
                </label>
                <input
                  id="phone"
                  v-model="form.phoneNumber"
                  type="tel"
                  autocomplete="tel"
                  placeholder="+1 555 000 0000"
                  class="w-full rounded-lg border border-gray-200 px-4 py-2.5 text-sm text-gray-900 placeholder-gray-400
                         focus:outline-none focus:ring-2 focus:ring-pink-400 focus:border-transparent transition"
                  :class="{ 'border-red-400 focus:ring-red-300': fieldErrors.phoneNumber }"
                />
                <p v-if="fieldErrors.phoneNumber" class="mt-1 text-xs text-red-500">{{ fieldErrors.phoneNumber }}</p>
              </div>

              <!-- Delivery Address -->
              <div>
                <label for="address" class="block text-sm font-medium text-gray-700 mb-1">
                  Delivery Address <span class="text-red-500">*</span>
                </label>
                <textarea
                  id="address"
                  v-model="form.deliveryAddress"
                  rows="3"
                  autocomplete="street-address"
                  placeholder="123 Rose Ave, Suite 4, New York, NY 10001"
                  class="w-full rounded-lg border border-gray-200 px-4 py-2.5 text-sm text-gray-900 placeholder-gray-400
                         focus:outline-none focus:ring-2 focus:ring-pink-400 focus:border-transparent transition resize-none"
                  :class="{ 'border-red-400 focus:ring-red-300': fieldErrors.deliveryAddress }"
                />
                <p v-if="fieldErrors.deliveryAddress" class="mt-1 text-xs text-red-500">{{ fieldErrors.deliveryAddress }}</p>
              </div>
            </form>
          </div>

          <!-- Payment info -->
          <div class="bg-orange-50 border border-orange-100 rounded-2xl px-6 py-4 flex items-start gap-3">
            <span class="text-2xl">💵</span>
            <div>
              <p class="font-semibold text-orange-800 text-sm">Cash on Delivery</p>
              <p class="text-orange-600 text-sm mt-0.5">Pay when your flowers arrive. No payment required now.</p>
            </div>
          </div>
        </section>

        <!-- Right column: Order summary -->
        <section class="lg:col-span-2">
          <div class="bg-white rounded-2xl shadow-sm border border-gray-100 p-6 sticky top-6">
            <h2 class="text-lg font-bold text-gray-900 mb-4">Order Summary</h2>

            <!-- Empty cart warning -->
            <div v-if="cartStore.items.length === 0" class="text-center py-8">
              <p class="text-gray-400 text-sm">Your cart is empty.</p>
              <button
                @click="router.push('/')"
                class="mt-3 text-sm text-pink-600 hover:underline font-medium"
              >
                Browse flowers →
              </button>
            </div>

            <!-- Item list -->
            <ul v-else class="divide-y divide-gray-100 -mx-2">
              <li
                v-for="item in cartStore.items"
                :key="item.flowerId"
                class="flex gap-3 px-2 py-3"
              >
                <!-- Image -->
                <div class="w-14 h-14 rounded-lg overflow-hidden bg-gray-100 flex-shrink-0">
                  <img
                    :src="getImageUrl(item.image)"
                    :alt="item.name"
                    class="w-full h-full object-cover"
                    @error="onImageError"
                  />
                </div>

                <!-- Details -->
                <div class="flex-1 min-w-0">
                  <p class="text-sm font-semibold text-gray-800 line-clamp-1">{{ item.name }}</p>
                  <p class="text-xs text-gray-500 mt-0.5">
                    {{ formatPrice(item.price, item.currency) }} × {{ item.quantity }}
                  </p>
                </div>

                <!-- Line total -->
                <p class="text-sm font-bold text-gray-900 flex-shrink-0 self-center">
                  {{ formatPrice(item.price * item.quantity, item.currency) }}
                </p>
              </li>
            </ul>

            <!-- Subtotal -->
            <div v-if="cartStore.items.length > 0" class="mt-4 pt-4 border-t border-gray-100">
              <div class="flex items-center justify-between">
                <span class="text-sm font-medium text-gray-600">Subtotal</span>
                <span class="text-base font-bold text-gray-900">{{ subtotalDisplay }}</span>
              </div>
              <p class="text-xs text-gray-400 mt-1">Shipping: Free</p>
            </div>

            <!-- Place order button -->
            <button
              @click="placeOrder"
              :disabled="isSubmitting || cartStore.items.length === 0"
              class="mt-6 w-full py-3 rounded-xl bg-orange-500 hover:bg-orange-600 disabled:opacity-60
                     disabled:cursor-not-allowed text-white font-bold text-sm transition-colors
                     flex items-center justify-center gap-2"
            >
              <svg
                v-if="isSubmitting"
                class="animate-spin w-4 h-4"
                fill="none"
                stroke="currentColor"
                viewBox="0 0 24 24"
              >
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 12a8 8 0 018-8v8z" />
              </svg>
              {{ isSubmitting ? 'Placing Order…' : 'Place Order' }}
            </button>
          </div>
        </section>
      </div>
    </main>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useCartStore } from '@/stores/useCartStore'
import { useAuthStore } from '@/stores/auth'
import { orderService } from '@/services/order.service'
import { LAST_ORDER_SESSION_KEY } from '@/config/order.constants'

const router = useRouter()
const cartStore = useCartStore()
const authStore = useAuthStore()

// ── Form state ────────────────────────────────────────────────────────────────

interface DeliveryForm {
  fullName: string
  email: string
  phoneNumber: string
  deliveryAddress: string
}

const form = ref<DeliveryForm>({
  fullName: '',
  email: '',
  phoneNumber: '',
  deliveryAddress: '',
})

const fieldErrors = ref<Partial<DeliveryForm>>({})
const errorMessage = ref<string | null>(null)
const isSubmitting = ref(false)

// Pre-fill from auth store if logged in
onMounted(() => {
  if (authStore.user) {
    form.value.fullName = authStore.user.fullName ?? ''
    form.value.email = authStore.user.email ?? ''
    form.value.phoneNumber = authStore.user.phoneNumber ?? ''
    form.value.deliveryAddress = authStore.user.deliveryAddress ?? ''
  }
})

// ── Helpers ───────────────────────────────────────────────────────────────────

function getImageUrl(image: string): string {
  if (!image) return 'https://placehold.co/200x200?text=No+Image'
  if (image.startsWith('http://') || image.startsWith('https://')) return image
  return `/src/assets/images/${image}`
}

function onImageError(event: Event) {
  const img = event.target as HTMLImageElement
  img.src = 'https://placehold.co/200x200?text=No+Image'
}

function formatPrice(price: number, currency: string): string {
  try {
    return new Intl.NumberFormat('en-US', {
      style: 'currency',
      currency: currency || 'USD',
    }).format(price)
  } catch {
    return `${currency} ${price}`
  }
}

const subtotalDisplay = computed(() => {
  const byCurrency = cartStore.items.reduce<Record<string, number>>((acc, item) => {
    const key = item.currency || 'USD'
    acc[key] = (acc[key] ?? 0) + item.price * item.quantity
    return acc
  }, {})

  return Object.entries(byCurrency)
    .map(([currency, total]) => formatPrice(total, currency))
    .join(' + ')
})

// ── Validation ────────────────────────────────────────────────────────────────

function validate(): boolean {
  const errors: Partial<DeliveryForm> = {}
  const f = form.value

  if (!f.fullName.trim()) errors.fullName = 'Full name is required.'
  if (!f.email.trim()) {
    errors.email = 'Email is required.'
  } else if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(f.email.trim())) {
    errors.email = 'Please enter a valid email address.'
  }
  if (!f.phoneNumber.trim()) errors.phoneNumber = 'Phone number is required.'
  if (!f.deliveryAddress.trim()) errors.deliveryAddress = 'Delivery address is required.'

  fieldErrors.value = errors
  return Object.keys(errors).length === 0
}

// ── Place order ───────────────────────────────────────────────────────────────

async function placeOrder() {
  errorMessage.value = null

  if (!validate()) return

  if (cartStore.items.length === 0) {
    errorMessage.value = 'Your cart is empty. Add some flowers before checking out.'
    return
  }

  isSubmitting.value = true

  try {
    // Snapshot items before clearing
    const itemsSnapshot = cartStore.items.map(i => ({ ...i }))

    const result = await orderService.placeOrder({
      customerId: authStore.user?.id ?? null,
      deliveryName: form.value.fullName,
      deliveryEmail: form.value.email,
      deliveryPhone: form.value.phoneNumber,
      items: cartStore.items.map(i => ({
        flowerId: i.flowerId,
        quantity: i.quantity,
      })),
    })

    // Persist delivery + order info for confirmation page
    // Note: phoneNumber is intentionally omitted from session storage
    // to avoid storing sensitive PII in clear text.
    const lastOrder = {
      orderId: result.id,
      fullName: form.value.fullName,
      email: form.value.email,
      deliveryAddress: form.value.deliveryAddress,
      items: itemsSnapshot,
      totalAmount: result.totalAmount,
      status: result.status,
      orderDate: result.orderDate,
    }
    sessionStorage.setItem(LAST_ORDER_SESSION_KEY, JSON.stringify(lastOrder))

    cartStore.clearCart()
    router.push(`/order/${result.id}`)
  } catch (err: unknown) {
    const axiosErr = err as { response?: { data?: { message?: string } }; message?: string }
    errorMessage.value =
      axiosErr?.response?.data?.message ||
      axiosErr?.message ||
      'Failed to place your order. Please try again.'
  } finally {
    isSubmitting.value = false
  }
}
</script>
