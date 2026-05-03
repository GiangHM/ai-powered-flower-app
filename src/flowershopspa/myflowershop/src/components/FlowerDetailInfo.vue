<template>
  <div class="flex flex-col gap-4">
    <span
      v-if="flower.categoryName"
      class="inline-block self-start bg-pink-100 text-pink-700 text-sm font-medium px-3 py-1 rounded-full"
    >
      {{ flower.categoryName }}
    </span>

    <h2 class="text-3xl font-bold text-gray-900">{{ flower.name }}</h2>

    <p class="text-2xl font-bold text-pink-600">
      {{ formatPrice(flower.unitPrice, flower.unitCurrency) }}
    </p>

    <div class="flex items-center gap-2">
      <span
        :class="flower.stockQuantity > 0 ? 'bg-green-100 text-green-700' : 'bg-red-100 text-red-700'"
        class="inline-flex items-center gap-1 text-sm font-medium px-3 py-1 rounded-full"
      >
        <span
          :class="flower.stockQuantity > 0 ? 'bg-green-500' : 'bg-red-500'"
          class="w-2 h-2 rounded-full inline-block"
        ></span>
        {{ flower.stockQuantity > 0 ? `In Stock (${flower.stockQuantity})` : 'Out of Stock' }}
      </span>
    </div>

    <div class="flex items-center gap-4 mt-2">
      <div class="flex items-center border border-gray-300 rounded-lg overflow-hidden">
        <button
          @click="decrementQty"
          class="px-3 py-2 bg-gray-50 hover:bg-gray-100 text-gray-700 transition-colors text-lg font-bold"
          :disabled="quantity <= 1"
        >−</button>
        <span class="px-5 py-2 text-gray-900 font-semibold min-w-[3rem] text-center">{{ quantity }}</span>
        <button
          @click="incrementQty"
          class="px-3 py-2 bg-gray-50 hover:bg-gray-100 text-gray-700 transition-colors text-lg font-bold"
        >+</button>
      </div>
      <button
        @click="addToCart"
        :disabled="flower.stockQuantity === 0"
        class="flex-1 flex items-center justify-center gap-2 bg-orange-500 hover:bg-orange-600 disabled:bg-gray-300 disabled:cursor-not-allowed text-white font-semibold py-3 px-6 rounded-lg transition-colors"
      >
        <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M3 3h2l.4 2M7 13h10l4-8H5.4M7 13l-1.4 7h12.8M7 13L5.4 5M17 13l1.4 7M9 21a1 1 0 100-2 1 1 0 000 2zm8 0a1 1 0 100-2 1 1 0 000 2z" />
        </svg>
        Add to Cart
      </button>
    </div>

    <transition name="fade">
      <p v-if="cartMessage" class="text-green-600 text-sm font-medium">{{ cartMessage }}</p>
    </transition>

    <div v-if="flower.description" class="mt-4 pt-4 border-t border-gray-100">
      <h3 class="text-lg font-semibold text-gray-900 mb-2">Your Flower</h3>
      <p class="text-gray-700 leading-relaxed whitespace-pre-line">{{ flower.description }}</p>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import type { FlowerDetail } from '@/models/flowers/flower'
import { useCartStore } from '@/stores/useCartStore'

interface Props {
  flower: FlowerDetail
}

const props = defineProps<Props>()

const cartStore = useCartStore()
const quantity = ref(1)
const cartMessage = ref<string | null>(null)

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

function incrementQty() {
  quantity.value++
}

function decrementQty() {
  if (quantity.value > 1) quantity.value--
}

function addToCart() {
  cartStore.addItem({
    flowerId: props.flower.id,
    name: props.flower.name,
    price: props.flower.unitPrice,
    currency: props.flower.unitCurrency,
    image: props.flower.image,
    quantity: quantity.value,
  })
  cartMessage.value = `✓ ${quantity.value} × "${props.flower.name}" added to cart!`
  setTimeout(() => { cartMessage.value = null }, 3000)
}
</script>

<style scoped>
.fade-enter-active,
.fade-leave-active {
  transition: opacity 0.3s ease;
}
.fade-enter-from,
.fade-leave-to {
  opacity: 0;
}
</style>
