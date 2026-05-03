<template>
  <div class="min-h-screen bg-gray-50">
    <!-- Header -->
    <div class="bg-white border-b border-gray-100 shadow-sm">
      <div class="max-w-3xl mx-auto px-4 sm:px-6 lg:px-8 py-4">
        <h1 class="text-xl font-bold text-gray-900">Order Confirmation</h1>
      </div>
    </div>

    <main class="max-w-3xl mx-auto px-4 sm:px-6 lg:px-8 py-12">
      <!-- Success card -->
      <div class="bg-white rounded-2xl shadow-sm border border-gray-100 overflow-hidden">
        <!-- Green banner -->
        <div class="bg-gradient-to-r from-green-500 to-emerald-500 px-8 py-10 text-white text-center">
          <div class="flex items-center justify-center mb-4">
            <div class="w-16 h-16 bg-white/20 rounded-full flex items-center justify-center">
              <svg class="w-9 h-9 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2.5" d="M5 13l4 4L19 7" />
              </svg>
            </div>
          </div>
          <h2 class="text-3xl font-extrabold">Order Placed! 🌸</h2>
          <p class="mt-2 text-green-100 text-sm">Thank you for your purchase.</p>
          <div class="mt-4 inline-block bg-white/20 rounded-full px-5 py-1.5 text-sm font-semibold">
            Order #{{ orderId }}
          </div>
        </div>

        <!-- Body -->
        <div class="px-8 py-8 space-y-8">
          <!-- Delivery info -->
          <div v-if="sessionData">
            <h3 class="text-base font-bold text-gray-800 mb-3 flex items-center gap-2">
              <svg class="w-4 h-4 text-pink-500" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17.657 16.657L13.414 20.9a1.998 1.998 0 01-2.827 0l-4.244-4.243a8 8 0 1111.314 0z" />
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 11a3 3 0 11-6 0 3 3 0 016 0z" />
              </svg>
              Delivery Information
            </h3>
            <div class="bg-gray-50 rounded-xl px-5 py-4 space-y-1.5 text-sm">
              <div class="flex items-start gap-2">
                <span class="text-gray-500 w-32 flex-shrink-0">Name</span>
                <span class="font-medium text-gray-900">{{ sessionData.fullName }}</span>
              </div>
              <div class="flex items-start gap-2">
                <span class="text-gray-500 w-32 flex-shrink-0">Email</span>
                <span class="font-medium text-gray-900">{{ sessionData.email }}</span>
              </div>
              <div class="flex items-start gap-2">
                <span class="text-gray-500 w-32 flex-shrink-0">Address</span>
                <span class="font-medium text-gray-900">{{ sessionData.deliveryAddress }}</span>
              </div>
            </div>
          </div>

          <!-- Items list -->
          <div v-if="displayItems.length > 0">
            <h3 class="text-base font-bold text-gray-800 mb-3">Items Ordered</h3>
            <ul class="divide-y divide-gray-100 border border-gray-100 rounded-xl overflow-hidden">
              <li
                v-for="(item, idx) in displayItems"
                :key="idx"
                class="flex items-center gap-4 px-4 py-3 bg-white"
              >
                <!-- Image (only for cart-source items) -->
                <div
                  v-if="isCartItem(item)"
                  class="w-12 h-12 rounded-lg overflow-hidden bg-gray-100 flex-shrink-0"
                >
                  <img
                    :src="getImageUrl((item as CartDisplayItem).image)"
                    :alt="(item as CartDisplayItem).name"
                    class="w-full h-full object-cover"
                    @error="onImageError"
                  />
                </div>

                <!-- Info -->
                <div class="flex-1 min-w-0">
                  <p class="text-sm font-semibold text-gray-800 truncate">
                    {{ isCartItem(item) ? (item as CartDisplayItem).name : (item as ApiDisplayItem).flowerName }}
                  </p>
                  <p class="text-xs text-gray-500 mt-0.5">
                    Qty: {{ item.quantity }}
                    <span v-if="isCartItem(item) && (item as CartDisplayItem).price">
                      · {{ formatPrice((item as CartDisplayItem).price, (item as CartDisplayItem).currency) }} each
                    </span>
                    <span v-else-if="!isCartItem(item) && (item as ApiDisplayItem).unitPrice">
                      · {{ formatPrice((item as ApiDisplayItem).unitPrice, 'USD') }} each
                    </span>
                  </p>
                </div>

                <!-- Line total -->
                <p v-if="isCartItem(item)" class="text-sm font-bold text-gray-900 flex-shrink-0">
                  {{ formatPrice((item as CartDisplayItem).price * item.quantity, (item as CartDisplayItem).currency) }}
                </p>
                <p v-else class="text-sm font-bold text-gray-900 flex-shrink-0">
                  {{ formatPrice((item as ApiDisplayItem).unitPrice * item.quantity, 'USD') }}
                </p>
              </li>
            </ul>
          </div>

          <!-- Total -->
          <div class="flex items-center justify-between border-t border-gray-100 pt-4">
            <span class="text-base font-semibold text-gray-700">Total</span>
            <span class="text-xl font-extrabold text-gray-900">{{ totalDisplay }}</span>
          </div>

          <!-- Estimated delivery -->
          <div class="flex items-center gap-3 bg-pink-50 border border-pink-100 rounded-xl px-5 py-4">
            <span class="text-2xl">🚚</span>
            <div>
              <p class="text-sm font-semibold text-pink-800">Estimated Delivery</p>
              <p class="text-sm text-pink-600 mt-0.5">3–5 business days</p>
            </div>
          </div>

          <!-- COD reminder -->
          <div class="flex items-center gap-3 bg-orange-50 border border-orange-100 rounded-xl px-5 py-4">
            <span class="text-2xl">💵</span>
            <div>
              <p class="text-sm font-semibold text-orange-800">Cash on Delivery</p>
              <p class="text-sm text-orange-600 mt-0.5">Please have the payment ready when your flowers arrive.</p>
            </div>
          </div>

          <!-- Continue shopping -->
          <div class="text-center pt-2">
            <button
              @click="router.push('/')"
              class="inline-flex items-center gap-2 bg-pink-600 hover:bg-pink-700 text-white font-semibold
                     px-8 py-3 rounded-xl transition-colors text-sm"
            >
              <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M3 3h2l.4 2M7 13h10l4-8H5.4M7 13l-1.4 7h12.8M9 21a1 1 0 100-2 1 1 0 000 2zm8 0a1 1 0 100-2 1 1 0 000 2z" />
              </svg>
              Continue Shopping
            </button>
          </div>
        </div>
      </div>

      <!-- Fallback when no session data -->
      <div
        v-if="!sessionData"
        class="mt-6 bg-blue-50 border border-blue-100 rounded-xl px-6 py-5 text-center"
      >
        <p class="text-sm text-blue-700">
          Your order <strong>#{{ orderId }}</strong> has been received.
          A confirmation email will be sent shortly.
        </p>
      </div>
    </main>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth'
