<template>
  <Teleport to="body">
    <!-- Backdrop -->
    <Transition name="backdrop">
      <div
        v-if="modelValue"
        class="fixed inset-0 bg-black/50 z-40"
        @click="close"
      />
    </Transition>

    <!-- Drawer panel -->
    <Transition name="drawer">
      <div
        v-if="modelValue"
        class="fixed top-0 right-0 h-full w-full max-w-md bg-white shadow-2xl z-50 flex flex-col"
      >
        <!-- Header -->
        <div class="flex items-center justify-between px-6 py-4 border-b border-gray-100">
          <h2 class="text-xl font-bold text-gray-900">Your Cart</h2>
          <button
            @click="close"
            class="p-2 rounded-full text-gray-500 hover:text-gray-800 hover:bg-gray-100 transition-colors"
            aria-label="Close cart"
          >
            <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" />
            </svg>
          </button>
        </div>

        <!-- Items list -->
        <div class="flex-1 overflow-y-auto px-6 py-4">
          <!-- Empty state -->
          <div v-if="cartStore.items.length === 0" class="flex flex-col items-center justify-center h-full text-center py-16">
            <svg class="w-20 h-20 text-gray-200 mb-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="1.5" d="M3 3h2l.4 2M7 13h10l4-8H5.4M7 13l-1.4 7h12.8M7 13L5.4 5M9 21a1 1 0 100-2 1 1 0 000 2zm8 0a1 1 0 100-2 1 1 0 000 2z" />
            </svg>
            <p class="text-lg font-semibold text-gray-400">Your cart is empty</p>
            <p class="text-sm text-gray-400 mt-1">Add some beautiful flowers!</p>
          </div>

          <!-- Cart items -->
          <ul v-else class="space-y-4">
            <li
              v-for="item in cartStore.items"
              :key="item.flowerId"
              class="flex gap-4 bg-gray-50 rounded-xl p-3"
            >
              <!-- Image -->
              <div class="w-20 h-20 flex-shrink-0 rounded-lg overflow-hidden bg-gray-100">
                <img
                  :src="getImageUrl(item.image)"
                  :alt="item.name"
                  class="w-full h-full object-cover"
                  @error="onImageError"
                />
              </div>

              <!-- Info -->
              <div class="flex-1 min-w-0">
                <p class="font-semibold text-gray-900 text-sm line-clamp-2">{{ item.name }}</p>
                <p class="text-pink-600 text-sm font-bold mt-0.5">{{ formatPrice(item.price, item.currency) }} each</p>

                <!-- Quantity controls -->
                <div class="flex items-center gap-2 mt-2">
                  <button
                    @click="cartStore.updateQuantity(item.flowerId, item.quantity - 1)"
                    class="w-7 h-7 flex items-center justify-center rounded-full bg-white border border-gray-200 text-gray-600 hover:bg-pink-50 hover:border-pink-300 transition-colors text-lg font-bold"
                  >−</button>
                  <span class="text-sm font-semibold text-gray-800 min-w-[1.5rem] text-center">{{ item.quantity }}</span>
                  <button
                    @click="handleIncreaseQuantity(item.flowerId, item.quantity + 1)"
                    :disabled="validatingItemId === item.flowerId"
                    class="w-7 h-7 flex items-center justify-center rounded-full bg-white border border-gray-200 text-gray-600 hover:bg-pink-50 hover:border-pink-300 transition-colors text-lg font-bold"
                  >+</button>
                </div>
                <p
                  v-if="validationErrorByItem[item.flowerId]"
                  class="mt-2 text-xs text-red-600"
                >
                  {{ validationErrorByItem[item.flowerId] }}
                </p>
              </div>

              <!-- Line total + remove -->
              <div class="flex flex-col items-end justify-between flex-shrink-0">
                <button
                  @click="cartStore.removeItem(item.flowerId)"
                  class="p-1 text-gray-400 hover:text-red-500 transition-colors"
                  aria-label="Remove item"
                >
                  <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" />
                  </svg>
                </button>
                <p class="text-sm font-bold text-gray-900">
                  {{ formatPrice(item.price * item.quantity, item.currency) }}
                </p>
              </div>
            </li>
          </ul>
        </div>

        <!-- Footer -->
        <div v-if="cartStore.items.length > 0" class="border-t border-gray-100 px-6 py-4 space-y-3 bg-white">
          <!-- Subtotal -->
          <div class="flex items-center justify-between text-gray-700">
            <span class="text-base font-medium">Subtotal</span>
            <span class="text-xl font-bold text-gray-900">{{ subtotalDisplay }}</span>
          </div>

          <!-- Actions -->
          <div class="flex gap-3">
            <button
              @click="cartStore.clearCart()"
              class="flex-1 py-2.5 px-4 rounded-lg border border-gray-200 text-gray-600 text-sm font-medium hover:bg-gray-50 transition-colors"
            >
              Clear Cart
            </button>
            <button
              @click="goToCheckout"
              class="flex-1 py-2.5 px-4 rounded-lg bg-orange-500 hover:bg-orange-600 text-white text-sm font-semibold transition-colors"
            >
              Checkout
            </button>
          </div>
        </div>
      </div>
    </Transition>
  </Teleport>