import { orderService } from '@/services/order.service'
import type { LastOrderSession, OrderItemResponseDto } from '@/models/order'
import { LAST_ORDER_SESSION_KEY } from '@/config/order.constants'

const route = useRoute()
const router = useRouter()
const authStore = useAuthStore()

const orderId = computed(() => Number(route.params.id))

const sessionData = ref<LastOrderSession | null>(null)
const apiOrder = ref<{ items: OrderItemResponseDto[]; totalAmount: number } | null>(null)

// ── Display helpers ───────────────────────────────────────────────────────────

interface CartDisplayItem {
  flowerId: number
  name: string
  price: number
  currency: string
  image: string
  quantity: number
}

interface ApiDisplayItem {
  id: number
  flowerId: number
  flowerName: string
  quantity: number
  unitPrice: number
}

type DisplayItem = CartDisplayItem | ApiDisplayItem

function isCartItem(item: DisplayItem): item is CartDisplayItem {
  return 'name' in item && 'price' in item
}

const displayItems = computed<DisplayItem[]>(() => {
  // Prefer sessionStorage snapshot (includes images)
  if (sessionData.value?.items?.length) {
    return sessionData.value.items as CartDisplayItem[]
  }
  // Fall back to API response
  if (apiOrder.value?.items?.length) {
    return apiOrder.value.items as ApiDisplayItem[]
  }
  return []
})

const totalDisplay = computed(() => {
  if (sessionData.value?.totalAmount != null) {
    return formatPrice(sessionData.value.totalAmount, 'USD')
  }
  if (apiOrder.value?.totalAmount != null) {
    return formatPrice(apiOrder.value.totalAmount, 'USD')
  }
  return '—'
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

// ── Mount ─────────────────────────────────────────────────────────────────────

onMounted(async () => {
  // Read sessionStorage snapshot
  const raw = sessionStorage.getItem(LAST_ORDER_SESSION_KEY)
  if (raw) {
    try {
      const parsed = JSON.parse(raw) as LastOrderSession
      // Only use if it matches the current order
      if (parsed.orderId === orderId.value) {
        sessionData.value = parsed
      }
    } catch {
      // ignore parse errors
    }
  }

  // If authenticated, also try to fetch from API for up-to-date order details
  if (authStore.isAuthenticated && orderId.value) {
    try {
      const order = await orderService.getOrderById(orderId.value)
      apiOrder.value = { items: order.items, totalAmount: order.totalAmount }
    } catch {
      // Not critical — session data is sufficient for display
    }
  }
})
</script>