</template>

<script setup lang="ts">
import { computed, ref } from 'vue'
import { useRouter } from 'vue-router'
import { useCartStore } from '@/stores/useCartStore'
import { flowerService } from '@/services/flower.service'
import type { CartValidationStatus } from '@/models/flowers/flower'

const props = defineProps<{
  modelValue: boolean
}>()

const emit = defineEmits<{
  'update:modelValue': [value: boolean]
}>()

const router = useRouter()
const cartStore = useCartStore()
const validatingItemId = ref<number | null>(null)
const validationErrorByItem = ref<Record<number, string>>({})

function close() {
  emit('update:modelValue', false)
}

function goToCheckout() {
  emit('update:modelValue', false)
  router.push('/checkout')
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

function getImageUrl(image: string): string {
  if (!image) return 'https://placehold.co/200x200?text=No+Image'
  if (image.startsWith('http://') || image.startsWith('https://')) return image
  return `/src/assets/images/${image}`
}

function onImageError(event: Event) {
  const img = event.target as HTMLImageElement
  img.src = 'https://placehold.co/200x200?text=No+Image'
}

function getValidationMessage(status: CartValidationStatus): string {
  if (status === 'out_of_stock') return 'Not enough stock for this quantity.'
  if (status === 'inactive') return 'This item is currently inactive.'
  if (status === 'not_found') return 'This item is no longer available.'
  return 'Unable to update quantity right now.'
}

async function handleIncreaseQuantity(flowerId: number, nextQuantity: number) {
  if (validatingItemId.value !== null) return

  validatingItemId.value = flowerId
  delete validationErrorByItem.value[flowerId]

  try {
    const response = await flowerService.validateCart({
      items: [{ flowerId, quantity: nextQuantity }]
    })

    const itemResult = response.data.results.find((result) => result.flowerId === flowerId)
    if (itemResult?.status === 'available') {
      cartStore.updateQuantity(flowerId, nextQuantity)
      return
    }

    validationErrorByItem.value[flowerId] = getValidationMessage(itemResult?.status ?? 'out_of_stock')
  } catch {
    validationErrorByItem.value[flowerId] = 'Could not validate stock. Please try again.'
  } finally {
    validatingItemId.value = null
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
</script>

<style scoped>
.backdrop-enter-active,
.backdrop-leave-active {
  transition: opacity 0.3s ease;
}
.backdrop-enter-from,
.backdrop-leave-to {
  opacity: 0;
}

.drawer-enter-active,
.drawer-leave-active {
  transition: transform 0.3s ease;
}
.drawer-enter-from,
.drawer-leave-to {
  transform: translateX(100%);
}
</style>
